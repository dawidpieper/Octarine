using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Octarine {
public class DownloaderWindow : Form {

private Label lb_downloads;
private ListBox lst_downloads;
private Button btn_download, btn_close;
private OctarineController controller;
private List<OctarineDownloadItem> items;

public DownloaderWindow(OctarineController _controller) {
controller=_controller;
items = new List<OctarineDownloadItem>();

this.Size = new Size(400, 200);
this.Text = "Pobieranie - Octarine";
this.ShowInTaskbar=false;

lb_downloads = new Label() {
Left=30,
Top=10,
Width=100,
Height=20,
Text="Dostępne pobierania"
};
this.Controls.Add(lb_downloads);

lst_downloads = new ListBox() {
Left=30,
Top=30,
Width=200,
Height=100
};
this.Controls.Add(lst_downloads);

btn_download = new Button() {
Left=30,
Top=150,
Width=80,
Height=20,
Text="Pobierz"
};
btn_download.Click += (sender, e) => DownloadFile(lst_downloads.SelectedIndex);
this.Controls.Add(btn_download);

btn_close = new Button() {
Left=120,
Top=150,
Width=80,
Height=20,
Text="Zamknij"
};
btn_close.Click += (sender, e) => this.Close();
this.Controls.Add(btn_close);

this.CancelButton = btn_close;
}

public void AddItem(OctarineDownloadItem item) {
items.Add(item);
lst_downloads.Items.Add(item.Name);
}

public bool DownloadFile(int index) {
if(index==-1) return false;
if(index>=items.Count) return false;
var item = items[index];
return controller.OpenDownloader(item.Sources, item.Destinations, this, $"Pobieranie - {item.Name}");
}
}
}