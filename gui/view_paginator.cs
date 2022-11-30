using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace Octarine {
public class PageEventArgs : EventArgs {
public int Page;
public PageEventArgs(int page) {
Page=page;
}
}

public class PagesWindow : Form {
public event EventHandler<PageEventArgs> PageJump;
public event EventHandler<PageEventArgs> PageDelete;

private Label lb_pages;
private ListBox lst_pages;
private Button btn_jump, btn_delete, btn_close;

private OCRResult Result;

public PagesWindow(OCRResult result) {
Result = result;
this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
this.ShowInTaskbar=false;
this.Size = new Size(480, 360);
this.StartPosition = FormStartPosition.CenterScreen;
this.Text = "Strony";

lb_pages = new Label();
lb_pages.Text = "Strony";
lb_pages.Location = new Point(20, 20);
lb_pages.Size = new Size(300, 50);
this.Controls.Add(lb_pages);

lst_pages = new ListBox();
lst_pages.Location = new Point(70, 20);
lst_pages.Size = new Size(200, 270);
this.Controls.Add(lst_pages);

btn_jump = new Button();
btn_jump.Text = "Przejdź";
btn_jump.Size = new Size(100, 100);
btn_jump.Location = new Point(240, 20);
btn_jump.Click += (sender, e) => {
if(Result.Pages.Count()>0 && lst_pages.SelectedIndex!=-1 && PageJump!=null) {
PageJump(this, new PageEventArgs(lst_pages.SelectedIndex));
}
};
this.Controls.Add(btn_jump);

btn_delete = new Button();
btn_delete.Text = "Usuń";
btn_delete.Size = new Size(100, 100);
btn_delete.Location = new Point(240, 240);
btn_delete.Click += (sender, e) => {
if(Result.Pages.Count()>0 && lst_pages.SelectedIndex!=-1 && PageDelete!=null)
if(MessageBox.Show(this, "Czy na pewno chcesz usunąć tę stronę?", "Octarine", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)==DialogResult.Yes) {
PageDelete(this, new PageEventArgs(lst_pages.SelectedIndex));
UpdateStatus();
}
};
this.Controls.Add(btn_delete);

btn_close = new Button();
btn_close.Text = "Zamknij";
btn_close.Size = new Size(100, 100);
btn_close.Location = new Point(360, 240);
btn_close.Click += (sender, e) => {
this.Close();
};
this.Controls.Add(btn_close);

this.CancelButton=btn_close;
this.AcceptButton = btn_jump;

UpdateStatus();
}

private void UpdateStatus() {
while(Result.Pages.Count() > lst_pages.Items.Count) {
lst_pages.Items.Add("Strona "+(lst_pages.Items.Count+1).ToString());
}
while(Result.Pages.Count() < lst_pages.Items.Count) {
lst_pages.Items.RemoveAt(lst_pages.Items.Count-1);
}
if(Result.Pages.Count()==0) {
btn_jump.Enabled=false;
btn_delete.Enabled=false;
} else {
btn_jump.Enabled=true;
btn_delete.Enabled=true;
}
}


public void SetPageIndex(int i) {
if(lst_pages.Items.Count>i) lst_pages.SelectedIndex=i;
}
}
}