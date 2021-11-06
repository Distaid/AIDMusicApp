﻿using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AIDMusicApp.Admin.Windows
{
    /// <summary>
    /// Логика взаимодействия для UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        public User UserItem = null;

        public UsersWindow()
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            LoadImage.Click += LoadImage_Click;

            AccessId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            TitleText.Text = "Добавление Пользователя";
            ConfirmButton.Content = "Добавить";
            ConfirmButton.Click += AddButton_Click;

            foreach (var access in SqlDatabase.Instance.AccessAdapter.GetAll())
            {
                if (access.Name == "Администратор")
                    continue;
                var item = new ComboBoxItem();
                item.Content = access.Name;
                item.Tag = access;
                AccessId.Items.Add(item);
            }
            AccessId.SelectedIndex = 0;
        }

        public UsersWindow(User user)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            LoadImage.Click += LoadImage_Click;

            AccessId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            UserItem = user;
            TitleText.Text = "Изменение Пользователя";
            ConfirmButton.Content = "Изменить";
            ConfirmButton.Click += EditButton_Click;

            LoginText.Text = UserItem.Login;
            PasswordText.Password = UserItem.Password;
            PhoneText.Text = UserItem.Phone;
            EmailText.Text = UserItem.Email;
            if (UserItem.AccessId.Name == "Администратор")
                AccessId.IsEnabled = false;

            foreach (var access in SqlDatabase.Instance.AccessAdapter.GetAll())
            {
                var item = new ComboBoxItem();
                item.Content = access.Name;
                item.Tag = access;
                if (access.Name == "Администратор")
                    item.IsEnabled = false;
                if (access.Id == UserItem.AccessId.Id)
                    item.IsSelected = true;
                AccessId.Items.Add(item);
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(UserItem.Avatar);
            image.EndInit();
            AvatarImage.ImageSource = image;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
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
                AvatarImage.ImageSource = image;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginText.Text))
            {
                AIDMessageWindow.Show("Поле \"Логин\" обязательно для заполнения!");
                LoginText.Focus();
                return;
            }

            if (SqlDatabase.Instance.UsersAdapter.ContainsLogin(LoginText.Text))
            {
                AIDMessageWindow.Show("Пользователь с таким логином уже существует!");
                LoginText.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordText.Password))
            {
                AIDMessageWindow.Show("Поле \"Пароль\" обязательно для заполнения!");
                PasswordText.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(PhoneText.Text))
            {
                if (!Regex.IsMatch(PhoneText.Text, @"\+375\d{9}"))
                {
                    AIDMessageWindow.Show("Поле \"Номер\" не соответствует шаблону!");
                    PhoneText.Focus();
                    return;
                }

                if (SqlDatabase.Instance.UsersAdapter.ContainsPhone(PhoneText.Text))
                {
                    AIDMessageWindow.Show("Пользователь с таким номером телефона уже существует!");
                    PhoneText.Focus();
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(EmailText.Text))
            {
                if (!Regex.IsMatch(EmailText.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,}$"))
                {
                    AIDMessageWindow.Show("Поле \"Почта\" не соответствует шаблону!");
                    EmailText.Focus();
                    return;
                }

                if (SqlDatabase.Instance.UsersAdapter.ContainsEmail(EmailText.Text))
                {
                    AIDMessageWindow.Show("Пользователь с такой почтой уже существует!");
                    EmailText.Focus();
                    return;
                }
            }

            var accessId = (Access)(AccessId.SelectedItem as ComboBoxItem).Tag;
            var stream = (AvatarImage.ImageSource as BitmapImage).StreamSource;
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

            UserItem = SqlDatabase.Instance.UsersAdapter.Insert(LoginText.Text, PasswordText.Password, PhoneText.Text, EmailText.Text, accessId.Id, avatar);

            DialogResult = true;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginText.Text))
            {
                AIDMessageWindow.Show("Поле \"Логин\" обязательно для заполнения!");
                LoginText.Focus();
                return;
            }

            if (LoginText.Text != UserItem.Login)
            {
                if (SqlDatabase.Instance.UsersAdapter.ContainsLogin(LoginText.Text))
                {
                    AIDMessageWindow.Show("Пользователь с таким логином уже существует!");
                    LoginText.Focus();
                    return;
                }
            }

            if (PhoneText.Text != UserItem.Phone)
            {
                if (!Regex.IsMatch(PhoneText.Text, @"\+375\d{9}"))
                {
                    AIDMessageWindow.Show("Поле \"Номер\" не соответствует шаблону!");
                    PhoneText.Focus();
                    return;
                }

                if (SqlDatabase.Instance.UsersAdapter.ContainsPhone(PhoneText.Text))
                {
                    AIDMessageWindow.Show("Пользователь с таким номером телефона уже существует!");
                    PhoneText.Focus();
                    return;
                }
            }

            if (EmailText.Text != UserItem.Email)
            {
                if (!Regex.IsMatch(EmailText.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,}$"))
                {
                    AIDMessageWindow.Show("Поле \"Почта\" не соответствует шаблону!");
                    EmailText.Focus();
                    return;
                }

                if (SqlDatabase.Instance.UsersAdapter.ContainsEmail(EmailText.Text))
                {
                    AIDMessageWindow.Show("Пользователь с такой почтой уже существует!");
                    EmailText.Focus();
                    return;
                }
            }

            var accessId = (Access)(AccessId.SelectedItem as ComboBoxItem).Tag;
            var stream = (AvatarImage.ImageSource as BitmapImage).StreamSource;
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

            UserItem.Update(LoginText.Text, PasswordText.Password, PhoneText.Text, EmailText.Text, accessId, avatar);

            DialogResult = true;
        }
    }
}
