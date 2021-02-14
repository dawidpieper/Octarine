    using System;
using System.Reflection;
using System.Text;
    using System.IO;
    using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Globalization;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.Graphics.Imaging;
using Tesseract;

namespace Octarine.OctarineEngine {
public class TesseractOctarineEngine : iEngine {
private Language language;
public TesseractOctarineEngine() {
string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)+@"\dependencies";
TesseractEnviornment.CustomSearchPath=dir;
var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
this.language = new Windows.Globalization.Language(topUserLanguage);
}
public string ID {get{return "Tesseract";}}
public string Name {get{return "Tesseract";}}
public bool ShouldRegister {get{return true;}}

public async Task<(string, OctarineError, string)> GetTextFromStreamAsync(IRandomAccessStream stream) {
System.Drawing.Image img = System.Drawing.Image.FromStream(stream.AsStream());
string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)+@"\dependencies\tessdata";
try {
using(var engine = new TesseractEngine(dir, this.language.AbbreviatedName, EngineMode.Default)) {
Bitmap bmp = new System.Drawing.Bitmap(img);
Pix pix = PixConverter.ToPix(bmp);
using (var page = engine.Process(pix)) {
await Task.Delay(1);
return (page.GetText(), OctarineError.Success, null);
}
}
} catch{
return (null, OctarineError.EngineError, null);
}
}

public Language[] languages {get{
var langs = new List<Language>();
var dict = new Dictionary<string, string>();
foreach(CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
dict[ci.ThreeLetterISOLanguageName.ToLower()] = ci.TwoLetterISOLanguageName;
}
string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)+@"\dependencies\tessdata";
string[] lngs = Directory.GetFiles(dir);
foreach(string lang in lngs) {
if(!lang.EndsWith(".traineddata")) continue;
try {
string code = Path.GetFileNameWithoutExtension(lang).ToLower();
if(dict.ContainsKey(code)) {
var lng = new Language(dict[code]);
langs.Add(lng);
}
}catch{}
}
return langs.ToArray();
}}

public void SetLanguage(Language lang) {
this.language = lang;
}

public Language currentLanguage {get{
return this.language;
}}
}
}