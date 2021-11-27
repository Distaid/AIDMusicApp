using AIDMusicApp.Models;
using System.Windows;
using System.Windows.Input;

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
            EditButton.Click += EditButton_Click;

            TitleText.Text = group.Name;

            GroupItem.Id = group.Id;
            GroupItem.Name = group.Name;
            GroupItem.CountryId = group.CountryId;
            GroupItem.YearOfCreation = group.YearOfCreation;
            GroupItem.YearOfBreakup = group.YearOfBreakup;
            GroupItem.Description = group.Description;
            GroupItem.Logo = group.Logo;
            GroupItem.Genres = group.Genres;
            GroupItem.Albums = group.Albums;
            GroupItem.Members = group.Members;
            GroupItem.Labels = group.Labels;
        }

        public Group GroupItem => (Group)Resources["GroupItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new GroupsWindow(GroupItem);
            editWindow.ShowDialog();
        }
    }
}
