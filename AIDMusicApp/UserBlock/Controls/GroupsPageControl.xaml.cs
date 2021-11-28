using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для GroupsPageControl.xaml
    /// </summary>
    public partial class GroupsPageControl : UserControl
    {
        private ObservableCollection<Group> _newGroups = new ObservableCollection<Group>();
        private ObservableCollection<Group> _searchGroups = new ObservableCollection<Group>();

        public GroupsPageControl()
        {
            InitializeComponent();

            SearchButton.Click += SearchButton_Click;

            NewGroups.ItemsSource = _newGroups;
            SearchGroups.ItemsSource = _searchGroups;

            foreach (var group in SqlDatabase.Instance.GroupsAdapter.GetTop10())
                _newGroups.Add(group);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Trim() != "")
            {
                if (SearchBlock.Visibility == Visibility.Collapsed)
                    SearchBlock.Visibility = Visibility.Visible;

                _searchGroups.Clear();

                foreach (var group in SqlDatabase.Instance.GroupsAdapter.GetAllByName(SearchBox.Text))
                    _searchGroups.Add(group);
            }
        }
    }
}
