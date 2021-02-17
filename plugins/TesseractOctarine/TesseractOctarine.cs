    using System;
using System.Reflection;
using System.Text;
    using System.IO;
using System.Threading;
    using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Tesseract;
using Octarine.OctarineEngine;

namespace Octarine.OctarineEngine {
public class TesseractOctarineEngine : IEngine {
private OctarineLanguage language;
public TesseractOctarineEngine() {
string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)+@"\Tesseract.Octarine";
TesseractEnviornment.CustomSearchPath=dir;
CultureInfo ci = Thread.CurrentThread.CurrentCulture;
this.language = new OctarineLanguage(ci.DisplayName, ci.ThreeLetterISOLanguageName);
}
public string ID {get{return "Tesseract";}}
public string Name {get{return "Tesseract";}}
public bool ShouldRegister {get{return true;}}

public async Task<(OCRPage, OctarineError, string)> GetTextFromStreamAsync(Stream stream) {
System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
OCRPage page = new OCRPage(img);
string dir = this.GetDataDir();
try {
using(var engine = new TesseractEngine(dir, this.language.Code, EngineMode.Default)) {
Bitmap bmp = new System.Drawing.Bitmap(img);
Pix pix = PixConverter.ToPix(bmp);
using (var oPage = engine.Process(pix)) {
await Task.Delay(1);
page.AddFragment(oPage.GetText(), oPage.RegionOfInterest.X1, oPage.RegionOfInterest.Y1, oPage.RegionOfInterest.Width, oPage.RegionOfInterest.Height);
return (page, OctarineError.Success, null);
}
}
} catch{
return (null, OctarineError.EngineError, null);
}
}

public OctarineLanguage[] Languages {get{
var langs = new List<OctarineLanguage>();
var dict = new Dictionary<string, string>();
foreach(CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
string name;
if(ci.IsNeutralCulture) name=ci.DisplayName;
else name=ci.Parent.DisplayName;
dict[ci.ThreeLetterISOLanguageName.ToLower()] = name;
}
string dir = this.GetDataDir("tessdata");
string[] lngs = Directory.GetFiles(dir);
foreach(string lang in lngs) {
if(!lang.EndsWith(".traineddata")) continue;
try {
string code = Path.GetFileNameWithoutExtension(lang).ToLower();
if(dict.ContainsKey(code)) {
var lng = new OctarineLanguage(dict[code], code);
langs.Add(lng);
}
}catch{}
}
return langs.ToArray();
}}

public void SetLanguage(OctarineLanguage lang) {
this.language = lang;
}

public OctarineLanguage currentLanguage {get{
return this.language;
}}

public bool CanEnable(bool auto=true) {return true;}
}
}