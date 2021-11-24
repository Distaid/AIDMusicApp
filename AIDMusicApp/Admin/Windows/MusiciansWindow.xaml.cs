using AIDMusicApp.Admin.Controls.Skills;
using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using AIDMusicApp.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AIDMusicApp.Admin.Windows
{
    /// <summary>
    /// Логика взаимодействия для MusiciansWindow.xaml
    /// </summary>
    public partial class MusiciansWindow : Window
    {
        public Musician MusicianItem { get; private set; } = null;

        public MusiciansWindow()
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            AgeText.PreviewTextInput += AgeText_PreviewTextInput;
            TitleText.Text = "Добавление Музыканта";
            ConfirmButton.Content = "Добавить";
            AddPanel.Visibility = Visibility.Visible;

            CountryId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            AddSkill.Click += AddSkill_Click;
            ConfirmButton.Click += AddButton_Click;

            foreach (var country in SqlDatabase.Instance.CountriesListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = country.Name, Tag = country };
                CountryId.Items.Add(item);
            }
        }

        public MusiciansWindow(Musician musician)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;
            AgeText.PreviewTextInput += AgeText_PreviewTextInput;
            TitleText.Text = "Редактирование Музыканта";

            SaveGeneralButton.Visibility = Visibility.Visible;
            SaveGeneralButton.Click += SaveGeneralButton_Click;

            SaveSkillsButton.Visibility = Visibility.Visible;
            SaveSkillsButton.Click += SaveSkillsButton_Click;

            CountryId.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            MusicianItem = musician;
            NameText.Text = MusicianItem.Name;
            AgeText.Text = MusicianItem.Age.ToString();
            IsDeadText.SelectedIndex = Convert.ToInt32(MusicianItem.IsDead);

            AddSkill.Click += AddSkill_Click;

            foreach (var country in SqlDatabase.Instance.CountriesListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = country.Name, Tag = country };
                CountryId.Items.Add(item);
                if (MusicianItem.CountryId.Id == country.Id)
                    item.IsSelected = true;
            }

            foreach (var skill in MusicianItem.Skills)
            {
                var item = new SkillBoxItemControl(skill.Id);
                SkillItems.Children.Insert(SkillItems.Children.Count - 1, item);
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void AgeText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !byte.TryParse(e.Text, out _);
        }

        private void AddSkill_Click(object sender, RoutedEventArgs e)
        {
            if (SkillItems.Children.Count != 1)
            {
                var skillItemControl = SkillItems.Children[SkillItems.Children.Count - 2] as SkillBoxItemControl;
                if (skillItemControl.SkillItem == null)
                    return;
            }

            var item = new SkillBoxItemControl();
            SkillItems.Children.Insert(SkillItems.Children.Count - 1, item);
        }

        private bool CheckFields()
        {
            if (string.IsNullOrWhiteSpace(NameText.Text))
            {
                AIDMessageWindow.Show("Поле \"Имя\" должно быть заполнено!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(AgeText.Text))
            {
                AIDMessageWindow.Show("Поле \"Возраст\" должно быть заполнено!");
                return false;
            }

            if (CountryId.SelectedIndex == -1)
            {
                AIDMessageWindow.Show("Поле \"Страна\" должно быть заполнено!");
                return false;
            }

            for (var i = 0; i < SkillItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < SkillItems.Children.Count - 1; j++)
                {
                    if ((SkillItems.Children[i] as SkillBoxItemControl).SkillItem.Id == (SkillItems.Children[j] as SkillBoxItemControl).SkillItem.Id)
                    {
                        AIDMessageWindow.Show("Навыки не должны повторяться!");
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

            MusicianItem = SqlDatabase.Instance.MusiciansAdapter.Insert(NameText.Text, Convert.ToByte(AgeText.Text), countryId.Id, Convert.ToBoolean(IsDeadText.SelectedIndex));

            for (var i = 0; i < SkillItems.Children.Count - 1; i++)
            {
                var skill = (SkillItems.Children[i] as SkillBoxItemControl).SkillItem;
                MusicianItem.Skills.Add(skill);
                SqlDatabase.Instance.MusicianSkillsAdapter.Insert(MusicianItem.Id, skill.Id);
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

            if (string.IsNullOrWhiteSpace(AgeText.Text))
            {
                AIDMessageWindow.Show("Поле \"Возраст\" должно быть заполнено!");
                return;
            }

            if (CountryId.SelectedIndex == -1)
            {
                AIDMessageWindow.Show("Поле \"Страна\" должно быть заполнено!");
                return;
            }

            var countryId = (Country)(CountryId.SelectedItem as ComboBoxItem).Tag;

            MusicianItem.Update(NameText.Text, Convert.ToByte(AgeText.Text), countryId, Convert.ToBoolean(IsDeadText.SelectedIndex));

            AIDMessageWindow.Show("Сохранено успешно!");
        }

        private void SaveSkillsButton_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < SkillItems.Children.Count - 1; i++)
            {
                for (var j = i + 1; j < SkillItems.Children.Count - 1; j++)
                {
                    if ((SkillItems.Children[i] as SkillBoxItemControl).SkillItem.Id == (SkillItems.Children[j] as SkillBoxItemControl).SkillItem.Id)
                    {
                        AIDMessageWindow.Show("Навыки не должны повторяться!");
                        return;
                    }
                }
            }

            var skills = new List<Skill>();
            for (var i = 0; i < SkillItems.Children.Count - 1; i++)
            {
                var skill = (SkillItems.Children[i] as SkillBoxItemControl).SkillItem;
                skills.Add(skill);
            }

            SqlDatabase.Instance.MusicianSkillsAdapter.Update(MusicianItem.Skills.ToList(), skills, MusicianItem.Id);

            MusicianItem.Skills.Clear();
            foreach (var skill in skills)
                MusicianItem.Skills.Add(skill);

            AIDMessageWindow.Show("Сохранено успешно!");
        }
    }
}
