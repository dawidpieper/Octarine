using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using Octarine.OctarineEngine;

namespace Octarine {
public class OctarineSettingsWindow : Form {
private List<OctarineEngine.IEngine> engines;

private Label lb_engine, lb_language;
private ListBox lst_engine, lst_language;
private Button btn_ok, btn_cancel;
private OctarineController controller;


public OctarineSettingsWindow(OctarineController tcontroller) {
controller=tcontroller;

engines = new List<OctarineEngine.IEngine>();
foreach(OctarineEngine.IEngine engine in OctarineEngines.engines)
engines.Add(engine);

this.Size = new Size(320,320);
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
lst_language.Size = new Size(180, 100);
lst_language.Location = new Point(120, 140);
this.Controls.Add(lst_language);

btn_ok = new Button();
btn_ok.Text = "OK";
btn_ok.Size = new Size(50, 50);
btn_ok.Location = new Point(20, 250);
this.Controls.Add(btn_ok);

btn_cancel = new Button();
btn_cancel.Text = "Anuluj";
btn_cancel.Size = new Size(50, 50);
btn_cancel.Location = new Point(250, 250);
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
if(engine.Languages!=null) controller.SetLanguage(engine, engine.Languages[lst_language.SelectedIndex]);
this.Close();
}
};
}

private void ShowLanguagesForEngine(OctarineEngine.IEngine engine) {
lst_language.Items.Clear();
lst_language.SelectedIndex=-1;
int i=0;
if(engine.Languages!=null)
foreach(var lang in engine.Languages) {
lst_language.Items.Add(lang.Name);
if(lang.Code == engine.currentLanguage.Code) lst_language.SelectedIndex=i;
++i;
}
else {
lst_language.Items.Add("Rozpoznaj automatycznie");
lst_language.SelectedIndex=0;
}
}
}
}