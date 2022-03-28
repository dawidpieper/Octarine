using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Octarine {
public class OctarineEngines {
private static List<OctarineEngine.IEngine> registeredEngines = new List<OctarineEngine.IEngine>();

public static OctarineEngine.IEngine[] engines {get{
return registeredEngines.ToArray();
}}

public static void RegisterEngine(OctarineEngine.IEngine engine) {
registeredEngines.Add(engine);
}
}
}

namespace Octarine.OctarineEngine {
public interface IEngine {
public string ID {get;}
public string Name {get;}
public Task<(OCRPage, OctarineError,string)> GetTextFromStreamAsync(Stream stream);
public OctarineLanguage CurrentLanguage {get;}
public int CurrentQuality {get;}
public OctarineLanguage[] Languages {get;}
public void SetLanguage(OctarineLanguage code, int quality=0);
public bool ShouldRegister{get;}
public bool CanEnable(bool auto=true);
public bool CanDownloadLanguages{get;}
public OctarineLanguage[] GetDownloadableLanguages();
public bool SecondaryLanguagesSupported{get;}
public bool AddSecondaryLanguage(OctarineLanguage lang);
public bool ClearSecondaryLanguages();
public OctarineLanguage[] GetSecondaryLanguages();
}

public static class IEngineExtensions {
public static string ReadConfig(this IEngine engine, string value) {
return Config.ReadConfig(value, @"engines\"+engine.ID);
}
public static void WriteConfig(this IEngine engine, string value, string data) {
Config.WriteConfig(value, data, @"engines\"+engine.ID);
}
public static int ReadConfigInt(this IEngine engine, string value) {
return Config.ReadConfigInt(value, @"engines\"+engine.ID);
}
public static void WriteConfig(this IEngine engine, string value, int data) {
Config.WriteConfig(value, data, @"engines\"+engine.ID);
}

public static string GetDataDir(this IEngine engine, string subdir=null) {
string basedir="engines\\"+engine.ID;
string dir=basedir;
if(subdir!=null) dir+="\\"+subdir;
return Config.GetDataDir(dir);
}
}
}