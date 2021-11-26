using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Musicians
{
    /// <summary>
    /// Логика взаимодействия для MusicianBoxItemControl.xaml
    /// </summary>
    public partial class MusicianBoxItemControl : UserControl
    {
        public MusicianBoxItemControl(bool isNeedFormer = true)
        {
            InitializeComponent();

            if (!isNeedFormer)
                Former.Visibility = Visibility.Collapsed;

            RemoveButton.Click += RemoveButton_Click;

            MusicianComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var musician in SqlDatabase.Instance.MusiciansAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = musician.Name, Tag = musician };
                MusicianComboBox.Items.Add(item);
            }
        }

        public MusicianBoxItemControl(int musicianId, bool isFormer)
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            MusicianComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var musician in SqlDatabase.Instance.MusiciansAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = musician.Name, Tag = musician };
                if (musician.Id == musicianId)
                    item.IsSelected = true;
                MusicianComboBox.Items.Add(item);
            }
            IsFormer = isFormer;
        }

        public MusicianBoxItemControl(int musicianId)
        {
            InitializeComponent();

            Former.Visibility = Visibility.Collapsed;

            RemoveButton.Click += RemoveButton_Click;

            MusicianComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var musician in SqlDatabase.Instance.MusiciansAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = musician.Name, Tag = musician };
                if (musician.Id == musicianId)
                    item.IsSelected = true;
                MusicianComboBox.Items.Add(item);
            }
        }

        public Musician MusicianItem => (Musician)((ComboBoxItem)MusicianComboBox.SelectedItem)?.Tag ?? null;

        public bool IsFormer { get => Former.IsChecked ?? false; private set => Former.IsChecked = value; }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
