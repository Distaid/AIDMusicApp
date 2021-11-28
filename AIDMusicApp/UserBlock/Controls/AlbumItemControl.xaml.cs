using AIDMusicApp.Models;
using AIDMusicApp.UserBlock.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для AlbumItemControl.xaml
    /// </summary>
    public partial class AlbumItemControl : UserControl
    {
        public static readonly DependencyProperty AlbumItemProperty;
        public static readonly DependencyProperty GroupNameProperty;
        public static readonly DependencyProperty BorderBackgroundProperty;

        static AlbumItemControl()
        {
            AlbumItemProperty = DependencyProperty.Register("AlbumItem", typeof(Album), typeof(AlbumItemControl));
            GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(AlbumItemControl));
            BorderBackgroundProperty = DependencyProperty.Register("BorderBackground", typeof(Brush), typeof(AlbumItemControl));
        }

        public AlbumItemControl()
        {
            InitializeComponent();

            InfoButton.Click += InfoButton_Click;
        }

        public Album AlbumItem
        {
            get => (Album)GetValue(AlbumItemProperty);
            set => SetValue(AlbumItemProperty, value);
        }

        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        public Brush BorderBackground
        {
            get => (Brush)GetValue(BorderBackgroundProperty);
            set => SetValue(BorderBackgroundProperty, value);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new AlbumInfoWindow(AlbumItem);
            infoWindow.ShowDialog();
        }
    }
}
