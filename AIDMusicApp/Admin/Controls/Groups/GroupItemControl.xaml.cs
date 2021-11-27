using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Models;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Groups
{
    /// <summary>
    /// Логика взаимодействия для GroupItemControl.xaml
    /// </summary>
    public partial class GroupItemControl : UserControl
    {
        public GroupItemControl(Group group)
        {
            InitializeComponent();

            InfoButton.Click += InfoButton_Click;
            EditButton.Click += EditButton_Click;
            RemoveButton.Click += RemoveButton_Click;

            GroupItem.Id = group.Id;
            GroupItem.Name = group.Name;
            GroupItem.Description = group.Description;
            GroupItem.YearOfCreation = group.YearOfCreation;
            GroupItem.YearOfBreakup = group.YearOfBreakup;
            GroupItem.CountryId = group.CountryId;
            GroupItem.Logo = group.Logo;
            GroupItem.Genres = group.Genres;
            GroupItem.Labels = group.Labels;
            GroupItem.Members = group.Members;
            GroupItem.Albums = group.Albums;
        }

        public Group GroupItem => (Group)Resources["GroupItem"];

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new GroupInfoWindow(GroupItem);
            infoWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new GroupsWindow(GroupItem);
            editWindow.ShowDialog();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            GroupItem.Delete();
            (Parent as WrapPanel).Children.Remove(this);
        }
    }
}
