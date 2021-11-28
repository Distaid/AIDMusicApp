using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AIDMusicApp.UserBlock.Windows
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

            TitleText.Text = group.Name;

            GroupItem.Id = group.Id;
            GroupItem.Name = group.Name;
            GroupItem.CountryId = group.CountryId;
            GroupItem.YearOfCreation = group.YearOfCreation;
            GroupItem.YearOfBreakup = group.YearOfBreakup;
            GroupItem.Description = group.Description;
            GroupItem.Logo = group.Logo;
            GroupItem.Genres = new ObservableCollection<Genre>();
            GroupItem.Labels = new ObservableCollection<Label>();
            GroupItem.Members = new ObservableCollection<Musician>();
            GroupItem.Albums = new ObservableCollection<Album>();

            foreach (var genre in SqlDatabase.Instance.GroupGenresAdapter.GetGenresByGroupId(group.Id))
                GroupItem.Genres.Add(genre);

            foreach (var label in SqlDatabase.Instance.ContractsAdapter.GetLabelsByGroupId(group.Id))
                GroupItem.Labels.Add(label);

            foreach (var musician in SqlDatabase.Instance.MembersAdapter.GetMusiciansByGroupId(group.Id))
                GroupItem.Members.Add(musician);

            foreach (var album in SqlDatabase.Instance.DiscographyAdapter.GetShortAlbumsByGroupId(group.Id))
                GroupItem.Albums.Add(album);
        }

        public Group GroupItem => (Group)Resources["GroupItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
