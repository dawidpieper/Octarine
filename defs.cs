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
public OctarineLanguage(string name, string code) {
Name=name;
Code=code;
}
}
}