    using System;
using System.Reflection;
using System.Text;
    using System.IO;
using System.Threading;
    using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Tesseract;
using Octarine.OctarineEngine;
using Newtonsoft.Json;

namespace Octarine.OctarineEngine {
public class TesseractOctarineEngine : IEngine {
private OctarineLanguage language;
private int quality=2;
public TesseractOctarineEngine() {
string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)+@"\Tesseract.Octarine";
TesseractEnviornment.CustomSearchPath=dir;
CultureInfo ci = Thread.CurrentThread.CurrentCulture;
this.language = new OctarineLanguage(ci.DisplayName, ci.ThreeLetterISOLanguageName, 3);
}
public string ID {get{return "Tesseract";}}
public string Name {get{return "Tesseract";}}
public int Priority {get{return 1;}}
public bool ShouldRegister {get{return true;}}

public async Task<(OCRPage, OctarineError, string)> GetTextFromStreamAsync(Stream stream) {
System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
OCRPage page = new OCRPage(img);
string dir;
switch(this.quality) {
case 1:
dir = Config.GetDataDir("tessdata_fast");
break;
case 3:
dir = Config.GetDataDir("tessdata_best");
break;
default:
dir = Config.GetDataDir("tessdata");
break;
}
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
} catch(Exception ex) {
return (null, OctarineError.EngineError, ex.Message);
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
string dir = Config.GetDataDir("tessdata");
string[] lngs = Directory.GetFiles(dir);
foreach(string lang in lngs) {
if(!lang.EndsWith(".traineddata")) continue;
try {
string code = Path.GetFileNameWithoutExtension(lang).ToLower();
if(dict.ContainsKey(code)) {
var lng = new OctarineLanguage(dict[code], code, 3);
langs.Add(lng);
}
}catch{}
}
return langs.ToArray();
}}

public void SetLanguage(OctarineLanguage lang, int quality=0) {
if(quality==-1) quality=1;
this.language = lang;
this.quality=quality;
}

public OctarineLanguage CurrentLanguage {get=>this.language;}
public int CurrentQuality {get=>this.quality;}

public bool CanEnable(bool auto=true) {return true;}

public bool CanDownloadLanguages {get=>true;}

public OctarineLanguage[] GetDownloadableLanguages() {
string standardDir = Config.GetDataDir("tessdata");
string bestDir = Config.GetDataDir("tessdata_best");
string fastDir = Config.GetDataDir("tessdata_fast");
var dict = new Dictionary<string, string>();
foreach(CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
string name;
if(ci.IsNeutralCulture) name=ci.DisplayName;
else name=ci.Parent.DisplayName;
dict[ci.ThreeLetterISOLanguageName.ToLower()] = name;
}
var langs = new List<OctarineLanguage>();
try {
using (var client = new HttpClient()) {
var url = "http://api.octarine.pl/tessdata";
var response = client.GetAsync(url).Result;
if(!response.IsSuccessStatusCode ) return null;
var json = response.Content.ReadAsStringAsync().Result;
dynamic j;
try {
j = JsonConvert.DeserializeObject(json);
} catch {return null;}
foreach(dynamic l in j) {
string code = (string)l.code;
if(!dict.ContainsKey(code)) continue;
var lng = new OctarineLanguage(dict[code], code, 3);
var lSources = new List<string>();
var lDestinations = new List<string>();
dynamic files = l.files;
if(files.ContainsKey("best")) {
lSources.Add("http://api.octarine.pl"+files.best);
lDestinations.Add(bestDir+"\\"+code+".traineddata");
}
if(files.ContainsKey("fast")) {
lSources.Add("http://api.octarine.pl"+files.fast);
lDestinations.Add(fastDir+"\\"+code+".traineddata");
}
if(files.ContainsKey("standard")) {
lSources.Add("http://api.octarine.pl"+files.standard);
lDestinations.Add(standardDir+"\\"+code+".traineddata");
}
lng.Sources=lSources.ToArray();
lng.Destinations=lDestinations.ToArray();
langs.Add(lng);
}
}
return langs.ToArray();
} catch {return null;}
}

public bool SecondaryLanguagesSupported{get => false;}

public bool AddSecondaryLanguage(OctarineLanguage lang) {return false;}
public bool ClearSecondaryLanguages() {return false;}
public OctarineLanguage[] GetSecondaryLanguages() {return null;}
public bool IsConfigurationRecommended {get => false;}
}
}