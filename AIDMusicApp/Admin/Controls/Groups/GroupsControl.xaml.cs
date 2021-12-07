using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Sql;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Groups
{
    /// <summary>
    /// Логика взаимодействия для GroupsControl.xaml
    /// </summary>
    public partial class GroupsControl : UserControl
    {
        public GroupsControl()
        {
            InitializeComponent();

            AddItemButton.Click += AddItemButton_Click;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            SearchButton.Click += SearchButton_Click;

            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    CountText.Text = $"Всего: {SqlDatabase.Instance.GroupsAdapter.GetCount()}";
                }));
                await Task.Delay(1);

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = false;
                    SearchTextBox.IsEnabled = false;
                    SearchButton.IsEnabled = false;
                }));
                await Task.Delay(1);

                foreach (var group in SqlDatabase.Instance.GroupsAdapter.GetAll())
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var item = new GroupItemControl(group);
                        GroupItems.Children.Add(item);
                    }));
                    await Task.Delay(1);
                }

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = true;
                    SearchTextBox.IsEnabled = true;
                    SearchButton.IsEnabled = true;
                }));
            });
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text.Length == 0)
            {
                foreach (UIElement item in GroupItems.Children)
                {
                    item.Visibility = Visibility.Visible;
                }

                AddItemButton.IsEnabled = true;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text.Length == 0)
                return;

            foreach (GroupItemControl item in GroupItems.Children)
            {
                if (item.GroupItem.Name.Contains(SearchTextBox.Text))
                    item.Visibility = Visibility.Visible;
                else
                    item.Visibility = Visibility.Collapsed;
            }

            AddItemButton.IsEnabled = false;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new GroupsWindow();
            if (addWindow.ShowDialog() == true)
            {
                var item = new GroupItemControl(addWindow.GroupItem);
                GroupItems.Children.Add(item);
            }
        }
    }
}
