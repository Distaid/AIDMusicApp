using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIDMusicApp.Admin.Controls.Albums
{
    /// <summary>
    /// Логика взаимодействия для AlbumItemControl.xaml
    /// </summary>
    public partial class AlbumItemControl : UserControl
    {
        public AlbumItemControl(Album album)
        {
            InitializeComponent();

            EditButton.Click += EditButton_Click;
            RemoveButton.Click += RemoveButton_Click;

            AlbumItem.Id = album.Id;
            AlbumItem.Name = album.Name;
            AlbumItem.Year = album.Year;
            AlbumItem.Formats = album.Formats;
            AlbumItem.Genres = album.Genres;
            AlbumItem.Cover = album.Cover;
            AlbumItem.Description = album.Description;
        }

        public Album AlbumItem => (Album)Resources["AlbumItem"];

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AlbumsWindow(AlbumItem);
            editWindow.ShowDialog();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            AlbumItem.Delete();
            (Parent as WrapPanel).Children.Remove(this);
        }
    }
}
