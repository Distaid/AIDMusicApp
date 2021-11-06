using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace AIDMusicApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoadingControl.xaml
    /// </summary>
    public partial class LoadingControl : UserControl
    {
        public LoadingControl()
        {
            InitializeComponent();
            //var image = new BitmapImage(new System.Uri("/AIDMusicApp;component/Images/LoadingIcon.gif"));
            //ImageBehavior.SetAnimatedSource(LoadImage, image);
        }
    }
}
