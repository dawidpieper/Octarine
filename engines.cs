using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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
public abstract string Name {get;}
public abstract Task<(string, OctarineError,string)> GetTextFromStreamAsync(IRandomAccessStream stream);
public abstract Language currentLanguage {get;}
public abstract Language[] languages {get;}
public abstract void SetLanguage(Language lang);
}
}