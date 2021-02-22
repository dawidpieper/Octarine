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

this.Size = new Size(320, 240);
this.Text = "Pobieranie - Octarine";
this.ShowInTaskbar=false;

lb_downloads = new Label();
lb_downloads.Text = "Dostępne pobierania";
lb_downloads.Size = new Size(100, 150);
lb_downloads.Location = new Point(20, 20);
this.Controls.Add(lb_downloads);
lst_downloads = new ListBox();
lst_downloads.Size = new Size(160, 150);
lb_downloads.Location = new Point(140, 20);
this.Controls.Add(lst_downloads);

btn_download = new Button();
btn_download.Text = "Pobierz";
btn_download.Size = new Size(130, 50);
btn_download.Location = new Point(20, 250);
btn_download.Click += (sender, e) => DownloadFile(lst_downloads.SelectedIndex);
this.Controls.Add(btn_download);

btn_close = new Button();
btn_close.Text = "Zamknij";
btn_close.Size = new Size(130, 50);
btn_close.Location = new Point(170, 250);
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