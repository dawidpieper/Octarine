using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;

namespace Octarine {
public class Program {

private static void LoadPlugins() {
string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+@"\plugins";
if(!Directory.Exists(dir)) return;
string[] plugins = Directory.GetFiles(dir);
foreach(string plugin in plugins)
if(plugin.EndsWith(".dll")) {
try {
Assembly.LoadFile(Path.GetFullPath(plugin));
} catch{}
}
}

private static void RegisterEngines() {
Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => typeof(OctarineEngine.iEngine).IsAssignableFrom(p) && p.IsClass).ToArray();
foreach (Type type in types) {
OctarineEngine.iEngine engine = (OctarineEngine.iEngine)Activator.CreateInstance(type);
if(engine.ShouldRegister) OctarineEngines.RegisterEngine(engine);
}
}

private static void RegisterHooks() {
Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => typeof(OctarineHook.iHook).IsAssignableFrom(p) && p.IsClass).ToArray();
foreach (Type type in types) {
OctarineHook.iHook hook = (OctarineHook.iHook)Activator.CreateInstance(type);
OctarineHooks.RegisterHook(hook);
}
}

[STAThread]
public static void Main() {

AppDomain currentDomain = AppDomain.CurrentDomain;
currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"\plugins\dependencies";
string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
if (!File.Exists(assemblyPath)) return null;
Assembly assembly = Assembly.LoadFrom(assemblyPath);
return assembly;
}

LoadPlugins();
RegisterEngines();
RegisterHooks();

OctarineHooks.TriggerEvent(OctarineEvent.ProgramLoaded);

Application.EnableVisualStyles();

var wnd = new OctarineWindow(new OctarineController());
Application.Run(wnd);
}
}
}