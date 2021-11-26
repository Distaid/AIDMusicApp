using AIDMusicApp.Admin.Controls.Musicians;
using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AIDMusicApp.Admin.Windows
{
    /// <summary>
    /// Логика взаимодействия для SongsWindow.xaml
    /// </summary>
    public partial class SongsWindow : Window
    {
        private int _albumId = 0;

        public Song SongItem { get; private set; } = null;

        public SongsWindow(int albumId)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            TitleText.Text = "Добавление Песни";
            ConfirmButton.Content = "Добавить";
            _albumId = albumId;

            AddMusician.Click += AddMusician_Click;

            ConfirmButton.Click += AddButton_Click;
        }

        public SongsWindow(Song song)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            TitleText.Text = "Редактирование Песни";
            ConfirmButton.Content = "Изменить";

            AddMusician.Click += AddMusician_Click;

            ConfirmButton.Click += EditButton_Click;

            SongItem = song;
            NameText.Text = SongItem.Name;
            TimeText.Text = SongItem.Time;

            foreach (var musician in song.Guests)
            {
                var item = new MusicianBoxItemControl(musician.Id);
                MusiciansItems.Children.Insert(MusiciansItems.Children.Count - 1, item);
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void AddMusician_Click(object sender, RoutedEventArgs e)
        {
            if (MusiciansItems.Children.Count != 1)
            {
                var musicianItemControl = MusiciansItems.Children[MusiciansItems.Children.Count - 2] as MusicianBoxItemControl;
                if (musicianItemControl.MusicianItem == null)
                    return;
            }

            var item = new MusicianBoxItemControl(false);
            MusiciansItems.Children.Insert(MusiciansItems.Children.Count - 1, item);
        }

        private bool CheckFields()
        {
            if (string.IsNullOrWhiteSpace(NameText.Text))
            {
                AIDMessageWindow.Show("Поле \"Название\" должно быть заполнено!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TimeText.Text))
            {
                AIDMessageWindow.Show("Поле \"Время\" должно быть заполнено!");
                return false;
            }
            else
            {
                if (!Regex.IsMatch(TimeText.Text, @"^[1-6]?[0-9]:[0-6][0-9]$"))
                {
                    AIDMessageWindow.Show("Поле \"Время\" должно быть в формате мм:сс");
                    return false;
                }
            }

            for (var i = 0; i < MusiciansItems.Children.Count - 1; i++)
            {
                if ((MusiciansItems.Children[i] as MusicianBoxItemControl).MusicianItem == null)
                {
                    AIDMessageWindow.Show("Гостевые участники не должны быть пустыми!");
                    return false;
                }

                for (var j = i + 1; j < MusiciansItems.Children.Count - 1; j++)
                {
                    if ((MusiciansItems.Children[j] as MusicianBoxItemControl).MusicianItem == null)
                    {
                        AIDMessageWindow.Show("Гостевые участники не должны быть пустыми!");
                        return false;
                    }

                    if ((MusiciansItems.Children[i] as MusicianBoxItemControl).MusicianItem.Id == (MusiciansItems.Children[j] as MusicianBoxItemControl).MusicianItem.Id)
                    {
                        AIDMessageWindow.Show("Гостевые участники не должны повторяться!");
                        return false;
                    }
                }
            }

            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            SongItem = SqlDatabase.Instance.SongsAdapter.Insert(NameText.Text, TimeText.Text);

            for (var i = 0; i < MusiciansItems.Children.Count - 1; i++)
            {
                var musician = (MusiciansItems.Children[i] as MusicianBoxItemControl).MusicianItem;
                SongItem.Guests.Add(musician);
                SqlDatabase.Instance.FeatsAdapter.Insert(SongItem.Id, musician.Id);
            }

            SqlDatabase.Instance.TrackListAdapter.Insert(SongItem.Id, _albumId);

            DialogResult = true;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            SongItem.Update(NameText.Text, TimeText.Text);

            var musicians = new List<Musician>();
            for (var i = 0; i < MusiciansItems.Children.Count - 1; i++)
            {
                var musician = (MusiciansItems.Children[i] as MusicianBoxItemControl).MusicianItem;
                musicians.Add(musician);
            }

            SqlDatabase.Instance.FeatsAdapter.Update(SongItem.Guests.ToList(), musicians, SongItem.Id);

            SongItem.Guests.Clear();
            foreach (var musician in musicians)
                SongItem.Guests.Add(musician);

            DialogResult = true;
        }
    }
}
