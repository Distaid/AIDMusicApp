using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для UserPageControl.xaml
    /// </summary>
    public partial class UserPageControl : UserControl
    {
        private User _parentUser = null;

        public UserPageControl(User user)
        {
            InitializeComponent();

            LoadImage.Click += LoadImage_Click;
            EditButton.Click += EditButton_Click;
            SaveButton.Click += SaveButton_Click;
            CancelButton.Click += CancelButton_Click;

            _parentUser = user;
            _userItem.Id = user.Id;
            _userItem.Login = user.Login;
            _userItem.Password = user.Password;
            _userItem.Phone = user.Phone;
            _userItem.Email = user.Email;
            _userItem.Avatar = user.Avatar;
            _userItem.AccessId = user.AccessId;
        }

        private User _userItem => (User)Resources["UserItem"];

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ViewBlock.Visibility = Visibility.Collapsed;
            EditBlock.Visibility = Visibility.Visible;

            LoginText.Text = _userItem.Login;
            PasswordText.Password = _userItem.Password;
            PhoneText.Text = _userItem.Phone;
            EmailText.Text = _userItem.Email;

            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(_userItem.Avatar);
            image.EndInit();
            AvatarImagePrev.ImageSource = image;
        }

        private bool CheckFields()
        {
            if (string.IsNullOrWhiteSpace(LoginText.Text))
            {
                AIDMessageWindow.Show("Поле \"Логин\" обязательно для заполнения!");
                LoginText.Focus();
                return false;
            }

            if (_userItem.Login != LoginText.Text)
            {
                if (SqlDatabase.Instance.UsersAdapter.ContainsLogin(LoginText.Text))
                {
                    AIDMessageWindow.Show("Пользователь с таким логином уже существует!");
                    LoginText.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(PasswordText.Password))
            {
                AIDMessageWindow.Show("Поле \"Пароль\" обязательно для заполнения!");
                PasswordText.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(PhoneText.Text))
            {
                if (!Regex.IsMatch(PhoneText.Text, @"\+375\d{9}"))
                {
                    AIDMessageWindow.Show("Поле \"Номер\" не соответствует шаблону!");
                    PhoneText.Focus();
                    return false;
                }

                if (_userItem.Phone != PhoneText.Text)
                {
                    if (SqlDatabase.Instance.UsersAdapter.ContainsPhone(PhoneText.Text))
                    {
                        AIDMessageWindow.Show("Пользователь с таким номером телефона уже существует!");
                        PhoneText.Focus();
                        return false;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(EmailText.Text))
            {
                if (!Regex.IsMatch(EmailText.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,}$"))
                {
                    AIDMessageWindow.Show("Поле \"Почта\" не соответствует шаблону!");
                    EmailText.Focus();
                    return false;
                }
                if (_userItem.Email != EmailText.Text)
                {
                    if (SqlDatabase.Instance.UsersAdapter.ContainsEmail(EmailText.Text))
                    {
                        AIDMessageWindow.Show("Пользователь с такой почтой уже существует!");
                        EmailText.Focus();
                        return false;
                    }
                }
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            var stream = (AvatarImagePrev.ImageSource as BitmapImage).StreamSource;
            byte[] avatar;
            if (stream == null)
            {
                var image = (BitmapImage)Application.Current.Resources["DefaultImage"];
                var resStream = Application.GetResourceStream(image.UriSource).Stream;
                avatar = new byte[resStream.Length];
                resStream.Read(avatar, 0, avatar.Length);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                avatar = new byte[stream.Length];
                stream.Read(avatar, 0, avatar.Length);
            }

            _userItem.Update(LoginText.Text, PasswordText.Password, PhoneText.Text, EmailText.Text, _userItem.AccessId, avatar);
            _parentUser.Avatar = _userItem.Avatar;

            ViewBlock.Visibility = Visibility.Visible;
            EditBlock.Visibility = Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewBlock.Visibility = Visibility.Visible;
            EditBlock.Visibility = Visibility.Collapsed;
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "PNG|*.png|JPG|*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = new MemoryStream(File.ReadAllBytes(dialog.FileName));
                image.EndInit();
                AvatarImagePrev.ImageSource = image;
            }
        }
    }
}
