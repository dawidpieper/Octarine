using System;
using System.Windows.Forms;
using System.Drawing;

namespace Octarine {
public class PageRangeWindow : Form {
public readonly int PageCount;

public int PageFirst {get{return (int)ud_pageFirst.Value;}}
public int PageLast {get{return (int)ud_pageLast.Value;}}

public bool Cancelled=false;

private Label lb_info, lb_pagesTotal, lb_pageFirst, lb_pageLast;
private NumericUpDown ud_pageFirst, ud_pageLast;
private Button btn_ok, btn_cancel;

public PageRangeWindow(int count) {
PageCount=count;

this.FormBorderStyle = FormBorderStyle.FixedDialog ;
this.ShowInTaskbar=false;

this.Size = new Size(600, 400);
this.StartPosition = FormStartPosition.CenterScreen;
this.Text = "Wybór zakresu stron - Octarine";

lb_info = new Label() {
Left = 25,
Top = 25,
Width = 100,
Height = 25,
Text = "Liczba stron w tym dokumencie"
            };
this.Controls.Add(lb_info);

lb_pagesTotal = new Label() {
Left = 150,
Top = 25,
Width = 50,
Height = 25,
Text = this.PageCount.ToString()
            };
this.Controls.Add(lb_pagesTotal);

lb_pageFirst = new Label() {
Left = 25,
Top = 75,
Width = 100,
Height = 25,
Text = "Pierwsza strona do rozpoznania:"
};
this.Controls.Add(lb_pageFirst);

ud_pageFirst = new NumericUpDown() {
Left = 150,
Top = 75,
Width = 100,
Height = 25,
Minimum=1,
Maximum=this.PageCount,
Value=1
            };
ud_pageFirst.ValueChanged += (sender, e) => {
ud_pageLast.Minimum=ud_pageFirst.Value;
};
this.Controls.Add(ud_pageFirst);

lb_pageLast = new Label() {
Left = 25,
Top = 125,
Width = 100,
Height = 25,
Text = "Ostatnia strona do rozpoznania: "
            };
this.Controls.Add(lb_pageLast);

ud_pageLast = new NumericUpDown() {
Left = 150,
Top = 125,
Width = 100,
Height = 25,
Minimum=1,
Maximum=this.PageCount,
Value=this.PageCount
};
ud_pageLast.ValueChanged += (sender, e) => {
ud_pageFirst.Maximum=ud_pageLast.Value;
};
this.Controls.Add(ud_pageLast);

btn_ok = new Button() {
Left = 250,
Top = 75,
Width = 100,
Height = 30,
Text = "OK"
};
btn_ok.Click+= (sender, e) => this.Close();
this.Controls.Add(btn_ok);
this.AcceptButton = btn_ok;

btn_cancel = new Button(){
Left = 250,
Top = 125,
Width = 100,
Height = 30,
Text = "Anuluj"
            };
btn_cancel.Click += (sender, e) => {
Cancelled=true;
this.Close();
};
this.Controls.Add(btn_cancel);
this.CancelButton = btn_cancel;
}
}
}