using AIDMusicApp.Models;
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
