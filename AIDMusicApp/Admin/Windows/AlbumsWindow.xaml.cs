using AIDMusicApp.Admin.Controls.Formats;
using AIDMusicApp.Admin.Controls.Genres;
using AIDMusicApp.Admin.Controls.Songs;
using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AIDMusicApp.Admin.Windows
{
    /// <summary>
    /// Логика взаимодействия для AlbumsWindow.xaml
    /// </summary>
    public partial class AlbumsWindow : Window
    {
        private int _groupId = 0;

        public Album AlbumItem { get; private set; } = null;

        public AlbumsWindow(int groupId, IEnumerable<Genre> genres)
        {
            InitializeComponent();

            AddPanel.Visibility = Visibility.Visible;
            SongsPanel.Visibility = Visibility.Collapsed;

            TitleBar.MouseDown += TitleBar_MouseDown;
            LoadImage.Click += LoadImage_Click;
            YearText.PreviewTextInput += YearText_PreviewTextInput;
            AddGenre.Click += AddGenre_Click;
            AddFormat.Click += AddFormat_Click;
            ConfirmButton.Click += AddButton_Click;

            TitleText.Text = "Добавление Альбома";
            _groupId = groupId;

            foreach (var genre in genres)
            {
                var item = new GenreBoxItemControl(genre.Id);
                GenreItems.Children.Insert(GenreItems.Children.Count - 1, item);
            }
        }

        public AlbumsWindow(Album album)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            LoadImage.Click += LoadImage_Click;
            YearText.PreviewTextInput += YearText_PreviewTextInput;
            AddGenre.Click += AddGenre_Click;
            AddFormat.Click += AddFormat_Click;
            AddSong.Click += AddSong_Click;

            TitleText.Text = "Редактирование Альбома";

            SaveGeneralButton.Visibility = Visibility.Visible;
            SaveGeneralButton.Click += SaveGeneralButton_Click;

            SaveGenresButton.Visibility = Visibility.Visible;
            SaveGenresButton.Click += SaveGenresButton_Click;

            SaveFormatsButton.Visibility = Visibility.Visible;
            SaveFormatsButton.Click += SaveFormatsButton_Click;

            AlbumItem = album;
            NameText.Text = AlbumItem.Name;
            YearText.Text = AlbumItem.Year.ToString();
            DescriptionText.Text = AlbumItem.Description;

            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(AlbumItem.Cover);
            image.EndInit();
            CoverImage.ImageSource = image;

            foreach (var genre in AlbumItem.Genres)
            {
                var item = new GenreBoxItemControl(genre.Id);
                GenreItems.Children.Insert(GenreItems.Children.Count - 1, item);
            }

            foreach (var format in AlbumItem.Formats)
            {
                var item = new FormatBoxItemControl(format.Id);
                FormatItems.Children.Insert(FormatItems.Children.Count - 1, item);
            }

            foreach (var song in AlbumItem.Songs)
            {
                var item = new SongItemControl(song, AlbumItem.Songs);
                SongItems.Children.Add(item);
            }
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
                CoverImage.ImageSource = image;
            }
        }

        private void YearText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !byte.TryParse(e.Text, out _);
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (GenreItems.Children.Count != 1)
            {
                var genreItemControl = GenreItems.Children[GenreItems.Children.Count - 2] as GenreBoxItemControl;
                if (genreItemControl.GenreItem == null)
                    return;
            }

            var item = new GenreBoxItemControl();
            GenreItems.Children.Insert(GenreItems.Children.Count - 1, item);
        }

        private void AddFormat_Click(object sender, RoutedEventArgs e)
        {
            if (FormatItems.Children.Count != 1)
            {
                var albumFormatItemControl = FormatItems.Children[FormatItems.Children.Count - 2] as FormatBoxItemControl;
                if (albumFormatItemControl.FormatItem == null)
                    return;
            }

            var item = new FormatBoxItemControl();
            FormatItems.Children.Insert(FormatItems.Children.Count - 1, item);
        }

        private bool CheckFields()
        {
            if (string.IsNullOrWhiteSpace(NameText.Text))
            {
                AIDMessageWindow.Show("Поле \"Название\" должно быть заполнено!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(YearText.Text))
            {
                AIDMessageWindow.Show("Поле \"Год\" должно быть заполнено!");
                return false;
            }
            else
            {
                if (short.TryParse(YearText.Text, out short res))
                {
                    if (res > 2021 || res < 1900)
                    {
                        AIDMessageWindow.Show("Поле \"Год\" должно быть в диапазоне от 1900 до 2021!");
                        return false;
                    }
                }
            }

            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                if ((GenreItems.Children[i] as GenreBoxItemControl).GenreItem == null)
                {
                    AIDMessageWindow.Show("Жанр не должны быть пустыми!");
                    return false;
                }

                for (var j = i + 1; j < GenreItems.Children.Count - 1; j++)
                {
                    if ((GenreItems.Children[j] as GenreBoxItemControl).GenreItem == null)
                    {
                        AIDMessageWindow.Show("Жанр не должны быть пустыми!");
                        return false;
                    }

                    if ((GenreItems.Children[i] as GenreBoxItemControl).GenreItem.Id == (GenreItems.Children[j] as GenreBoxItemControl).GenreItem.Id)
                    {
                        AIDMessageWindow.Show("Жанры не должны повторяться!");
                        return false;
                    }
                }
            }

            for (var i = 0; i < FormatItems.Children.Count - 1; i++)
            {
                if ((FormatItems.Children[i] as FormatBoxItemControl).FormatItem == null)
                {
                    AIDMessageWindow.Show("Формат не должны быть пустыми!");
                    return false;
                }

                for (var j = i + 1; j < FormatItems.Children.Count - 1; j++)
                {
                    if ((FormatItems.Children[j] as FormatBoxItemControl).FormatItem == null)
                    {
                        AIDMessageWindow.Show("Формат не должны быть пустыми!");
                        return false;
                    }

                    if ((FormatItems.Children[i] as FormatBoxItemControl).FormatItem.Id == (FormatItems.Children[j] as FormatBoxItemControl).FormatItem.Id)
                    {
                        AIDMessageWindow.Show("Форматы не должны повторяться!");
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

            var stream = (CoverImage.ImageSource as BitmapImage).StreamSource;
            byte[] cover;
            if (stream == null)
            {
                var image = (BitmapImage)Application.Current.Resources["DefaultCover"];
                var resStream = Application.GetResourceStream(image.UriSource).Stream;
                cover = new byte[resStream.Length];
                resStream.Read(cover, 0, cover.Length);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                cover = new byte[stream.Length];
                stream.Read(cover, 0, cover.Length);
            }

            AlbumItem = SqlDatabase.Instance.AlbumsAdapter.Insert(NameText.Text, Convert.ToInt16(YearText.Text), DescriptionText.Text, cover);

            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                var genre = (GenreItems.Children[i] as GenreBoxItemControl).GenreItem;
                AlbumItem.Genres.Add(genre);
                SqlDatabase.Instance.AlbumGenresAdapter.Insert(AlbumItem.Id, genre.Id);
            }

            for (var i = 0; i < FormatItems.Children.Count - 1; i++)
            {
                var format = (FormatItems.Children[i] as FormatBoxItemControl).FormatItem;
                AlbumItem.Formats.Add(format);
                SqlDatabase.Instance.AlbumFormatsAdapter.Insert(AlbumItem.Id, format.Id);
            }

            SqlDatabase.Instance.DiscographyAdapter.Insert(AlbumItem.Id, _groupId);

            DialogResult = true;
        }

        private void SaveGeneralButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameText.Text))
            {
                AIDMessageWindow.Show("Поле \"Название\" должно быть заполнено!");
                return;
            }

            if (string.IsNullOrWhiteSpace(YearText.Text))
            {
                AIDMessageWindow.Show("Поле \"Год\" должно быть заполнено!");
                return;
            }
            else
            {
                if (short.TryParse(YearText.Text, out short res))
                {
                    if (res > 2021 || res < 1900)
                    {
                        AIDMessageWindow.Show("Поле \"Год\" должно быть в диапазоне от 1900 до 2021!");
                        return;
                    }
                }
            }

            var stream = (CoverImage.ImageSource as BitmapImage).StreamSource;
            byte[] cover;
            if (stream == null)
            {
                var image = (BitmapImage)Application.Current.Resources["DefaultCover"];
                var resStream = Application.GetResourceStream(image.UriSource).Stream;
                cover = new byte[resStream.Length];
                resStream.Read(cover, 0, cover.Length);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                cover = new byte[stream.Length];
                stream.Read(cover, 0, cover.Length);
            }

            AlbumItem.Update(NameText.Text, Convert.ToInt16(YearText.Text), DescriptionText.Text, cover);

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void SaveGenresButton_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                if ((GenreItems.Children[i] as GenreBoxItemControl).GenreItem == null)
                {
                    AIDMessageWindow.Show("Жанр не должны быть пустыми!");
                    return;
                }

                for (var j = i + 1; j < GenreItems.Children.Count - 1; j++)
                {
                    if ((GenreItems.Children[j] as GenreBoxItemControl).GenreItem == null)
                    {
                        AIDMessageWindow.Show("Жанр не должны быть пустыми!");
                        return;
                    }

                    if ((GenreItems.Children[i] as GenreBoxItemControl).GenreItem.Id == (GenreItems.Children[j] as GenreBoxItemControl).GenreItem.Id)
                    {
                        AIDMessageWindow.Show("Жанры не должны повторяться!");
                        return;
                    }
                }
            }

            var genres = new List<Genre>();
            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                var genre = (GenreItems.Children[i] as GenreBoxItemControl).GenreItem;
                genres.Add(genre);
            }

            SqlDatabase.Instance.AlbumGenresAdapter.Update(AlbumItem.Genres.ToList(), genres, AlbumItem.Id);

            AlbumItem.Genres.Clear();
            foreach (var genre in genres)
                AlbumItem.Genres.Add(genre);

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void SaveFormatsButton_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < FormatItems.Children.Count - 1; i++)
            {
                if ((FormatItems.Children[i] as FormatBoxItemControl).FormatItem == null)
                {
                    AIDMessageWindow.Show("Формат не должны быть пустыми!");
                    return;
                }

                for (var j = i + 1; j < FormatItems.Children.Count - 1; j++)
                {
                    if ((FormatItems.Children[j] as FormatBoxItemControl).FormatItem == null)
                    {
                        AIDMessageWindow.Show("Формат не должны быть пустыми!");
                        return;
                    }

                    if ((FormatItems.Children[i] as FormatBoxItemControl).FormatItem.Id == (FormatItems.Children[j] as FormatBoxItemControl).FormatItem.Id)
                    {
                        AIDMessageWindow.Show("Форматы не должны повторяться!");
                        return;
                    }
                }
            }

            var formats = new List<Format>();
            for (var i = 0; i < FormatItems.Children.Count - 1; i++)
            {
                var genre = (FormatItems.Children[i] as FormatBoxItemControl).FormatItem;
                formats.Add(genre);
            }

            SqlDatabase.Instance.AlbumFormatsAdapter.Update(AlbumItem.Formats.ToList(), formats, AlbumItem.Id);

            AlbumItem.Formats.Clear();
            foreach (var format in formats)
                AlbumItem.Formats.Add(format);

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void AddSong_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new SongsWindow(AlbumItem.Id);
            if (addWindow.ShowDialog() == true)
            {
                AlbumItem.Songs.Add(addWindow.SongItem);
                var item = new SongItemControl(addWindow.SongItem, AlbumItem.Songs);
                SongItems.Children.Add(item);
            }
        }
    }
}
