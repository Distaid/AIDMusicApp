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
using System.Windows.Shapes;

namespace AIDMusicApp.Admin.Windows
{
    /// <summary>
    /// Логика взаимодействия для AlbumInfoWindow.xaml
    /// </summary>
    public partial class AlbumInfoWindow : Window
    {
        public AlbumInfoWindow(Album album)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;

            TitleText.Text = album.Name;

            AlbumItem.Id = album.Id;
            AlbumItem.Name = album.Name;
            AlbumItem.Year = album.Year;
            AlbumItem.Description = album.Description;
            AlbumItem.Cover = album.Cover;
            AlbumItem.Genres = album.Genres;
            AlbumItem.Formats = album.Formats;
        }

        public Album AlbumItem => (Album)Resources["AlbumItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
