using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using Octarine.OctarineEngine;

namespace Octarine {
public class OctarineSettingsWindow : OctarineWindowBase {
private List<OctarineEngine.IEngine> engines;

private Label lb_engine, lb_language, lb_secondary, lb_quality;
private ListBox lst_engine, lst_language, lst_quality;
CheckedListBox clt_secondary;
private Button btn_downloadLanguages, btn_secondaryDeselectAll, btn_ok, btn_cancel;
private OctarineController controller;


public OctarineSettingsWindow(OctarineController tcontroller) {
controller=tcontroller;

engines = new List<OctarineEngine.IEngine>();
foreach(OctarineEngine.IEngine engine in OctarineEngines.engines)
engines.Add(engine);

this.Size = new Size(320, 600);
this.Text = "Ustawienia - Octarine";
this.ShowInTaskbar=false;

lb_engine = new Label();
lb_engine.Size = new Size(100, 100);
lb_engine.Location = new Point(20, 20);
lb_engine.Text="Silnik";
this.Controls.Add(lb_engine);
lst_engine = new ListBox();
lst_engine.Size = new Size(180, 100);
lst_engine.Location = new Point(120, 20);
this.Controls.Add(lst_engine);

lb_language = new Label();
lb_language.Size = new Size(100, 100);
lb_language.Location = new Point(20, 140);
lb_language.Text="Język";
this.Controls.Add(lb_language);
lst_language = new ListBox();
lst_language.Size = new Size(100, 100);
lst_language.Location = new Point(120, 140);
this.Controls.Add(lst_language);
btn_downloadLanguages = new Button();
btn_downloadLanguages.Text = "Pobierz pakiety językowe";
btn_downloadLanguages.Size = new Size(80, 100);
btn_downloadLanguages.Location = new Point(200, 140);
btn_downloadLanguages.Enabled=false;
btn_downloadLanguages.Click += (sender, e) => DownloadLanguages();
this.Controls.Add(btn_downloadLanguages);

lb_secondary = new Label();
lb_secondary.Size = new Size(100, 100);
lb_secondary.Location = new Point(20, 260);
lb_secondary.Text="Języki dodatkowe";
this.Controls.Add(lb_secondary);
clt_secondary = new CheckedListBox();
clt_secondary.Size = new Size(100, 100);
clt_secondary.Location = new Point(120, 260);
this.Controls.Add(clt_secondary);
btn_secondaryDeselectAll = new Button();
btn_secondaryDeselectAll.Text = "Odznacz wszystkie";
btn_secondaryDeselectAll.Size = new Size(80, 100);
btn_secondaryDeselectAll.Location = new Point(200, 260);
btn_secondaryDeselectAll.Enabled=false;
btn_secondaryDeselectAll.Click += (sender, e) => SecondaryDeselectAll();
this.Controls.Add(btn_secondaryDeselectAll);

lb_quality = new Label();
lb_quality.Size = new Size(100, 100);
lb_quality.Location = new Point(20, 380);
lb_quality.Text="Jakość";
this.Controls.Add(lb_quality);
lst_quality = new ListBox();
lst_quality.Size = new Size(100, 100);
lst_quality.Location = new Point(120, 380);
this.Controls.Add(lst_quality);

btn_ok = new Button();
btn_ok.Text = "OK";
btn_ok.Size = new Size(50, 50);
btn_ok.Location = new Point(20, 530);
this.Controls.Add(btn_ok);

btn_cancel = new Button();
btn_cancel.Text = "Anuluj";
btn_cancel.Size = new Size(50, 50);
btn_cancel.Location = new Point(250, 530);
this.Controls.Add(btn_cancel);

this.CancelButton = btn_cancel;
this.AcceptButton = btn_ok;

lst_engine.SelectedIndexChanged += (sender, e) => ShowLanguagesForEngine(engines[lst_engine.SelectedIndex]);
if(engines.Count()>0) {
foreach(OctarineEngine.IEngine engine in engines) {
lst_engine.Items.Add(engine.Name);
if(engine==controller.Engine) lst_engine.SelectedIndex=lst_engine.Items.Count-1;
}
}

lst_language.SelectedIndexChanged += (sender, e) => {
var engine = engines[lst_engine.SelectedIndex];
if(engine.Languages==null)
ShowQualitiesForLanguage(engine, null);
else
ShowQualitiesForLanguage(engine, engine.Languages[lst_language.SelectedIndex]);
for(int i=0; i<engine.Languages.Count(); ++i)
if(clt_secondary.GetItemCheckState(i) == CheckState.Indeterminate) clt_secondary.SetItemCheckState(i, CheckState.Unchecked);
clt_secondary.SetItemCheckState(lst_language.SelectedIndex,CheckState.Indeterminate);
};

clt_secondary.ItemCheck += (sender, e) => {
if(lst_language.SelectedIndex>=0 && e.Index==lst_language.SelectedIndex && e.NewValue!=CheckState.Indeterminate)
e.NewValue=CheckState.Indeterminate;
};

btn_cancel.Click += (sender, e) => this.Close();
btn_ok.Click += (sender, e) => {
if(engines.Count()>0) {
var engine = engines[lst_engine.SelectedIndex];
if(lst_language.SelectedIndex==-1) {
MessageBox.Show(this, "Nie wybrano języka.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
return;
}
if(!engine.CanEnable(false)) return;
controller.Engine=engine;
int q=0;
if(lst_quality.Items.Count>1) q=lst_quality.SelectedIndex+1;
OctarineLanguage[] sl = null;
if(engine.SecondaryLanguagesSupported) {
var slt = new List<OctarineLanguage>();
for(int i=0; i<engine.Languages.Count(); ++i)
if(clt_secondary.GetItemCheckState(i)==CheckState.Checked) slt.Add(engine.Languages[i]);
sl=slt.ToArray();
}
if(engine.Languages!=null) controller.SetLanguage(engine, engine.Languages[lst_language.SelectedIndex], sl, q);
this.Close();
}
};
}

private void ShowLanguagesForEngine(OctarineEngine.IEngine engine) {
lst_language.Items.Clear();
clt_secondary.Items.Clear();
lst_language.SelectedIndex=-1;
int i=0;
OctarineLanguage[] selan = null;
if(engine.SecondaryLanguagesSupported) selan = engine.GetSecondaryLanguages();
if(engine.Languages!=null)
foreach(var lang in engine.Languages) {
lst_language.Items.Add(lang.Name);
clt_secondary.Items.Add(lang.Name);
if(lang.Code == engine.CurrentLanguage.Code) lst_language.SelectedIndex=i;
if(selan!=null)
foreach(var l in selan)
if(l.Code==lang.Code) clt_secondary.SetItemChecked(i, true);
++i;
}
else {
lst_language.Items.Add("Rozpoznaj automatycznie");
lst_language.SelectedIndex=0;
}
if(lst_language.SelectedIndex>=0 && engine.Languages!=null) {
ShowQualitiesForLanguage(engine, engine.Languages[lst_language.SelectedIndex]);
clt_secondary.SetItemCheckState(lst_language.SelectedIndex, CheckState.Indeterminate);
}
else
ShowQualitiesForLanguage(engine, null);
btn_downloadLanguages.Enabled = engine.CanDownloadLanguages;
}

private void ShowQualitiesForLanguage(IEngine engine, OctarineLanguage language) {
if(language==null) {
lst_quality.Items.Clear();
lst_quality.Enabled=false;
return;
}
lst_quality.Items.Clear();
if(language.Qualities==1) {
lst_quality.Items.Add("Domyślna");
} else if(language.Qualities==2) {
lst_quality.Items.Add("Niska");
lst_quality.Items.Add("Wysoka");
lst_quality.SelectedIndex=1;
} else if(language.Qualities==3) {
lst_quality.Items.Add("Szybka");
lst_quality.Items.Add("Standardowa");
lst_quality.Items.Add("Najlepsza");
lst_quality.SelectedIndex=1;
} else {
for(int i=0; i<language.Qualities; ++i)
lst_quality.Items.Add((i+1).ToString());
lst_quality.SelectedIndex = language.Qualities/2;
}
lst_quality.Enabled=language.Qualities>1;
if(engine.CurrentQuality>0 && engine.CurrentQuality<=lst_quality.Items.Count)
lst_quality.SelectedIndex=engine.CurrentQuality-1;
clt_secondary.Enabled = btn_secondaryDeselectAll.Enabled = engine.SecondaryLanguagesSupported;
}

private void DownloadLanguages() {
if(lst_engine.SelectedIndex==-1 || engines.Count()==0) return;
var engine = engines[lst_engine.SelectedIndex];
OctarineLanguage[] languages = engine.GetDownloadableLanguages();
if(languages==null) {
MessageBox.Show(this, "Nie udało się pobrać informacji o dostępnych językach.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
return;
}
var wnd = new DownloaderWindow(controller);
foreach(OctarineLanguage language in languages) {
wnd.AddItem(new OctarineDownloadItem(language.Name, language.Sources, language.Destinations));
}
wnd.ShowDialog(this);
ShowLanguagesForEngine(engine);
}

private void SecondaryDeselectAll() {
for(int i=0; i<clt_secondary.Items.Count; ++i)
if(clt_secondary.GetItemCheckState(i)==CheckState.Checked) clt_secondary.SetItemCheckState(i, CheckState.Unchecked);
}
}
}