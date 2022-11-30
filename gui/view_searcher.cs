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
this.Size = new Size(500, 150);
this.Text = "Znajdź";
lb_input = new Label();
lb_input.Text = "Znajdź";
lb_input.Location = new Point(10, 20);
lb_input.Size = new Size(100, 50);
this.Controls.Add(lb_input);
edt_input = new TextBox();
edt_input.Location = new Point(150, 20);
edt_input.Size = new Size(300, 50);
this.Controls.Add(edt_input);
btn_search = new Button();
btn_search.Text = "Znajdź następny";
btn_search.Location = new Point(350, 70);
btn_search.Size = new Size(100, 50);
this.Controls.Add(btn_search);
btn_close = new Button();
btn_close.Text = "Anuluj";
btn_close.Location = new Point(250, 70);
btn_close.Size = new Size(100, 50);
this.Controls.Add(btn_close);
btn_close.Click += (sender, e) => this.Close();
btn_search.Click += (sender, e) => FindNext.Invoke(this, EventArgs.Empty);
this.CancelButton = btn_close;
this.AcceptButton = btn_search;
}
public string CurrentSearchPhrase {get => edt_input.Text;}
}
}