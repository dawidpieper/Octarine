namespace Octarine {
public enum OctarineError {
Success,
CancellationRequested,
LanguageNotSupported,
WrongFileFormat,
EngineError
}
public enum OctarineEvent {
ProgramLoaded
}

public class OctarineLanguage {
public string Name;
public string Code;
public string[] Sources=null;
public string[] Destinations=null;
public int Qualities;
public OctarineLanguage(string name, string code, int qualities=1) {
Name=name;
Code=code;
Qualities=qualities;
}
}

public struct OctarineDownloadItem {
public string Name;
public string[] Sources;
public string[] Destinations;
public OctarineDownloadItem(string name, string[] sources, string[] destinations) {
Name=name;
Sources=sources;
Destinations=destinations;
}
}
}