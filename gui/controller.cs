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

namespace Octarine {
public class OctarineController {

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

public void PrepareOCR(string file, OctarineEngine.Engine engine) {
if(updateWorker!=null && !updateWorker.IsCompleted) {
updateWorkerCTS.Cancel();
try {
updateWorker.Wait();
} catch{}
updateWorker=null;
}
updateWorkerCTS = new CancellationTokenSource();
updateWorkerCT = updateWorkerCTS.Token;
updateWorker = Task.Factory.StartNew(() => {

(string result, OctarineError error) = engine.GetTextFromFileAsync(file).Result;
if(result!=null)
wnd.SetResult(file, result);
else
wnd.ShowError(error);
updateWorker=null;
}, updateWorkerCT);
}

public void SetLanguage(OctarineEngine.Engine engine, Language language) {
engine.SetLanguage(language);
wnd.RefreshResult();
}

public void SaveFile(string file, string text) {
File.WriteAllText(file, text);
}
}
}