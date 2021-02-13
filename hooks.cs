using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Octarine {
public class OctarineHooks {
private static List<OctarineHook.iHook> registeredHooks = new List<OctarineHook.iHook>();

public static OctarineHook.iHook[] Hooks {get{
return registeredHooks.ToArray();
}}

public static void RegisterHook(OctarineHook.iHook Hook) {
registeredHooks.Add(Hook);
}

public static void TriggerEvent(OctarineEvent ev) {
foreach(OctarineHook.iHook hk in registeredHooks)
hk.TriggerEvent(ev);
}
}
}

namespace Octarine.OctarineHook {
public interface iHook {
public void TriggerEvent(OctarineEvent ev);
}
}