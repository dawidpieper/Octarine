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

this.Size = new Size(600, 600);
this.Text = "Ustawienia - Octarine";
this.ShowInTaskbar=false;

lb_engine = new Label() {
Left=30,
Top=10,
Width=100,
Height=20,
Text="Silnik"
};
this.Controls.Add(lb_engine);

lst_engine = new ListBox() {
Left=30,
Top=30,
Width=200,
Height=100
};
this.Controls.Add(lst_engine);

lb_language = new Label() {
Left=30,
Top=150,
Width=100,
Height=20,
Text="Język"
};
this.Controls.Add(lb_language);

lst_language = new ListBox() {
Left=30,
Top=170,
Width=200,
Height=100
};
this.Controls.Add(lst_language);

btn_downloadLanguages = new Button() {
Left=250,
Top=170,
Width=100,
Height=20,
Text = "Pobierz pakiety językowe",
Enabled=false
};
btn_downloadLanguages.Click += (sender, e) => DownloadLanguages();
this.Controls.Add(btn_downloadLanguages);

lb_secondary = new Label() {
Left=30,
Top=290,
Width=150,
Height=20,
Text="Języki dodatkowe"
};
this.Controls.Add(lb_secondary);

clt_secondary = new CheckedListBox() {
Left=30,
Top=310,
Width=200,
Height=100
};
this.Controls.Add(clt_secondary);

btn_secondaryDeselectAll = new Button() {
Left=250,
Top=310,
Width=100,
Height=20,
Text="Odznacz wszystkie",
Enabled=false
};
btn_secondaryDeselectAll.Click += (sender, e) => SecondaryDeselectAll();
this.Controls.Add(btn_secondaryDeselectAll);

lb_quality = new Label() {
Left=30,
Top=430,
Width=100,
Height=20,
Text="Jakość"
};
this.Controls.Add(lb_quality);

lst_quality = new ListBox() {
Left=30,
Top=450,
Width=200,
Height=100
};
this.Controls.Add(lst_quality);

btn_ok = new Button() {
Left=30,
Top=570,
Width=80,
Height=20,
Text="OK"
};
this.Controls.Add(btn_ok);

btn_cancel = new Button() {
Left=120,
Top=570,
Width=80,
Height=20,
Text="Anuluj"
};
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
int desiredIndex=-1;
OctarineLanguage[] selan = null;
if(engine.SecondaryLanguagesSupported) selan = engine.GetSecondaryLanguages();
if(engine.Languages!=null)
foreach(var lang in engine.Languages) {
lst_language.Items.Add(lang.Name);
clt_secondary.Items.Add(lang.Name);
if(lang.Code == engine.CurrentLanguage.Code) desiredIndex=i;
if(selan!=null)
foreach(var l in selan)
if(l.Code==lang.Code) clt_secondary.SetItemChecked(i, true);
++i;
}
else {
lst_language.Items.Add("Rozpoznaj automatycznie");
lst_language.SelectedIndex=0;
}
lst_language.SelectedIndex=desiredIndex;
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
} else if(language.Qualities==4) {
lst_quality.Items.Add("Szybka");
lst_quality.Items.Add("Standardowa");
lst_quality.Items.Add("Najlepsza");
lst_quality.Items.Add("Ultra");
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