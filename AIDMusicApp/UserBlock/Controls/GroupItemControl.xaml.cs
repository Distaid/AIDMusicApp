using AIDMusicApp.Models;
using AIDMusicApp.UserBlock.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для GroupItemControl.xaml
    /// </summary>
    public partial class GroupItemControl : UserControl
    {
        public static readonly DependencyProperty GroupItemProperty;
        public static readonly DependencyProperty BorderBackgroundProperty;

        static GroupItemControl()
        {
            GroupItemProperty = DependencyProperty.Register("GroupItem", typeof(Group), typeof(GroupItemControl));
            BorderBackgroundProperty = DependencyProperty.Register("BorderBackground", typeof(Brush), typeof(GroupItemControl));
        }

        public GroupItemControl()
        {
            InitializeComponent();

            InfoButton.Click += InfoButton_Click;
        }

        public Group GroupItem
        {
            get => (Group)GetValue(GroupItemProperty);
            set => SetValue(GroupItemProperty, value);
        }

        public Brush BorderBackground
        {
            get => (Brush)GetValue(BorderBackgroundProperty);
            set => SetValue(BorderBackgroundProperty, value);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new GroupInfoWindow(GroupItem);
            infoWindow.ShowDialog();
        }
    }
}
