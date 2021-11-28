using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AIDMusicApp.UserBlock.Windows
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
            AlbumItem.Genres = new ObservableCollection<Genre>();
            AlbumItem.Formats = new ObservableCollection<Format>();
            AlbumItem.Songs = new ObservableCollection<Song>();

            foreach (var genre in SqlDatabase.Instance.AlbumGenresAdapter.GetGenresByGroupId(album.Id))
                AlbumItem.Genres.Add(genre);

            foreach (var format in SqlDatabase.Instance.AlbumFormatsAdapter.GetFormatsByGroupId(album.Id))
                AlbumItem.Formats.Add(format);

            foreach (var song in SqlDatabase.Instance.TrackListAdapter.GetSongsByAlbumId(album.Id))
                AlbumItem.Songs.Add(song);
        }

        public Album AlbumItem => (Album)Resources["AlbumItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
