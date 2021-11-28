using AIDMusicApp.Admin.Controls.Albums;
using AIDMusicApp.Admin.Controls.Genres;
using AIDMusicApp.Admin.Controls.Labels;
using AIDMusicApp.Admin.Controls.Musicians;
using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AIDMusicApp.Admin.Windows
{
    /// <summary>
    /// Логика взаимодействия для GroupsWindow.xaml
    /// </summary>
    public partial class GroupsWindow : Window
    {
        public Group GroupItem { get; private set; } = null;

        public GroupsWindow()
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            YearOfCreationText.PreviewTextInput += YearText_PreviewTextInput;
            YearOfBreakupText.PreviewTextInput += YearText_PreviewTextInput;
            TitleText.Text = "Добавление Группы";
            AddPanel.Visibility = Visibility.Visible;

            CountryId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            LoadImage.Click += LoadImage_Click;
            AddGenre.Click += AddGenre_Click;
            AddLabel.Click += AddLabel_Click;
            AddMember.Click += AddMember_Click;
            ConfirmButton.Click += AddButton_Click;

            foreach (var country in SqlDatabase.Instance.CountriesListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = country.Name, Tag = country };
                CountryId.Items.Add(item);
            }
        }

        public GroupsWindow(Group group)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            TitleText.Text = "Редактирование Группы";

            SaveGeneralButton.Visibility = Visibility.Visible;
            SaveGeneralButton.Click += SaveGeneralButton_Click;

            SaveGenresButton.Visibility = Visibility.Visible;
            SaveGenresButton.Click += SaveGenresButton_Click;

            SaveLabelsButton.Visibility = Visibility.Visible;
            SaveLabelsButton.Click += SaveLabelsButton_Click;

            SaveMembersButton.Visibility = Visibility.Visible;
            SaveMembersButton.Click += SaveMembersButton_Click;

            AlbumsPanel.Visibility = Visibility.Visible;

            CountryId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            GroupItem = group;
            NameText.Text = GroupItem.Name;
            YearOfCreationText.Text = GroupItem.YearOfCreation.ToString();
            YearOfBreakupText.Text = GroupItem.YearOfBreakup.ToString();
            DescriptionText.Text = GroupItem.Description;

            LoadImage.Click += LoadImage_Click;
            AddGenre.Click += AddGenre_Click;
            AddLabel.Click += AddLabel_Click;
            AddMember.Click += AddMember_Click;
            AddAlbum.Click += AddAlbum_Click;

            foreach (var country in SqlDatabase.Instance.CountriesListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = country.Name, Tag = country };
                CountryId.Items.Add(item);
                if (GroupItem.CountryId.Id == country.Id)
                    item.IsSelected = true;
            }

            foreach (var genre in GroupItem.Genres)
            {
                var item = new GenreBoxItemControl(genre.Id);
                GenreItems.Children.Insert(GenreItems.Children.Count - 1, item);
            }

            foreach (var label in GroupItem.Labels)
            {
                var item = new LabelBoxItemControl(label.Id);
                LabelItems.Children.Insert(LabelItems.Children.Count - 1, item);
            }

            foreach (var member in GroupItem.Members)
            {
                var item = new MusicianBoxItemControl(member.Id, member.IsFormer);
                MemberItems.Children.Insert(MemberItems.Children.Count - 1, item);
            }

            foreach (var album in GroupItem.Albums)
            {
                var item = new AlbumItemControl(album, GroupItem.Albums);
                AlbumItems.Children.Add(item);
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(GroupItem.Logo);
            image.EndInit();
            LogoImage.ImageSource = image;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void YearText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !byte.TryParse(e.Text, out _);
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
                LogoImage.ImageSource = image;
            }
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (GenreItems.Children.Count != 1)
            {
                var genreBoxItemControl = GenreItems.Children[GenreItems.Children.Count - 2] as GenreBoxItemControl;
                if (genreBoxItemControl.GenreItem == null)
                    return;
            }

            var item = new GenreBoxItemControl();
            GenreItems.Children.Insert(GenreItems.Children.Count - 1, item);
        }

        private void AddLabel_Click(object sender, RoutedEventArgs e)
        {
            if (LabelItems.Children.Count != 1)
            {
                var labelBoxItemControl = LabelItems.Children[LabelItems.Children.Count - 2] as LabelBoxItemControl;
                if (labelBoxItemControl.LabelItem == null)
                    return;
            }

            var item = new LabelBoxItemControl();
            LabelItems.Children.Insert(LabelItems.Children.Count - 1, item);
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            if (MemberItems.Children.Count != 1)
            {
                var musicianBoxItemControl = MemberItems.Children[MemberItems.Children.Count - 2] as MusicianBoxItemControl;
                if (musicianBoxItemControl.MusicianItem == null)
                    return;
            }

            var item = new MusicianBoxItemControl();
            MemberItems.Children.Insert(MemberItems.Children.Count - 1, item);
        }

        private void AddAlbum_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AlbumsWindow(GroupItem.Id, GroupItem.Genres);
            if (addWindow.ShowDialog() == true)
            {
                GroupItem.Albums.Add(addWindow.AlbumItem);
                var item = new AlbumItemControl(addWindow.AlbumItem, GroupItem.Albums);
                AlbumItems.Children.Add(item);
            }
        }

        private bool CheckFieldInputs()
        {
            if (string.IsNullOrWhiteSpace(NameText.Text))
            {
                AIDMessageWindow.Show("Поле \"Имя\" должно быть заполнено!");
                return false;
            }

            if (CountryId.SelectedIndex == -1)
            {
                AIDMessageWindow.Show("Поле \"Страна\" должно быть заполнено!");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(YearOfCreationText.Text))
            {
                if (short.TryParse(YearOfCreationText.Text, out short res))
                {
                    if (res > 2021 || res < 1900)
                    {
                        AIDMessageWindow.Show("Поле \"Год основания\" должно быть в диапазоне от 1900 до 2021!");
                        return false;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(YearOfBreakupText.Text))
            {
                if (short.TryParse(YearOfCreationText.Text, out short res))
                {
                    if (res > 2021 || res < 1900)
                    {
                        AIDMessageWindow.Show("Поле \"Год распада\" должно быть в диапазоне от 1900 до 2021!");
                        return false;
                    }
                }
            }

            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < GenreItems.Children.Count - 1; j++)
                {
                    if ((GenreItems.Children[i] as GenreBoxItemControl).GenreItem.Id == (GenreItems.Children[j] as GenreBoxItemControl).GenreItem.Id)
                    {
                        AIDMessageWindow.Show("Жанры не должны повторяться!");
                        return false;
                    }
                }
            }

            for (var i = 0; i < LabelItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < LabelItems.Children.Count - 1; j++)
                {
                    if ((LabelItems.Children[i] as LabelBoxItemControl).LabelItem.Id == (LabelItems.Children[j] as LabelBoxItemControl).LabelItem.Id)
                    {
                        AIDMessageWindow.Show("Лэйблы не должны повторяться!");
                        return false;
                    }
                }
            }

            for (var i = 0; i < MemberItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < MemberItems.Children.Count - 1; j++)
                {
                    if ((MemberItems.Children[i] as MusicianBoxItemControl).MusicianItem.Id == (MemberItems.Children[j] as MusicianBoxItemControl).MusicianItem.Id)
                    {
                        AIDMessageWindow.Show("Участники не должны повторяться!");
                        return false;
                    }
                }
            }

            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFieldInputs())
                return;

            var countryId = (Country)(CountryId.SelectedItem as ComboBoxItem).Tag;
            var stream = (LogoImage.ImageSource as BitmapImage).StreamSource;
            byte[] logo;
            if (stream == null)
            {
                var image = (BitmapImage)Application.Current.Resources["DefaultCover"];
                var resStream = Application.GetResourceStream(image.UriSource).Stream;
                logo = new byte[resStream.Length];
                resStream.Read(logo, 0, logo.Length);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                logo = new byte[stream.Length];
                stream.Read(logo, 0, logo.Length);
            }

            GroupItem = SqlDatabase.Instance.GroupsAdapter.Insert(NameText.Text, DescriptionText.Text,
                !string.IsNullOrWhiteSpace(YearOfCreationText.Text) ? Convert.ToInt16(YearOfCreationText.Text) : null,
                !string.IsNullOrWhiteSpace(YearOfBreakupText.Text) ? Convert.ToInt16(YearOfBreakupText.Text) : null,
                countryId.Id, logo);

            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                var genre = (GenreItems.Children[i] as GenreBoxItemControl).GenreItem;
                GroupItem.Genres.Add(genre);
                SqlDatabase.Instance.GroupGenresAdapter.Insert(GroupItem.Id, genre.Id);
            }

            for (var i = 0; i < LabelItems.Children.Count - 1; i++)
            {
                var label = (LabelItems.Children[i] as LabelBoxItemControl).LabelItem;
                GroupItem.Labels.Add(label);
                SqlDatabase.Instance.ContractsAdapter.Insert(GroupItem.Id, label.Id);
            }

            for (var i = 0; i < MemberItems.Children.Count - 1; i++)
            {
                var musician = (MemberItems.Children[i] as MusicianBoxItemControl).MusicianItem;
                musician.IsFormer = (MemberItems.Children[i] as MusicianBoxItemControl).IsFormer;
                GroupItem.Members.Add(musician);
                SqlDatabase.Instance.MembersAdapter.Insert(GroupItem.Id, musician.Id, musician.IsFormer);
            }

            DialogResult = true;
        }

        private void SaveGeneralButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameText.Text))
            {
                AIDMessageWindow.Show("Поле \"Имя\" должно быть заполнено!");
                return;
            }

            if (CountryId.SelectedIndex == -1)
            {
                AIDMessageWindow.Show("Поле \"Страна\" должно быть заполнено!");
                return;
            }

            if (!string.IsNullOrWhiteSpace(YearOfCreationText.Text))
            {
                if (short.TryParse(YearOfCreationText.Text, out short res))
                {
                    if (res > 2021 || res < 1900)
                    {
                        AIDMessageWindow.Show("Поле \"Год основания\" должно быть в диапазоне от 1900 до 2021!");
                        return;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(YearOfBreakupText.Text))
            {
                if (short.TryParse(YearOfCreationText.Text, out short res))
                {
                    if (res > 2021 || res < 1900)
                    {
                        AIDMessageWindow.Show("Поле \"Год распада\" должно быть в диапазоне от 1900 до 2021!");
                        return;
                    }
                }
            }

            var countryId = (Country)(CountryId.SelectedItem as ComboBoxItem).Tag;
            var stream = (LogoImage.ImageSource as BitmapImage).StreamSource;
            byte[] logo;
            if (stream == null)
            {
                var image = (BitmapImage)Application.Current.Resources["DefaultCover"];
                var resStream = Application.GetResourceStream(image.UriSource).Stream;
                logo = new byte[resStream.Length];
                resStream.Read(logo, 0, logo.Length);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                logo = new byte[stream.Length];
                stream.Read(logo, 0, logo.Length);
            }

            GroupItem.Update(NameText.Text, DescriptionText.Text,
                !string.IsNullOrWhiteSpace(YearOfCreationText.Text) ? Convert.ToInt16(YearOfCreationText.Text) : null,
                !string.IsNullOrWhiteSpace(YearOfBreakupText.Text) ? Convert.ToInt16(YearOfBreakupText.Text) : null,
                countryId, logo);

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void SaveGenresButton_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < GenreItems.Children.Count - 1; j++)
                {
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

            SqlDatabase.Instance.GroupGenresAdapter.Update(GroupItem.Genres.ToList(), genres, GroupItem.Id);

            GroupItem.Genres.Clear();
            foreach (var genre in genres)
                GroupItem.Genres.Add(genre);

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void SaveLabelsButton_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < LabelItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < LabelItems.Children.Count - 1; j++)
                {
                    if ((LabelItems.Children[i] as LabelBoxItemControl).LabelItem.Id == (LabelItems.Children[j] as LabelBoxItemControl).LabelItem.Id)
                    {
                        AIDMessageWindow.Show("Лэйблы не должны повторяться!");
                        return;
                    }
                }
            }

            var labels = new List<Models.Label>();
            for (var i = 0; i < LabelItems.Children.Count - 1; i++)
            {
                var label = (LabelItems.Children[i] as LabelBoxItemControl).LabelItem;
                labels.Add(label);
            }

            SqlDatabase.Instance.ContractsAdapter.Update(GroupItem.Labels.ToList(), labels, GroupItem.Id);

            GroupItem.Labels.Clear();
            foreach (var label in labels)
                GroupItem.Labels.Add(label);

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void SaveMembersButton_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < MemberItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < MemberItems.Children.Count - 1; j++)
                {
                    if ((MemberItems.Children[i] as MusicianBoxItemControl).MusicianItem.Id == (MemberItems.Children[j] as MusicianBoxItemControl).MusicianItem.Id)
                    {
                        AIDMessageWindow.Show("Участники не должны повторяться!");
                        return;
                    }
                }
            }

            var members = new List<Musician>();
            for (var i = 0; i < MemberItems.Children.Count - 1; i++)
            {
                var musician = (MemberItems.Children[i] as MusicianBoxItemControl).MusicianItem;
                musician.IsFormer = (MemberItems.Children[i] as MusicianBoxItemControl).IsFormer;
                members.Add(musician);
            }

            SqlDatabase.Instance.MembersAdapter.Update(GroupItem.Members.ToList(), members, GroupItem.Id);

            GroupItem.Members.Clear();
            foreach (var member in members)
                GroupItem.Members.Add(member);

            AIDMessageWindow.Show("Сохранено успешно!");
        }
    }
}
