using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Sql;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Users
{
    /// <summary>
    /// Логика взаимодействия для UsersControl.xaml
    /// </summary>
    public partial class UsersControl : UserControl
    {
        public UsersControl()
        {
            InitializeComponent();

            AddItemButton.Click += AddItemButton_Click;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            SearchButton.Click += SearchButton_Click;

            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = false;
                    SearchTextBox.IsEnabled = false;
                    SearchButton.IsEnabled = false;
                    SearchField.IsEnabled = false;
                }));
                await Task.Delay(1);

                foreach (var user in SqlDatabase.Instance.UsersAdapter.GetAll())
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var item = new UserItemControl(user);
                        UserItems.Children.Add(item);
                    }));
                    await Task.Delay(1);
                }

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = true;
                    SearchTextBox.IsEnabled = true;
                    SearchButton.IsEnabled = true;
                    SearchField.IsEnabled = true;
                }));
            });
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextBox.Text.Length == 0)
            {
                foreach (UIElement item in UserItems.Children)
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

            switch (SearchField.SelectedIndex)
            {
                case 0:
                    foreach (UserItemControl item in UserItems.Children)
                    {
                        if (item.UserItem.Login.Contains(SearchTextBox.Text))
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    break;

                case 1:
                    foreach (UserItemControl item in UserItems.Children)
                    {
                        if (item.UserItem.Phone.Contains(SearchTextBox.Text))
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    break;

                case 2:
                    foreach (UserItemControl item in UserItems.Children)
                    {
                        if (item.UserItem.Email.Contains(SearchTextBox.Text))
                            item.Visibility = Visibility.Visible;
                        else
                            item.Visibility = Visibility.Collapsed;
                    }
                    break;
            }

            AddItemButton.IsEnabled = false;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new UsersWindow();
            if (addWindow.ShowDialog() == true)
            {
                var item = new UserItemControl(addWindow.UserItem);
                UserItems.Children.Add(item);
            }
        }
    }
}
