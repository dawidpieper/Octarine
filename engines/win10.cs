    using System;
using System.Text;
    using System.IO;
    using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Globalization;
using Windows.Data.Pdf;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.Graphics.Imaging;

namespace Octarine.OctarineEngine {
public class Win10Engine : IEngine {
private Language language;
public Win10Engine() {
var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
this.language = new Windows.Globalization.Language(topUserLanguage);
}
public string ID {get{return "Win10";}}
public string Name {get{return "Windows 10 OCR";}}
public bool ShouldRegister {get{return true;}}

public async Task<(string, OctarineError, string)> GetTextFromStreamAsync(Stream istream) {
IRandomAccessStream stream = istream.AsRandomAccessStream();
if(!OcrEngine.IsLanguageSupported(language))
return (null, OctarineError.LanguageNotSupported,null);
try {
var engine = OcrEngine.TryCreateFromLanguage(language);
var decoder = await BitmapDecoder.CreateAsync(stream);
var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
var ocrResult = await engine.RecognizeAsync(softwareBitmap);
var sb = new StringBuilder();
foreach(OcrLine line in ocrResult.Lines) {
sb.Append(line.Text);
sb.Append("\n");
}
return (sb.ToString(), OctarineError.Success,null);
} catch(Exception ex) {
return (null, OctarineError.EngineError, ex.Message);
}
}

public OctarineLanguage[] Languages {get{
var langs = new List<OctarineLanguage>();
foreach(var lang in OcrEngine.AvailableRecognizerLanguages) langs.Add(new OctarineLanguage(lang.DisplayName, lang.LanguageTag));
return langs.ToArray();
}}

public void SetLanguage(OctarineLanguage lng) {
foreach(var lang in OcrEngine.AvailableRecognizerLanguages) 
if(lng.Code == lang.LanguageTag)
this.language = lang;
}

public OctarineLanguage currentLanguage {get{
if(this.language==null) return null;
return new OctarineLanguage(this.language.DisplayName, this.language.LanguageTag);
}}

public bool CanEnable(bool auto=true) {return true;}
}
}