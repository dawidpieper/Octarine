using System;
using System.Collections.Generic;
using System.Linq;

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