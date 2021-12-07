using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Sql;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Musicians
{
    /// <summary>
    /// Логика взаимодействия для MusiciansControl.xaml
    /// </summary>
    public partial class MusiciansControl : UserControl
    {
        public MusiciansControl()
        {
            InitializeComponent();

            AddItemButton.Click += AddItemButton_Click;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            SearchButton.Click += SearchButton_Click;

            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    CountText.Text = $"Всего: {SqlDatabase.Instance.MusiciansAdapter.GetCount()}";
                }));
                await Task.Delay(1);

                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddItemButton.IsEnabled = false;
                    SearchTextBox.IsEnabled = false;
                    SearchButton.IsEnabled = false;
                }));
                await Task.Delay(1);

                foreach (var musician in SqlDatabase.Instance.MusiciansAdapter.GetAll())
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var item = new MusicianItemControl(musician);
                        MusicianItems.Children.Add(item);
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
                foreach (UIElement item in MusicianItems.Children)
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

            foreach (MusicianItemControl item in MusicianItems.Children)
            {
                if (item.MusicianItem.Name.Contains(SearchTextBox.Text))
                    item.Visibility = Visibility.Visible;
                else
                    item.Visibility = Visibility.Collapsed;
            }

            AddItemButton.IsEnabled = false;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new MusiciansWindow();
            if (addWindow.ShowDialog() == true)
            {
                var item = new MusicianItemControl(addWindow.MusicianItem);
                MusicianItems.Children.Add(item);
            }
        }
    }
}
