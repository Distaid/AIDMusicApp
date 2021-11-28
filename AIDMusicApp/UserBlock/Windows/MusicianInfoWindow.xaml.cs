using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AIDMusicApp.UserBlock.Windows
{
    /// <summary>
    /// Логика взаимодействия для MusicianInfoWindow.xaml
    /// </summary>
    public partial class MusicianInfoWindow : Window
    {
        public MusicianInfoWindow(Musician musician)
        {
            InitializeComponent();

            TitleBar.MouseDown += TitleBar_MouseDown;

            TitleText.Text = musician.Name;

            MusicianItem.Id = musician.Id;
            MusicianItem.Name = musician.Name;
            MusicianItem.Age = musician.Age;
            MusicianItem.CountryId = musician.CountryId;
            MusicianItem.IsDead = musician.IsDead;
            MusicianItem.Skills = new ObservableCollection<Skill>();
            MusicianItem.Groups = new ObservableCollection<Group>();

            foreach (var skill in SqlDatabase.Instance.MusicianSkillsAdapter.GetSkillsByMusicianId(musician.Id))
                MusicianItem.Skills.Add(skill);

            foreach (var group in SqlDatabase.Instance.MembersAdapter.GetShortGroupsByMusicianId(musician.Id))
                MusicianItem.Groups.Add(group);
        }

        public Musician MusicianItem => (Musician)Resources["MusicianItem"];

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
