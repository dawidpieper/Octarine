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
public class Win10Engine : Engine {
private Language language;
public Win10Engine() {
var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
this.language = new Windows.Globalization.Language(topUserLanguage);
}
public override string name {get{return "Windows 10 OCR";}}
public override async Task<(string, OctarineError, string)> GetTextFromFileAsync(string filePath) {
if(!OcrEngine.IsLanguageSupported(language))
return (null, OctarineError.LanguageNotSupported,null);
try {
var engine = OcrEngine.TryCreateFromLanguage(language);
var file = await StorageFile.GetFileFromPathAsync(filePath);
if(Path.GetExtension(filePath).ToLower()==".pdf") {
var pdfDoc = await PdfDocument.LoadFromFileAsync(file);
uint pageCount = pdfDoc.PageCount;
var sb = new StringBuilder();
for (uint i = 0; i < pageCount; i++) {
using (PdfPage page = pdfDoc.GetPage(i)) {
var stream = new InMemoryRandomAccessStream();
await page.RenderToStreamAsync(stream);
var decoder = await BitmapDecoder.CreateAsync(stream);
var softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
OcrResult result = await engine.RecognizeAsync(softwareBitmap);
if(i>0) sb.Append("\r\n");
sb.Append(result.Text);
}
}
return (sb.ToString(), OctarineError.Success,null);
} else {
var stream = await file.OpenAsync(FileAccessMode.Read);
var decoder = await BitmapDecoder.CreateAsync(stream);
var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

var ocrResult = await engine.RecognizeAsync(softwareBitmap);

return (ocrResult.Text, OctarineError.Success,null);
}
} catch(Exception ex) {
return (null, OctarineError.WrongFileFormat, ex.Message);
}
}

public override Language[] languages {get{
var langs = new List<Language>();
foreach(var lang in OcrEngine.AvailableRecognizerLanguages) langs.Add(lang);
return langs.ToArray();
}}

public override void SetLanguage(Language lang) {
this.language = lang;
}

public override Language currentLanguage {get{
return this.language;
}}
}
}