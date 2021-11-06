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

namespace AIDMusicApp.Admin.Controls.Genres
{
    /// <summary>
    /// Логика взаимодействия для GenreBoxItemControl.xaml
    /// </summary>
    public partial class GenreBoxItemControl : UserControl
    {
        public GenreBoxItemControl()
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            GenreComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var skill in SqlDatabase.Instance.GenresListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = skill.Name, Tag = skill };
                GenreComboBox.Items.Add(item);
            }
        }

        public GenreBoxItemControl(int genreId)
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            GenreComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var genre in SqlDatabase.Instance.GenresListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = genre.Name, Tag = genre };
                if (genre.Id == genreId)
                    item.IsSelected = true;
                GenreComboBox.Items.Add(item);
            }
        }

        public Genre GenreItem => (Genre)((ComboBoxItem)GenreComboBox.SelectedItem)?.Tag ?? null;

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
