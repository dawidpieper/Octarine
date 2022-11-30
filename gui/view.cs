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
private OCRResult Result = null;
private SearchWindow CurrentSearchWindow = null;

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
ToolStripMenuItem mi_export = new ToolStripMenuItem("&Eksportuj");
mb_file.DropDownItems.Add(mi_export);
ToolStripMenuItem mi_save = new ToolStripMenuItem("Zapi&sz jako plik tekstowy", null,
new EventHandler((sender, e) => {SaveFile();}));
mi_save.ShortcutKeys = Keys.Control | Keys.S;
mi_export.DropDownItems.Add(mi_save);
ToolStripMenuItem mi_pdf = new ToolStripMenuItem("Zapisz jako &pdf", null,
new EventHandler((sender, e) => {SavePDF();}));
mi_pdf.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
mi_export.DropDownItems.Add(mi_pdf);
ToolStripMenuItem mi_exit = new ToolStripMenuItem("Za&kończ", null,
new EventHandler((sender, e) => {this.Close();}));
mb_file.DropDownItems.Add(mi_exit);
ms.Items.Add(mb_file);
ToolStripMenuItem mb_edit = new ToolStripMenuItem("&Edycja");
ToolStripMenuItem mi_find = new ToolStripMenuItem("&Znajdź", null,
new EventHandler((sender, e) => {ShowFindDialog();}));
mi_find.ShortcutKeys = Keys.Control | Keys.F;
mb_edit.DropDownItems.Add(mi_find);
ToolStripMenuItem mi_findNext = new ToolStripMenuItem("Znajdź &następny", null,
new EventHandler((sender, e) => {FindNext();}));
mi_findNext.ShortcutKeys = Keys.F3;
mb_edit.DropDownItems.Add(mi_findNext);
ms.Items.Add(mb_edit);
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

private void SavePDF() {
if(this.file==null) return;
var dialog = new SaveFileDialog();
dialog.Filter = "Plik pdf zawierający oryginalny obraz i przetworzony tekst|*.pdf|Plik pdf zawierający tylko przetworzony tekst|*.pdf|Plik pdf zawierający tylko oryginalny obraz|*.pdf";
dialog.RestoreDirectory=true;
dialog.FilterIndex=1;
if(dialog.ShowDialog()==DialogResult.OK)
controller.SavePDF(dialog.FileName, Result, dialog.FilterIndex==1||dialog.FilterIndex==2, dialog.FilterIndex==1||dialog.FilterIndex==3);
dialog.Dispose();
}

public void SetResult(string file, OCRResult result) {
this.Text = Path.GetFileName(file)+" - Octarine";
this.file=file;
this.Result=result;
edt_result.Text=result.Text;
}

public void RefreshResult() {
if(this.file!=null) controller.PrepareOCR(this.file);
}

public void ShowFindDialog() {
if(CurrentSearchWindow==null) {
CurrentSearchWindow = new SearchWindow();
CurrentSearchWindow.FindNext += (sender, e) => {
FindNext(CurrentSearchWindow.CurrentSearchPhrase);
};
}
CurrentSearchWindow.ShowDialog(this);
}

public void FindNext(string searchText=null) {
if(searchText==null || searchText=="") {
if(CurrentSearchWindow==null || (searchText==null && CurrentSearchWindow.CurrentSearchPhrase=="")) {
ShowFindDialog();
return;
}
else searchText = CurrentSearchWindow.CurrentSearchPhrase;
}
if(searchText!="") {
try {
var fp = edt_result.Find(searchText, edt_result.SelectionStart+1, RichTextBoxFinds.None);
edt_result.Select(fp, searchText.Length);
}
catch {
MessageBox.Show("Nie znaleziono wskazanego tekstu", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
}
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