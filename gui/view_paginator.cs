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
this.Size = new Size(600, 400);
this.StartPosition = FormStartPosition.CenterScreen;
this.Text = "Strony";

lb_pages = new Label() {
Left = 25,
Top = 25,
Width = 100,
Height = 25,
Text = "Strony"
            };
this.Controls.Add(lb_pages);

lst_pages = new ListBox() {
Left = 25,
Top = 50,
Width = 200,
Height = 300
            };
lst_pages.Location = new Point(70, 20);
lst_pages.Size = new Size(200, 270);
this.Controls.Add(lst_pages);

btn_jump = new Button() {
Left = 250,
Top = 50,
Width = 100,
Height = 30,
Text = "Przejdź"
            };
btn_jump.Click += (sender, e) => {
if(Result.Pages.Count()>0 && lst_pages.SelectedIndex!=-1 && PageJump!=null) {
PageJump(this, new PageEventArgs(lst_pages.SelectedIndex));
}
};
this.Controls.Add(btn_jump);

btn_delete = new Button() {
Left = 250,
Top = 100,
Width = 100,
Height = 30,
Text = "Usuń"
            };
btn_delete.Click += (sender, e) => {
if(Result.Pages.Count()>0 && lst_pages.SelectedIndex!=-1 && PageDelete!=null)
if(MessageBox.Show(this, "Czy na pewno chcesz usunąć tę stronę?", "Octarine", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)==DialogResult.Yes) {
PageDelete(this, new PageEventArgs(lst_pages.SelectedIndex));
UpdateStatus();
}
};
this.Controls.Add(btn_delete);

btn_close = new Button() {
Left = 425,
Top = 50,
Width = 100,
Height = 30,
Text = "Zamknij"
            };
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