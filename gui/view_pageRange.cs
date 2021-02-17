using System;
using System.Windows.Forms;
using System.Drawing;

namespace Octarine {
public class PageRangeWindow : Form {
public readonly int PageCount;

public int PageFirst {get{return (int)ud_pageFirst.Value;}}
public int PageLast {get{return (int)ud_pageLast.Value;}}

public bool Cancelled=false;

private Label lb_info, lb_pageFirst, lb_pageLast;
private NumericUpDown ud_pageFirst, ud_pageLast;
private Button btn_ok, btn_cancel;

public PageRangeWindow(int count) {
PageCount=count;

this.FormBorderStyle = FormBorderStyle.FixedDialog ;
this.ShowInTaskbar=false;

this.Size = new Size(320, 240);
this.StartPosition = FormStartPosition.CenterScreen;
this.Text = "Wybór zakresu stron - Octarine";

lb_info = new Label();
lb_info.Size = new Size(280, 50);
lb_info.Location = new Point(20, 20);
lb_info.Text = $"Liczba stron w tym dokumencie: {this.PageCount}.";
this.Controls.Add(lb_info);

lb_pageFirst = new Label();
lb_pageFirst.Text = "Pierwsza strona do rozpoznania";
lb_pageFirst.Size = new Size(200, 40);
lb_pageFirst.Location = new Point(20, 90);
this.Controls.Add(lb_pageFirst);

ud_pageFirst = new NumericUpDown();
ud_pageFirst.Size = new Size(80, 40);
ud_pageFirst.Location = new Point(220, 90);
ud_pageFirst.Minimum=1;
ud_pageFirst.Maximum=this.PageCount;
ud_pageFirst.Value=1;
ud_pageFirst.ValueChanged += (sender, e) => {
ud_pageLast.Minimum=ud_pageFirst.Value;
};
this.Controls.Add(ud_pageFirst);

lb_pageLast = new Label();
lb_pageLast.Text = "Ostatnia strona do rozpoznania";
lb_pageLast.Size = new Size(200, 40);
lb_pageLast.Location = new Point(20, 130);
this.Controls.Add(lb_pageLast);

ud_pageLast = new NumericUpDown();
ud_pageLast.Size = new Size(80, 40);
ud_pageLast.Location = new Point(220, 130);
ud_pageLast.Minimum=1;
ud_pageLast.Maximum=this.PageCount;
ud_pageLast.Value=this.PageCount;
ud_pageLast.ValueChanged += (sender, e) => {
ud_pageFirst.Maximum=ud_pageLast.Value;
};
this.Controls.Add(ud_pageLast);

btn_ok = new Button();
btn_ok.Text = "OK";
btn_ok.Size = new Size(140, 40);
btn_ok.Location = new Point(20, 40);
btn_ok.Click+= (sender, e) => this.Close();
this.Controls.Add(btn_ok);
this.AcceptButton = btn_ok;

btn_cancel = new Button();
btn_cancel.Text = "Anuluj";
btn_cancel.Size = new Size(140, 40);
btn_cancel.Location = new Point(160, 40);
btn_cancel.Click += (sender, e) => {
Cancelled=true;
this.Close();
};
this.Controls.Add(btn_cancel);
this.CancelButton = btn_cancel;
}
}
}