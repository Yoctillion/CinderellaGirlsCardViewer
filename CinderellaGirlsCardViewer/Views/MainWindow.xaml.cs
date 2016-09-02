using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CinderellaGirlsCardViewer.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebBrowser _browser;

        public MainWindow()
        {
            InitializeComponent();
            this.InitBrowser();
        }

        private void InitBrowser()
        {
            this._browser = new WebBrowser();
            this._browser.Navigating += BrowserOnNavigating;
            this._browser.Navigate("http://sp.mbga.jp/_lg?_from=globalHeaderNaviGuest");

            this.Content.Content = this._browser;
        }

        private void BrowserOnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var url = e.Uri.ToString();

            Debug.WriteLine("Navegating to " + url);

            if (url == "http://sp.mbga.jp/")
            {
                e.Cancel = true;
                this._browser.Navigating -= this.BrowserOnNavigating;
                this._browser = null;

                this.Content.Content = new GalleryView();
            }
        }
    }
}
