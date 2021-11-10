using AIDMusicApp.Admin.Controls.Genres;
using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            ConfirmButton.Content = "Добавить";

            CountryId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            AddGenre.Click += AddGenre_Click;
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
            ConfirmButton.Content = "Изменить";

            CountryId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            GroupItem = group;
            NameText.Text = GroupItem.Name;
            YearOfCreationText.Text = GroupItem.YearOfCreation.ToString();
            YearOfBreakupText.Text = GroupItem.YearOfBreakup.ToString();
            DescriptionText.Text = GroupItem.Description;

            AddGenre.Click += AddGenre_Click;
            ConfirmButton.Click += ChangeButton_Click;

            foreach (var country in SqlDatabase.Instance.CountriesListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = country.Name, Tag = country };
                CountryId.Items.Add(item);
                if (GroupItem.CountryId.Id == country.Id)
                    item.IsSelected = true;
            }

            foreach (var genre in SqlDatabase.Instance.GroupGenresAdapter.GetGenresByGroupId(GroupItem.Id))
            {
                var item = new GenreBoxItemControl(genre.Id);
                GenreItems.Children.Insert(GenreItems.Children.Count - 1, item);
            }
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

        private bool CheckFields()
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

            if (GenreItems.Children.Count == 1)
            {
                AIDMessageWindow.Show("Поле \"Жанры\" должно содержать хотя бы один элемент!");
                return false;
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

            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            var countryId = (Country)(CountryId.SelectedItem as ComboBoxItem).Tag;

            GroupItem = SqlDatabase.Instance.GroupsAdapter.Insert(NameText.Text, DescriptionText.Text,
                !string.IsNullOrWhiteSpace(YearOfCreationText.Text) ? Convert.ToInt16(YearOfCreationText.Text) : null,
                !string.IsNullOrWhiteSpace(YearOfBreakupText.Text) ? Convert.ToInt16(YearOfBreakupText.Text) : null,
                countryId.Id);

            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                var genre = (GenreItems.Children[i] as GenreBoxItemControl).GenreItem;
                GroupItem.Genres.Add(genre);
                SqlDatabase.Instance.GroupGenresAdapter.Insert(GroupItem.Id, genre.Id);
            }

            DialogResult = true;
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            var countryId = (Country)(CountryId.SelectedItem as ComboBoxItem).Tag;

            var genres = new List<Genre>();
            for (var i = 0; i < GenreItems.Children.Count - 1; i++)
            {
                var genre = (GenreItems.Children[i] as GenreBoxItemControl).GenreItem;
                genres.Add(genre);
            }

            GroupItem.Update(NameText.Text, DescriptionText.Text,
                !string.IsNullOrWhiteSpace(YearOfCreationText.Text) ? Convert.ToInt16(YearOfCreationText.Text) : null,
                !string.IsNullOrWhiteSpace(YearOfBreakupText.Text) ? Convert.ToInt16(YearOfBreakupText.Text) : null,
                countryId, genres);

            DialogResult = true;
        }
    }
}
