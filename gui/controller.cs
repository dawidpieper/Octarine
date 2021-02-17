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

public void SetLanguage(IEngine engine, OctarineLanguage language) {
if(engine.Languages==null) return;
engine.SetLanguage(language);
engine.WriteConfig("Language", language.Code);
wnd.RefreshResult();
}

public void SaveFile(string file, string text) {
File.WriteAllText(file, text);
}
}
}