using AIDMusicApp.Sql;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Labels
{
    /// <summary>
    /// Логика взаимодействия для LabelBoxItemControl.xaml
    /// </summary>
    public partial class LabelBoxItemControl : UserControl
    {
        public LabelBoxItemControl()
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            LabelComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var label in SqlDatabase.Instance.LabelsListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = label.Name, Tag = label };
                LabelComboBox.Items.Add(item);
            }
        }

        public LabelBoxItemControl(int labelId)
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            LabelComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var label in SqlDatabase.Instance.LabelsListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = label.Name, Tag = label };
                if (label.Id == labelId)
                    item.IsSelected = true;
                LabelComboBox.Items.Add(item);
            }
        }

        public Models.Label LabelItem => (Models.Label)((ComboBoxItem)LabelComboBox.SelectedItem)?.Tag ?? null;

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
