using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octarine {
public class OctarineWindowBase : Form {}

public class OctarineWindow : OctarineWindowBase {
private RichTextBox edt_result;
public readonly OctarineController controller;
private string file=null;

private static List<ToolStripMenuItem> additionalMenus = new List<ToolStripMenuItem>();

private static OctarineWindow _CurrentWindow;
public static OctarineWindow CurrentWindow {get{return _CurrentWindow;}}

public OctarineWindow(OctarineController tcontroller) {
controller=tcontroller;
controller.SetWindow(this);
this.Shown += (sender, e) => {
_CurrentWindow=this;
controller.Initiate();
};

this.Size = new Size(800,600);
this.Text = "Octarine";

edt_result = new RichTextBox();
edt_result.Size = new Size(760, 560);
edt_result.Location = new Point(20, 20);
edt_result.ReadOnly = true;
edt_result.Multiline = true;
this.Controls.Add(edt_result);

MenuStrip ms = new MenuStrip();
ms.Parent = this;
ToolStripMenuItem mb_file = new ToolStripMenuItem("&Plik");
ToolStripMenuItem mi_open = new ToolStripMenuItem("&Otwórz", null,
new EventHandler((sender, e) => {BrowseFile();}));
mi_open.ShortcutKeys = Keys.Control | Keys.O;
mb_file.DropDownItems.Add(mi_open);
ToolStripMenuItem mi_save = new ToolStripMenuItem("Zapi&sz", null,
new EventHandler((sender, e) => {SaveFile();}));
mi_save.ShortcutKeys = Keys.Control | Keys.S;
mb_file.DropDownItems.Add(mi_save);
ToolStripMenuItem mi_exit = new ToolStripMenuItem("Za&kończ", null,
new EventHandler((sender, e) => {this.Close();}));
mb_file.DropDownItems.Add(mi_exit);
ms.Items.Add(mb_file);
ToolStripMenuItem mb_tools = new ToolStripMenuItem("&Narzędzia");
ToolStripMenuItem mi_settings = new ToolStripMenuItem("Wybór &silnika", null,
new EventHandler((sender, e) => {
OctarineSettingsWindow wnd_settings = new OctarineSettingsWindow(this.controller);
wnd_settings.ShowDialog(this);
}));
mb_tools.DropDownItems.Add(mi_settings);
ms.Items.Add(mb_tools);
foreach(ToolStripMenuItem mi in additionalMenus) {
ms.Items.Add(mi);
}
MainMenuStrip = ms;

}


private void BrowseFile() {
var dialog = new OpenFileDialog();
dialog.Filter = "Wszystkie obsługiwane pliki|*.bmp;*.jpg;*.png;*.gif;*.pdf|Pliki zdjęć|*.bmp;*.jpg;*.png;*.gif|Pliki pdf|*.pdf";
dialog.RestoreDirectory=true;
if(dialog.ShowDialog()==DialogResult.OK)
controller.PrepareOCR(dialog.FileName);
dialog.Dispose();
}

private void SaveFile() {
if(this.file==null) return;
var dialog = new SaveFileDialog();
dialog.Filter = "Pliki tekstowe|*.txt|Wszystkie pliki|*.*";
dialog.RestoreDirectory=true;
if(dialog.ShowDialog()==DialogResult.OK)
controller.SaveFile(dialog.FileName, edt_result.Text);
dialog.Dispose();
}

public void SetResult(string file, OCRResult result) {
this.Text = Path.GetFileName(file)+" - Octarine";
this.file=file;
edt_result.Text=result.Text;
}

public void RefreshResult() {
if(this.file!=null) controller.PrepareOCR(this.file);
}

public void ShowError(OctarineError error, string msg=null) {
string errormessage = "Wystąpił nierozpoznany błąd";
switch(error) {
case OctarineError.LanguageNotSupported:
errormessage = "Wybrany język nie jest już obsługiwany.\nUstaw poprawny język.";
break;
case OctarineError.WrongFileFormat:
errormessage = "Nierozpoznany format pliku.";
break;
case OctarineError.EngineError:
errormessage = "Błąd wewnętrzny silnika OCR.";
break;
}
if(msg!=null) errormessage+="\n"+msg;
MessageBox.Show(errormessage, "Wystąpił błąd podczas próby otwarcia pliku", MessageBoxButtons.OK, MessageBoxIcon.Error);
}

public static void RegisterAdditionalMenu(ToolStripMenuItem mi) {
additionalMenus.Add(mi);
}
}
}