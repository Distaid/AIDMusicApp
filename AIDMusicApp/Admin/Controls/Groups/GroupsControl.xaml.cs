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
                    SearchField.IsEnabled = true;
                }));
            });
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
