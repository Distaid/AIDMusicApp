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

            RemoveButton.Click += RemoveButton_Click;

            GroupItem.Id = group.Id;
            GroupItem.Name = group.Name;
            GroupItem.Description = group.Description;
            GroupItem.YearOfCreation = group.YearOfCreation;
            GroupItem.YearOfBreakup = group.YearOfBreakup;
            GroupItem.CountryId = group.CountryId;
        }

        public Group GroupItem => (Group)Resources["GroupItem"];

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            GroupItem.Delete();
            (Parent as WrapPanel).Children.Remove(this);
        }
    }
}
