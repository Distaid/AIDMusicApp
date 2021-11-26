using AIDMusicApp.Models;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Albums
{
    /// <summary>
    /// Логика взаимодействия для AlbumGroupItemControl.xaml
    /// </summary>
    public partial class AlbumGroupItemControl : UserControl
    {
        public static readonly DependencyProperty AlbumItemProperty;

        static AlbumGroupItemControl()
        {
            AlbumItemProperty = DependencyProperty.Register("AlbumItem", typeof(Album), typeof(AlbumGroupItemControl));
        }

        public AlbumGroupItemControl()
        {
            InitializeComponent();
        }

        public Album AlbumItem
        {
            get => (Album)GetValue(AlbumItemProperty);
            set => SetValue(AlbumItemProperty, value);
        }
    }
}
