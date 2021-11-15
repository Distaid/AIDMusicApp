using AIDMusicApp.Admin.Controls.Albums;
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
    /// Логика взаимодействия для GroupInfoWindow.xaml
    /// </summary>
    public partial class GroupInfoWindow : Window
    {
        public GroupInfoWindow(Group group)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            AddAlbum.Click += AddAlbum_Click;

            GroupItem.Id = group.Id;
            GroupItem.Name = group.Name;
            GroupItem.CountryId = group.CountryId;
            GroupItem.YearOfCreation = group.YearOfCreation;
            GroupItem.YearOfBreakup = group.YearOfBreakup;
            GroupItem.Description = group.Description;
            GroupItem.Genres = group.Genres;
            GroupItem.Albums = group.Albums;
            GroupItem.Members = group.Members;
            GroupItem.Labels = group.Labels;
            TitleText.Text = group.Name;
        }

        public Group GroupItem => (Group)Resources["GroupItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            var addAlbumWindow = new AlbumsWindow(GroupItem.Id);
            if (addAlbumWindow.ShowDialog() == true)
                GroupItem.Albums.Add(addAlbumWindow.AlbumItem);
        }
    }
}
