using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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
public Task<(string, OctarineError,string)> GetTextFromStreamAsync(IRandomAccessStream stream);
public Language currentLanguage {get;}
public Language[] Languages {get;}
public void SetLanguage(Language lang);
public bool ShouldRegister{get;}
}

public static class IEngineExtensions {
public static string ReadConfig(this IEngine engine, string value) {
return Config.ReadConfig(value, @"engines\"+engine.ID);
}
public static void WriteConfig(this IEngine engine, string value, string data) {
Config.WriteConfig(value, data, @"engines\"+engine.ID);
}
}
}