using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Songs
{
    /// <summary>
    /// Логика взаимодействия для SongItemControl.xaml
    /// </summary>
    public partial class SongItemControl : UserControl
    {
        private ObservableCollection<Song> _songs;
        private Song _song;

        public SongItemControl(Song song, ObservableCollection<Song> songs)
        {
            InitializeComponent();

            EditButton.Click += EditButton_Click;
            RemoveButton.Click += RemoveButton_Click;

            _songs = songs;
            _song = song;

            SongItem.Id = song.Id;
            SongItem.Name = song.Name;
            SongItem.Time = song.Time;
            SongItem.Guests = song.Guests;
        }

        public Song SongItem => (Song)Resources["SongItem"];

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new SongsWindow(SongItem);
            editWindow.ShowDialog();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _songs.Remove(_song);
            _song.Delete();
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
