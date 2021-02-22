using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using Windows.Globalization;
using Octarine.OctarineEngine;

namespace Octarine {
public class Program {

private static void LoadPlugins() {
string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+@"\plugins";
if(!Directory.Exists(dir)) return;
string[] plugins = Directory.GetFiles(dir);
foreach(string plugin in plugins)
if(plugin.ToLower().EndsWith(".octarine.dll")) {
try {
Assembly.LoadFile(Path.GetFullPath(plugin));
} catch{}
}
}

private static List<Type> loadedTypes=null;

private static Type[] GetLoadedTypes(bool refresh=false) {
if(loadedTypes==null || refresh) {
loadedTypes = new List<Type>();
Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
foreach (Assembly a in assems) {
try {
foreach(Type p in a.GetTypes()) {
loadedTypes.Add(p);
}
} catch {
DialogResult d = MessageBox.Show("Nie udało się załadować pliku "+new Uri(a.CodeBase).LocalPath+"\nTa wtyczka nie będzie dostępna.", "Błąd podczas ładowania pliku", MessageBoxButtons.AbortRetryIgnore , MessageBoxIcon.Error);
if(d==DialogResult.Abort) Environment.Exit(1);
else if(d==DialogResult.Retry) return GetLoadedTypes(true);
}
}
}
return loadedTypes.ToArray();
}

private static void RegisterEngines() {
Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
foreach(Type p in GetLoadedTypes()) {
try {
if(typeof(OctarineEngine.IEngine).IsAssignableFrom(p) && p.IsClass) {
OctarineEngine.IEngine engine = (OctarineEngine.IEngine)Activator.CreateInstance(p);
if(engine.ShouldRegister) OctarineEngines.RegisterEngine(engine);
}
} catch{}
}
}

private static void RegisterHooks() {
Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
foreach(Type p in GetLoadedTypes()) {
try {
if(typeof(OctarineHook.iHook).IsAssignableFrom(p) && p.IsClass) {
OctarineHook.iHook Hook = (OctarineHook.iHook)Activator.CreateInstance(p);
OctarineHooks.RegisterHook(Hook);
}
} catch{}
}
}

private static void SetEngines() {
foreach(OctarineEngine.IEngine engine in OctarineEngines.engines) {
string lng = engine.ReadConfig("Language");
if(lng!=null) {
int q = engine.ReadConfigInt("Quality");
if(q==0) q=0;
try {
foreach(OctarineLanguage lang in engine.Languages)
if(lang.Code==lng)
engine.SetLanguage(lang, q);
} catch{}
}
}
}

[STAThread]
public static void Main() {

AppDomain currentDomain = AppDomain.CurrentDomain;
currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"\plugins";
string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
if (!File.Exists(assemblyPath)) return null;
Assembly assembly = Assembly.LoadFrom(assemblyPath);
return assembly;
}

LoadPlugins();
RegisterEngines();
RegisterHooks();

OctarineHooks.TriggerEvent(OctarineEvent.ProgramLoaded);

SetEngines();

Application.EnableVisualStyles();

var wnd = new OctarineWindow(new OctarineController());
Application.Run(wnd);
}
}
}