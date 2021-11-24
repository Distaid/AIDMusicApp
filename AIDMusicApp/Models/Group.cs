using AIDMusicApp.Sql;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AIDMusicApp.Models
{
    public class Group : INotifyPropertyChanged
    {
        private int _id;

        private string _name;

        private string _description;

        private short? _yearOfCreation;

        private short? _yearOfBreakup;

        private Country _countryId;

        private ObservableCollection<Genre> _genres;

        private ObservableCollection<Label> _labels;

        private ObservableCollection<Album> _albums;

        private ObservableCollection<Musician> _members;

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

        public short? YearOfCreation
        {
            get => _yearOfCreation;
            set
            {
                _yearOfCreation = value;
                OnPropertyChanged("YearOfCreation");
            }
        }

        public short? YearOfBreakup
        {
            get => _yearOfBreakup;
            set
            {
                _yearOfBreakup = value;
                OnPropertyChanged("YearOfBreakup");
            }
        }

        public Country CountryId
        {
            get => _countryId;
            set
            {
                _countryId = value;
                OnPropertyChanged("CountryId");
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

        public ObservableCollection<Label> Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged("Labels");
            }
        }

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                OnPropertyChanged("Albums");
            }
        }

        public ObservableCollection<Musician> Members
        {
            get => _members;
            set
            {
                _members = value;
                OnPropertyChanged("Members");
            }
        }

        public void Delete()
        {
            SqlDatabase.Instance.GroupsAdapter.Delete(Id);
        }

        public void Update(string name, string description, short? yearOfCreation, short? yearOfBreakup, Country countryId)
        {
            SqlDatabase.Instance.GroupsAdapter.Update(Id, name, description, yearOfCreation, yearOfBreakup, countryId.Id);
            Name = name;
            Description = description;
            YearOfCreation = yearOfCreation;
            YearOfBreakup = yearOfBreakup;
            CountryId = countryId;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
