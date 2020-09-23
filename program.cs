using System;
using System.Windows.Forms;

namespace Octarine {
public class Program {
[STAThread]
public static void Main() {

OctarineEngines.RegisterEngine(new OctarineEngine.Win10Engine());
Application.EnableVisualStyles();

var wnd = new OctarineWindow(new OctarineController());
Application.Run(wnd);
}
}
}