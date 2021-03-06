using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Formats
{
    /// <summary>
    /// Логика взаимодействия для AlbumFormatBoxItemControl.xaml
    /// </summary>
    public partial class FormatBoxItemControl : UserControl
    {
        public FormatBoxItemControl()
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            FormatComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var albumFormat in SqlDatabase.Instance.FormatsListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = albumFormat.Name, Tag = albumFormat };
                FormatComboBox.Items.Add(item);
            }
        }

        public FormatBoxItemControl(int formatId)
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            FormatComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var format in SqlDatabase.Instance.FormatsListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = format.Name, Tag = format };
                if (format.Id == formatId)
                    item.IsSelected = true;
                FormatComboBox.Items.Add(item);
            }
        }

        public Format FormatItem => (Format)((ComboBoxItem)FormatComboBox.SelectedItem)?.Tag ?? null;

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
