using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для AlbumsPageControl.xaml
    /// </summary>
    public partial class AlbumsPageControl : UserControl
    {
        private ObservableCollection<Tuple<Album, string>> _newAlbums = new ObservableCollection<Tuple<Album, string>>();
        private ObservableCollection<Tuple<Album, string>> _searchAlbums = new ObservableCollection<Tuple<Album, string>>();

        public AlbumsPageControl()
        {
            InitializeComponent();

            SearchButton.Click += SearchButton_Click;

            NewAlbums.ItemsSource = _newAlbums;
            SearchAlbums.ItemsSource = _searchAlbums;

            foreach (var album in SqlDatabase.Instance.AlbumsAdapter.GetTop10())
            {
                var group = SqlDatabase.Instance.DiscographyAdapter.GetGroupNameByAlbumId(album.Id);
                _newAlbums.Add(new Tuple<Album, string>(album, group));
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Trim() != "")
            {
                if (SearchBlock.Visibility == Visibility.Collapsed)
                    SearchBlock.Visibility = Visibility.Visible;

                _searchAlbums.Clear();

                if (SearchType.SelectedIndex == 0)
                {
                    foreach (var album in SqlDatabase.Instance.AlbumsAdapter.GetAllByName(SearchBox.Text))
                    {
                        var group = SqlDatabase.Instance.DiscographyAdapter.GetGroupNameByAlbumId(album.Id);
                        _searchAlbums.Add(new Tuple<Album, string>(album, group));
                    }
                }
                else if (SearchType.SelectedIndex == 1)
                {
                    foreach (var album in SqlDatabase.Instance.DiscographyAdapter.GetAlbumsByGroupName(SearchBox.Text))
                        _searchAlbums.Add(album);
                }
            }
        }
    }
}
