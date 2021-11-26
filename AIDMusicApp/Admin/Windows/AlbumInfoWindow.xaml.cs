using AIDMusicApp.Models;
using System.Windows;
using System.Windows.Input;

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
            EditButton.Click += EditButton_Click;

            TitleText.Text = album.Name;

            AlbumItem.Id = album.Id;
            AlbumItem.Name = album.Name;
            AlbumItem.Year = album.Year;
            AlbumItem.Description = album.Description;
            AlbumItem.Cover = album.Cover;
            AlbumItem.Genres = album.Genres;
            AlbumItem.Formats = album.Formats;
            AlbumItem.Songs = album.Songs;
        }

        public Album AlbumItem => (Album)Resources["AlbumItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AlbumsWindow(AlbumItem);
            editWindow.ShowDialog();
        }
    }
}
