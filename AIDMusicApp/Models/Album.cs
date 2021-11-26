using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AIDMusicApp.Models
{
    public class Album : INotifyPropertyChanged
    {
        private int _id;

        private string _name;

        private string _description;

        private short _year;

        private byte[] _cover;

        private ObservableCollection<Genre> _genres;

        private ObservableCollection<Format> _formats;

        private ObservableCollection<Song> _songs;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public short Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged("Year");
            }
        }

        public byte[] Cover
        {
            get => _cover;
            set
            {
                _cover = value;
                OnPropertyChanged("Cover");
            }
        }

        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set
            {
                _genres = value;
                OnPropertyChanged("Genres");
            }
        }

        public ObservableCollection<Format> Formats
        {
            get => _formats;
            set
            {
                _formats = value;
                OnPropertyChanged("Formats");
            }
        }

        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set
            {
                _songs = value;
                OnPropertyChanged("Songs");
            }
        }

        public void Update(string name, short year, string description, byte[] cover)
        {
            SqlDatabase.Instance.AlbumsAdapter.Update(Id, name, year, description, cover);

            Name = name;
            Year = year;
            Description = description;
            Cover = cover;
        }

        public void Delete()
        {
            SqlDatabase.Instance.AlbumsAdapter.Delete(Id);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
