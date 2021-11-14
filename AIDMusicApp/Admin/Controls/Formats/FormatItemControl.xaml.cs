using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Formats
{
    /// <summary>
    /// Логика взаимодействия для FormatItemControl.xaml
    /// </summary>
    public partial class FormatItemControl : UserControl
    {
        public FormatItemControl(Format albumFormat)
        {
            InitializeComponent();

            FormatItem.Id = albumFormat.Id;
            FormatItem.Name = albumFormat.Name;

            EditButton.Click += EditButton_Click;
            RemoveButton.Click += RemoveButton_Click;
        }

        public Format FormatItem => (Format)Resources["FormatItem"];

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new FormatsWindow(FormatItem);
            editWindow.ShowDialog();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            FormatItem.Delete();
            (Parent as WrapPanel).Children.Remove(this);
        }
    }
}
