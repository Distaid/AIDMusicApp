﻿using AIDMusicApp.Admin.Windows;
using AIDMusicApp.Models;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.Admin.Controls.Users
{
    /// <summary>
    /// Логика взаимодействия для UserItemControl.xaml
    /// </summary>
    public partial class UserItemControl : UserControl
    {
        //public User UserItem { get; private set; }

        public UserItemControl(User user)
        {
            InitializeComponent();

            UserItem.Id = user.Id;
            UserItem.Login = user.Login;
            UserItem.Password = user.Password;
            UserItem.Email = user.Email;
            UserItem.Phone = user.Phone;
            UserItem.AccessId = user.AccessId;
            UserItem.Avatar = user.Avatar;

            if (UserItem.AccessId.Name == "Администратор")
                RemoveButton.IsEnabled = false;

            EditButton.Click += EditButton_Click;
            RemoveButton.Click += RemoveButton_Click;
        }

        public User UserItem => (User)Resources["UserItem"];

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new UsersWindow(UserItem);
            if (editWindow.ShowDialog() == true)
            {
                UserItem.Login = editWindow.UserItem.Login;
                UserItem.Password = editWindow.UserItem.Login;
                UserItem.Email = editWindow.UserItem.Email;
                UserItem.Phone = editWindow.UserItem.Phone;
                UserItem.AccessId = editWindow.UserItem.AccessId;
                UserItem.Avatar = editWindow.UserItem.Avatar;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            UserItem.Delete();
            (Parent as WrapPanel).Children.Remove(this);
        }
    }
}
