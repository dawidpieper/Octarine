using System.Windows.Forms;
    using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Globalization;
using Windows.Data.Pdf;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.Graphics.Imaging;

namespace Octarine {

public enum OCRFragmentType {
None,
Heading
}

public class OCRFragment {
public string Text;
public OCRFragmentType Type=OCRFragmentType.None;
public int X, Y, Width, Height;
public OCRFragment(string text, int x, int y, int width, int height, OCRFragmentType type=OCRFragmentType.None) {
Text=text;
X=x;
Y=y;
Width=width;
Type=type;
Height=height;
}
}

public class OCRPage {
public System.Drawing.Image _Source;
public List<OCRFragment> _Fragments;
public System.Drawing.Image Source {get{return _Source;}}
public OCRFragment[] Fragments {get{return _Fragments.ToArray();}}
public OCRPage(System.Drawing.Image source=null) {
_Source=source;
_Fragments = new List<OCRFragment>();
}
public void AddFragment(string text, int x, int y, int width, int height, OCRFragmentType type=OCRFragmentType.None) {
_Fragments.Add(new OCRFragment(text, x, y, width, height, type));
}

public string Text {get{
var sb = new StringBuilder();
bool s=true;
foreach(OCRFragment fragment in _Fragments) {
if(s) sb.Append("\r\n");
s=true;
sb.Append(fragment.Text);
}
return sb.ToString();
}}
}

public class OCRResult {
public string _File = null;
public List<OCRPage> _Pages;
public string File {get{return _File;}}
public OCRPage[] Pages {get{return _Pages.ToArray();}}
public OCRResult(string file=null) {
_File=file;
_Pages = new List<OCRPage>();
}

public void AddPage(OCRPage page) {
_Pages.Add(page);
}

public string Text {get{
var sb = new StringBuilder();
bool s=true;
foreach(OCRPage page in _Pages) {
if(s) sb.Append("\r\n\r\n");
s=true;
sb.Append(page.Text);
}
return sb.ToString();
}}
}

public class OCRStatus {
public uint _PageCount=1, _PageCurrent=0;
public OctarineError Error=0;
public string ErrorMessage="";
public CancellationToken OCRCancellationToken;
public bool Cancelled=false;

public int ActiveWorkers=0;

public event EventHandler PageCountChanged;
public event EventHandler PageCurrentChanged;

public uint PageCount {
get{return _PageCount;}
set{
_PageCount=value;
if(PageCountChanged!=null) PageCountChanged(this, EventArgs.Empty);
}
}

public uint PageCurrent {
get{return _PageCurrent;}
set{
_PageCurrent=value;
if(PageCurrentChanged!=null) PageCurrentChanged(this, EventArgs.Empty);
}
}

public OCRStatus(){}
}

public class OCR {
private struct InternalOCRProcessorStatus {
public OctarineEngine.IEngine Engine;
public OCRStatus Status;
public Stream Source;
public uint PageNumber;
public OCRPage[] Pages;
public bool Started;
public InternalOCRProcessorStatus(OctarineEngine.IEngine engine, OCRStatus status, Stream source, uint pageNumber, OCRPage[] pages) {
Engine=engine;
Status=status;
Source=source;
PageNumber=pageNumber;
Pages=pages;
Started=false;
}
}


public static async Task<OCRResult> GetTextFromFileAsync(string filePath, OctarineEngine.IEngine engine, OCRStatus status) {
var result = new OCRResult(filePath);
try {
var file = await StorageFile.GetFileFromPathAsync(filePath);
if(Path.GetExtension(filePath).ToLower()==".pdf") {
var pdfDoc = await PdfDocument.LoadFromFileAsync(file);
status.PageCount = pdfDoc.PageCount;
Stream[] streams = new Stream[status.PageCount];
int pagesRunning = 0;
int numThreads = Environment.ProcessorCount;
for (uint p = 0; p < status.PageCount; p++) {
while(pagesRunning>numThreads) await Task.Delay(10);
using (PdfPage pdfPage = pdfDoc.GetPage(p)) {
int i=(int)p;
_ = Task.Run(async () => {
++pagesRunning;
var stream = new InMemoryRandomAccessStream();
await pdfPage.RenderToStreamAsync(stream);
streams[i] = stream.AsStream();
--pagesRunning;
});
}
await Task.Delay(10);
}
while(pagesRunning>0) await Task.Delay(200);
OCRPage[] pages = new OCRPage[status.PageCount];
for (uint i = 0; i < status.PageCount; i++) {
if(status.Error!=OctarineError.Success) break;
while(status.ActiveWorkers>numThreads) await Task.Delay(200);
var st = new InternalOCRProcessorStatus(engine, status, streams[i], i, pages);
//new Thread(new ParameterizedThreadStart(ProcessWithPage)).Start(st);
_ = Task.Run(() => ProcessWithPageAsync(st));
await Task.Delay(250);
}
if(status.Error!=OctarineError.Success) return null;
while(status.ActiveWorkers>0) await Task.Delay(200);
if(status.Error!=OctarineError.Success) return null;
foreach(OCRPage page in pages) result.AddPage(page);
return result;
} else {
var stream = await file.OpenAsync(FileAccessMode.Read);
(OCRPage page, OctarineError error, string errorMessage) = await engine.GetTextFromStreamAsync(stream.AsStream());
if(page==null) {
status.Error=error;
status.ErrorMessage=errorMessage;
return null;
}
if(status.OCRCancellationToken.IsCancellationRequested) {
status.Error=OctarineError.CancellationRequested;
return null;
}
result.AddPage(page);
return result;
}
} catch(Exception ex) {
status.Error=OctarineError.WrongFileFormat;
status.ErrorMessage=ex.Message;
return null;
}
}

public static async Task<OCRResult> GetTextFromStreamAsync(Stream stream, OctarineEngine.IEngine engine, OCRStatus status) {
var result = new OCRResult();
try {
(OCRPage page, OctarineError error, string errorMessage) = await engine.GetTextFromStreamAsync(stream);
if(page==null) {
status.Error=error;
status.ErrorMessage=errorMessage;
return null;
}
if(status.OCRCancellationToken.IsCancellationRequested) {
status.Error=OctarineError.CancellationRequested;
return null;
}
result.AddPage(page);
return result;
} catch(Exception ex) {
status.Error=OctarineError.WrongFileFormat;
status.ErrorMessage=ex.Message;
return null;
}
}

public static async Task<OCRResult> GetTextFromStreamsAsync(Stream[] streams, OctarineEngine.IEngine engine, OCRStatus status) {
var result = new OCRResult();
try {
foreach(Stream stream in streams) {
(OCRPage page, OctarineError error, string errorMessage) = await engine.GetTextFromStreamAsync(stream);
if(page==null) {
status.Error=error;
status.ErrorMessage=errorMessage;
return null;
}
if(status.OCRCancellationToken.IsCancellationRequested) {
status.Error=OctarineError.CancellationRequested;
return null;
}
result.AddPage(page);
}
return result;
} catch(Exception ex) {
status.Error=OctarineError.WrongFileFormat;
status.ErrorMessage=ex.Message;
return null;
}
}

private static void ProcessWithPage(object st) {
OCRPage p = ProcessWithPageAsync((InternalOCRProcessorStatus)st).Result;
return;
}

private static async Task<OCRPage> ProcessWithPageAsync(InternalOCRProcessorStatus st) {
st.Started=true;
if(st.Status.Error!=OctarineError.Success) return null;
++st.Status.ActiveWorkers;
if(st.Status.OCRCancellationToken.IsCancellationRequested) {
st.Status.Error=OctarineError.CancellationRequested;
--st.Status.ActiveWorkers;
return null;
}
(OCRPage page, OctarineError error, string errorMessage) = await st.Engine.GetTextFromStreamAsync(st.Source);
if(page==null) {
st.Status.Error=error;
st.Status.ErrorMessage=errorMessage;
--st.Status.ActiveWorkers;
return null;
}
++st.Status.PageCurrent;
st.Pages[st.PageNumber] = page;
--st.Status.ActiveWorkers;
return page;
}
}
}