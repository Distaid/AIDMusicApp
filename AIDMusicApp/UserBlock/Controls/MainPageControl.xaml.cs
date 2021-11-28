using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainPageControl.xaml
    /// </summary>
    public partial class MainPageControl : UserControl
    {
        private ObservableCollection<Group> _newGroups = new ObservableCollection<Group>();
        private ObservableCollection<Tuple<Album, string>> _newAlbums = new ObservableCollection<Tuple<Album, string>>();
        private ObservableCollection<Musician> _newMusicians = new ObservableCollection<Musician>();

        public MainPageControl()
        {
            InitializeComponent();

            NewGroups.ItemsSource = _newGroups;
            NewAlbums.ItemsSource = _newAlbums;
            NewMusicians.ItemsSource = _newMusicians;

            foreach (var group in SqlDatabase.Instance.GroupsAdapter.GetTop10())
                _newGroups.Add(group);

            foreach (var album in SqlDatabase.Instance.AlbumsAdapter.GetTop10())
            {
                var group = SqlDatabase.Instance.DiscographyAdapter.GetGroupNameByAlbumId(album.Id);
                _newAlbums.Add(new Tuple<Album, string>(album, group));
            }

            foreach (var musician in SqlDatabase.Instance.MusiciansAdapter.GetTop10())
                _newMusicians.Add(musician);
        }
    }
}
