using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using System.Threading.Tasks;

namespace Octarine {
public class OctarineEngines {
private static List<OctarineEngine.Engine> registeredEngines = new List<OctarineEngine.Engine>();

public static OctarineEngine.Engine[] engines {get{
return registeredEngines.ToArray();
}}

public static void RegisterEngine(OctarineEngine.Engine engine) {
registeredEngines.Add(engine);
}
}
}

namespace Octarine.OctarineEngine {
public abstract class Engine {
public abstract string name {get;}
public abstract Task<(string, OctarineError)> GetTextFromFileAsync(string filePath);
public abstract Language currentLanguage {get;}
public abstract Language[] languages {get;}
public abstract void SetLanguage(Language lang);
}
}