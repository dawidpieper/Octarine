using System;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Windows.Globalization;
using Windows.Storage.Streams;
using Octarine.OctarineEngine;

namespace Octarine {
public class OctarineController {

private IEngine _Engine=null;

public IEngine Engine {
get {
if(_Engine!=null) return _Engine;
string eng = Config.ReadConfig("Engine");
IEngine[] engines = OctarineEngines.engines;
if(engines.Count()==0) return null;
if(eng!=null)
foreach(IEngine engine in engines) {
if(engine.ID==eng && engine.CanEnable()) return engine;
}
foreach(IEngine engine in engines)
if(engine.CanEnable()) return engine;
return null;
}
set {
_Engine=value;
Config.WriteConfig("Engine", _Engine.ID);
}
}

private OctarineWindow wnd=null;

private Task updateWorker = null;
private CancellationTokenSource updateWorkerCTS = null;
private CancellationToken updateWorkerCT;

public OctarineController() {
}

public void SetWindow(OctarineWindow twnd) {
wnd=twnd;
}

public void Initiate() {

}

public void PrepareOCR(string file) {
if(updateWorker!=null && !updateWorker.IsCompleted) {
updateWorkerCTS.Cancel();
try {
updateWorker.Wait();
} catch{}
updateWorker=null;
}
int pageFirst=0, pageLast=0;

if(Path.GetExtension(file).ToLower()==".pdf") {
int cnt = (int)OCR.GetPDFPagesCount(file);
if(cnt>1) {
var wnd_range = new PageRangeWindow(cnt);
wnd_range.ShowDialog(wnd);
if(wnd_range.Cancelled) return;
pageFirst=wnd_range.PageFirst;
pageLast=wnd_range.PageLast;
}
}
LoadingWindow wnd_waiter = new LoadingWindow("Rozpoznawanie");
updateWorkerCTS = new CancellationTokenSource();
updateWorkerCT = updateWorkerCTS.Token;
updateWorker = Task.Factory.StartNew(() => {
wnd_waiter.SetStatus("Przygotowywanie");

OCRStatus status = new OCRStatus();
status.OCRCancellationToken = updateWorkerCT;
status.PageCurrentChanged += (sender, e) => {
wnd_waiter.SetStatus($"Rozpoznawanie strony {status.PageCurrent} z {status.PageCount}");
int prc=(int)(100*status.PageCurrent/status.PageCount);
if(prc>100) prc=100;
wnd_waiter.SetPercentage(prc);
};
OCRResult result = OCR.GetTextFromFileAsync(file, this.Engine, status, pageFirst, pageLast).Result;
if(result!=null)
wnd.SetResult(file, result);
else
if(status.Error!=OctarineError.CancellationRequested) wnd.ShowError(status.Error, status.ErrorMessage);
wnd_waiter.Close();
updateWorker=null;
}, updateWorkerCT);
wnd_waiter.ShowDialog(wnd);
if(updateWorker!=null && !updateWorker.IsCompleted) {
updateWorkerCTS.Cancel();
try {
updateWorker.Wait();
} catch{}
updateWorker=null;
}
}

public void PrepareOCR(Stream stream, string name="Plik") {
Stream[] streams = new Stream[1];
streams[0]=stream;
PrepareOCR(streams, name);
}

public void PrepareOCR(Stream[] streams, string name="Plik") {
if(updateWorker!=null && !updateWorker.IsCompleted) {
updateWorkerCTS.Cancel();
try {
updateWorker.Wait();
} catch{}
updateWorker=null;
}
LoadingWindow wnd_waiter = new LoadingWindow("Rozpoznawanie");
updateWorkerCTS = new CancellationTokenSource();
updateWorkerCT = updateWorkerCTS.Token;
updateWorker = Task.Factory.StartNew(() => {
wnd_waiter.SetStatus("Przygotowywanie");
OCRStatus status = new OCRStatus();
status.OCRCancellationToken = updateWorkerCT;
status.PageCurrentChanged += (sender, e) => {
wnd_waiter.SetStatus($"Rozpoznawanie strony {status.PageCurrent} z {status.PageCount}");
wnd_waiter.SetPercentage((int)(100*status.PageCurrent/status.PageCount));
};
OCRResult result = OCR.GetTextFromStreamsAsync(streams, this.Engine, status).Result;
if(result!=null)
wnd.SetResult(name, result);
else
if(status.Error!=OctarineError.CancellationRequested) wnd.ShowError(status.Error, status.ErrorMessage);
wnd_waiter.Close();
updateWorker=null;
}, updateWorkerCT);
wnd_waiter.ShowDialog(wnd);
if(updateWorker!=null && !updateWorker.IsCompleted) {
updateWorkerCTS.Cancel();
try {
updateWorker.Wait();
} catch{}
updateWorker=null;
}
}

public void SetLanguage(IEngine engine, OctarineLanguage language, OctarineLanguage[] secondaryLanguages=null, int quality=0) {
if(engine.Languages==null) return;
engine.SetLanguage(language, quality);
if(engine.SecondaryLanguagesSupported) {
engine.ClearSecondaryLanguages();
if(secondaryLanguages!=null)
foreach(var l in secondaryLanguages)
engine.AddSecondaryLanguage(l);
}
engine.WriteConfig("Language", language.Code);
if(quality>0) engine.WriteConfig("Quality", quality);
if(engine.SecondaryLanguagesSupported) {
string sl = "";
var selan = secondaryLanguages;
if(selan.Count()>0)
foreach(OctarineLanguage l in selan) {
if(sl!="") sl+=",";
sl+=l.Code;
}
engine.WriteConfig("SecondaryLanguages", sl);
}
wnd.RefreshResult();
}

public void SaveFile(string file, string text) {
File.WriteAllText(file, text);
}

public bool OpenDownloader(string[] sources, string[] destinations, Form window=null, string label="Pobieranie") {
if(window==null) window=wnd;
bool cancellationProcessed=false;
var l = new LoadingWindow(label);
l.SetStatus("Inicjowanie...");
bool cancelled = true;
var cts = new CancellationTokenSource();
var ct = cts.Token;
Task.Factory.StartNew(() => {
int i=0;
using (var client = new WebClient ()) {
client.DownloadProgressChanged += (sender, e) => {
int percentage = i*100;
percentage += e.ProgressPercentage;
percentage = percentage/sources.Count();
l.SetPercentage(percentage);
l.SetStatus($"Pobieranie pliku {i+1} z {sources.Count()}: {e.BytesReceived/1048576} / {e.TotalBytesToReceive/1048576} MB");
if(ct.IsCancellationRequested) {
client.CancelAsync();
cancellationProcessed=true;
}
};
client.DownloadFileCompleted += (sender, e) => {
i++;
if(i<sources.Count())
client.DownloadFileAsync(new Uri(sources[i]), destinations[i]);
else {
cancelled=false;
l.Close();
}
};
client.DownloadFileAsync(new Uri(sources[i]), destinations[i]);
}
}, ct);
l.ShowDialog(window);
if(cancelled) {
cts.Cancel();
while(cancellationProcessed==false) Thread.Sleep(100);
for(int i=0; i<destinations.Count(); ++i) {
try {
if(File.Exists(destinations[i]))
File.Delete(destinations[i]);
} catch{}
}
}
return !cancelled;
}
}
}