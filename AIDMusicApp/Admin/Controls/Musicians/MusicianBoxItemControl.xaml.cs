using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIDMusicApp.Admin.Controls.Musicians
{
    /// <summary>
    /// Логика взаимодействия для MusicianBoxItemControl.xaml
    /// </summary>
    public partial class MusicianBoxItemControl : UserControl
    {
        public MusicianBoxItemControl()
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

        public Musician MusicianItem => (Musician)((ComboBoxItem)MusicianComboBox.SelectedItem)?.Tag ?? null;

        public bool IsFormer { get => Former.IsChecked ?? false; private set => Former.IsChecked = value; }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
