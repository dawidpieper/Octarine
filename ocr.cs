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
public class OCRStatus {
public uint _PageCount=1, _PageCurrent=0;
public OctarineError Error=0;
public string ErrorMessage="";
public CancellationToken OCRCancellationToken;

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
public static async Task<string> GetTextFromFileAsync(string filePath, OctarineEngine.IEngine engine, OCRStatus status) {
try {
var file = await StorageFile.GetFileFromPathAsync(filePath);
if(Path.GetExtension(filePath).ToLower()==".pdf") {
var pdfDoc = await PdfDocument.LoadFromFileAsync(file);
status.PageCount = pdfDoc.PageCount;
var sb = new StringBuilder();
for (uint i = 0; i < status.PageCount; i++) {
if(status.OCRCancellationToken.IsCancellationRequested) {
status.Error=OctarineError.CancellationRequested;
return null;
}
status.PageCurrent=i+1;
using (PdfPage page = pdfDoc.GetPage(i)) {
var stream = new InMemoryRandomAccessStream();
await page.RenderToStreamAsync(stream);
(string result, OctarineError error, string errorMessage) = await engine.GetTextFromStreamAsync(stream.AsStream());
if(result==null) {
status.Error=error;
status.ErrorMessage=errorMessage;
return null;
}
if(i>0) sb.Append("\r\n");
sb.Append(result);
}
}
return sb.ToString();
} else {
var stream = await file.OpenAsync(FileAccessMode.Read);
(string result, OctarineError error, string errorMessage) = await engine.GetTextFromStreamAsync(stream.AsStream());
if(result==null) {
status.Error=error;
status.ErrorMessage=errorMessage;
return null;
}
if(status.OCRCancellationToken.IsCancellationRequested) {
status.Error=OctarineError.CancellationRequested;
return null;
}
return result;
}
} catch(Exception ex) {
status.Error=OctarineError.WrongFileFormat;
status.ErrorMessage=ex.Message;
return null;
}
}

public static async Task<string> GetTextFromStreamAsync(Stream stream, OctarineEngine.IEngine engine, OCRStatus status) {
try {
(string result, OctarineError error, string errorMessage) = await engine.GetTextFromStreamAsync(stream);
if(result==null) {
status.Error=error;
status.ErrorMessage=errorMessage;
return null;
}
if(status.OCRCancellationToken.IsCancellationRequested) {
status.Error=OctarineError.CancellationRequested;
return null;
}
return result;
} catch(Exception ex) {
status.Error=OctarineError.WrongFileFormat;
status.ErrorMessage=ex.Message;
return null;
}
}

}
}