using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AIDMusicApp.Models
{
    public class Song : INotifyPropertyChanged
    {
        private int _id;

        private string _name;

        private string _time;

        private ObservableCollection<Musician> _guests;

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

        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        public ObservableCollection<Musician> Guests
        {
            get => _guests;
            set
            {
                _guests = value;
                OnPropertyChanged("Guests");
            }
        }

        public void Update(string name, string time)
        {
            SqlDatabase.Instance.SongsAdapter.Update(Id, name, time);

            Name = name;
            Time = time;
        }

        public void Delete()
        {
            SqlDatabase.Instance.SongsAdapter.Delete(Id);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
