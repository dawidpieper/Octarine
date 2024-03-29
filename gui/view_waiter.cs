using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Octarine {
public class LoadingWindow : Form {
private Label lb_status;
private ProgressBar pb_percentage;
private Button btn_cancel;
public LoadingWindow(string text="Ładowanie...") {
this.FormBorderStyle = FormBorderStyle.FixedDialog ;
this.ShowInTaskbar=false;

this.Size = new Size(320, 240);
this.StartPosition = FormStartPosition.CenterScreen;
this.Text = text;

lb_status = new Label();
lb_status.Size = new Size(280, 80);
lb_status.Location = new Point(20, 20);
this.Controls.Add(lb_status);

pb_percentage = new ProgressBar();
pb_percentage.Size = new Size(120, 30);
pb_percentage.Location = new Point(100, 115);
pb_percentage.Minimum=0;
pb_percentage.Step=1;
pb_percentage.Maximum=100;
this.Controls.Add(pb_percentage);

btn_cancel = new Button();
btn_cancel.Text = "Anuluj";
btn_cancel.Size = new Size(100, 60);
btn_cancel.Location = new Point(120, 160);
btn_cancel.Click += (Object, e) => {this.Close();};
this.Controls.Add(btn_cancel);
this.CancelButton = btn_cancel;
}

public void SetStatus(String t) {
try {
lb_status.Text=t;
lb_status.Update();
} catch(Exception) {}
}

public void SetPercentage(int p) {
try {
pb_percentage.Value=p;
pb_percentage.Update();
} catch(Exception) {}
}
}
}