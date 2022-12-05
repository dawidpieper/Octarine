using System;
using System.Windows.Forms;
using System.Drawing;

namespace Octarine {
public class SearchWindow : Form {
public event EventHandler FindNext;
private Label lb_input;
private TextBox edt_input;
private Button btn_search, btn_close;
public SearchWindow() {
this.ShowInTaskbar=false;
this.Size = new Size(300, 150);
this.Text = "Znajdź";

lb_input = new Label() {
Left=30,
Top=10,
Width=100,
Height=20,
Text = "Znajdź"
};
this.Controls.Add(lb_input);

edt_input = new TextBox() {
Left=30,
Top=30,
Width=200,
Height=20
};
this.Controls.Add(edt_input);

btn_search = new Button() {
Left=30,
Top=60,
Width=80,
Height=20,
Text = "Znajdź następny"
};
this.Controls.Add(btn_search);

btn_close = new Button() {
Left=120,
Top=60,
Width=80,
Height=20,
Text="Anuluj"
};
this.Controls.Add(btn_close);

btn_close.Click += (sender, e) => this.Close();
btn_search.Click += (sender, e) => FindNext.Invoke(this, EventArgs.Empty);

this.CancelButton = btn_close;
this.AcceptButton = btn_search;
}
public string CurrentSearchPhrase {get => edt_input.Text;}
}
}