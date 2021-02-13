using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Octarine {
public class OctarineEngines {
private static List<OctarineEngine.iEngine> registeredEngines = new List<OctarineEngine.iEngine>();

public static OctarineEngine.iEngine[] engines {get{
return registeredEngines.ToArray();
}}

public static void RegisterEngine(OctarineEngine.iEngine engine) {
registeredEngines.Add(engine);
}
}
}

namespace Octarine.OctarineEngine {
public interface iEngine {
public string Name {get;}
public Task<(string, OctarineError,string)> GetTextFromStreamAsync(IRandomAccessStream stream);
public Language currentLanguage {get;}
public Language[] languages {get;}
public void SetLanguage(Language lang);
public bool ShouldRegister{get;}
}
}