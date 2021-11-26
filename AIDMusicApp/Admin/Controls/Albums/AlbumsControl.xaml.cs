using AIDMusicApp.Sql;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Albums
{
    /// <summary>
    /// Логика взаимодействия для AlbumsControl.xaml
    /// </summary>
    public partial class AlbumsControl : UserControl
    {
        public AlbumsControl()
        {
            InitializeComponent();

            GroupField.SelectionChanged += GroupField_SelectionChanged;

            foreach (var group in SqlDatabase.Instance.GroupsAdapter.GetAllIdName())
                GroupField.Items.Add(new ComboBoxItem { Content = group.Name, Tag = group.Id });
        }

        private void GroupField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                var groupId = 0;

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = false;
                    GroupField.IsEnabled = false;
                    AlbumItems.Children.Clear();
                    groupId = (int)(GroupField.SelectedItem as ComboBoxItem).Tag;
                }));
                await Task.Delay(1);

                foreach (var group in SqlDatabase.Instance.DiscographyAdapter.GetAlbumsByGroupId(groupId))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var item = new AlbumItemControl(group);
                        AlbumItems.Children.Add(item);
                    }));
                    await Task.Delay(1);
                }

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = true;
                    GroupField.IsEnabled = true;
                }));
            });
        }
    }
}
