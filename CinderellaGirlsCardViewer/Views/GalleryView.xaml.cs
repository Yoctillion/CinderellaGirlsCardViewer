using System.Windows;
using System.Windows.Controls;
using CinderellaGirlsCardViewer.Models;
using CinderellaGirlsCardViewer.ViewModels;
using Microsoft.Win32;

namespace CinderellaGirlsCardViewer.Views
{
    /// <summary>
    /// GalleryView.xaml 的交互逻辑
    /// </summary>
    public partial class GalleryView : UserControl
    {
        private readonly GalleryViewModel _vm;

        public GalleryView()
        {
            InitializeComponent();

            this._vm = new GalleryViewModel();
            this.DataContext = this._vm;
        }

        private void QueryText_OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.SearchButton.IsDefault = true;
        }

        private void QueryText_OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.SearchButton.IsDefault = false;
        }

        private async void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            await this._vm.QueryCharacter(this.QueryText.Text);
        }

        private static readonly GridLength Star = new GridLength(1, GridUnitType.Star);
        private static readonly GridLength ZeroLength = new GridLength(0);

        private void IdolList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IdolList.SelectedItem != null)
            {
                this.IdolsColumn.Width = new GridLength(200);
                this.CardsColumn.Width = Star;
            }
            else
            {
                this.IdolsColumn.Width = Star;
                this.CardsColumn.Width = ZeroLength;
            }
        }

        private void CardsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CardsList.SelectedItem != null)
            {
                this.CardsList.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);

                this.CardsRow.Height = new GridLength(170);
                this.CardViewRow.Height = Star;
            }
            else
            {
                this.CardsList.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);

                this.CardsRow.Height = Star;
                this.CardViewRow.Height = ZeroLength;
            }
        }

        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var imageInfo = (CardImage)((Button)sender).Tag;
            var dialog = new SaveFileDialog
            {
                FileName = imageInfo.FileName,
                DefaultExt = ".jpg",
                Filter = "Image Files | *.jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                await this._vm.SaveImage(imageInfo.Url, path);
                MessageBox.Show("Save completed");
            }
        }
    }
}
