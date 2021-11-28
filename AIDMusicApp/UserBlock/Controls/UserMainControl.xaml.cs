using AIDMusicApp.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserMainControl.xaml
    /// </summary>
    public partial class UserMainControl : UserControl
    {
        public event Action ExitClick = null;

        private ToggleButton _last = null;

        public UserMainControl(User user)
        {
            InitializeComponent();

            ExitButton.Click += ExitButton_Click;

            MainPage.Checked += Page_Checked;
            MainPage.Unchecked += Page_Unchecked;

            GroupsPage.Checked += Page_Checked;
            GroupsPage.Unchecked += Page_Unchecked;

            AlbumsPage.Checked += Page_Checked;
            AlbumsPage.Unchecked += Page_Unchecked;

            MusiciansPage.Checked += Page_Checked;
            MusiciansPage.Unchecked += Page_Unchecked;

            UserPage.Checked += Page_Checked;
            UserPage.Unchecked += Page_Unchecked;

            MainPage.IsChecked = true;

            _userItem.Id = user.Id;
            _userItem.Login = user.Login;
            _userItem.Password = user.Password;
            _userItem.Phone = user.Phone;
            _userItem.Email = user.Email;
            _userItem.Avatar = user.Avatar;
            _userItem.AccessId = user.AccessId;
        }

        private User _userItem => (User)Resources["UserItem"];

        private void Page_Checked(object sender, RoutedEventArgs e)
        {
            if (_last == null)
            {
                _last = sender as ToggleButton;
                _last.IsChecked = true;
                LoadPanel(_last.Name);
                return;
            }

            if (_last.Name == (sender as ToggleButton).Name)
                return;

            _last.IsChecked = false;
            _last = sender as ToggleButton;
            _last.IsChecked = true;
            LoadPanel(_last.Name);
        }

        private void Page_Unchecked(object sender, RoutedEventArgs e)
        {
            _last = null;
            MainContent.Content = null;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitClick?.Invoke();
        }

        private void LoadPanel(string name)
        {
            MainContent.Content = null;

            switch (name)
            {
                case "MainPage":
                    MainContent.Content = new MainPageControl();
                    break;

                case "GroupsPage":
                    MainContent.Content = new GroupsPageControl();
                    break;

                case "AlbumsPage":
                    MainContent.Content = new AlbumsPageControl();
                    break;

                case "MusiciansPage":
                    MainContent.Content = new MusiciansPageControl();
                    break;

                case "UserPage":
                    MainContent.Content = new UserPageControl(_userItem);
                    break;
            }
        }
    }
}
