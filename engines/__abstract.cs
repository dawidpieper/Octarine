using System.Threading.Tasks;
using Windows.Globalization;

namespace Octarine.OctarineEngine {
public abstract class Engine {
public abstract string name {get;}
public abstract Task<(string, OctarineError)> GetTextFromFileAsync(string filePath);
public abstract Language currentLanguage {get;}
public abstract Language[] languages {get;}
public abstract void SetLanguage(Language lang);
}
}