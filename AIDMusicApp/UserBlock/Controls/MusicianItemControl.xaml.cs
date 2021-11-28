using AIDMusicApp.Models;
using AIDMusicApp.UserBlock.Windows;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для MusicianItemControl.xaml
    /// </summary>
    public partial class MusicianItemControl : UserControl
    {
        public static readonly DependencyProperty MusicianItemProperty;

        static MusicianItemControl()
        {
            MusicianItemProperty = DependencyProperty.Register("MusicianItem", typeof(Musician), typeof(MusicianItemControl));
        }

        public MusicianItemControl()
        {
            InitializeComponent();

            InfoButton.Click += InfoButton_Click;
        }

        public Musician MusicianItem
        {
            get => (Musician)GetValue(MusicianItemProperty);
            set => SetValue(MusicianItemProperty, value);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new MusicianInfoWindow(MusicianItem);
            infoWindow.ShowDialog();
        }
    }
}
