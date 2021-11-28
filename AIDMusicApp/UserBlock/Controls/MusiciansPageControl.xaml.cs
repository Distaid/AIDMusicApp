using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AIDMusicApp.UserBlock.Controls
{
    /// <summary>
    /// Логика взаимодействия для MusiciansPageControl.xaml
    /// </summary>
    public partial class MusiciansPageControl : UserControl
    {
        private ObservableCollection<Musician> _newMusicians = new ObservableCollection<Musician>();
        private ObservableCollection<Musician> _searchMusicians = new ObservableCollection<Musician>();

        public MusiciansPageControl()
        {
            InitializeComponent();

            SearchButton.Click += SearchButton_Click;

            NewMusicians.ItemsSource = _newMusicians;
            SearchMusicians.ItemsSource = _searchMusicians;

            foreach (var musician in SqlDatabase.Instance.MusiciansAdapter.GetTop10())
                _newMusicians.Add(musician);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Trim() != "")
            {
                if (SearchBlock.Visibility == Visibility.Collapsed)
                    SearchBlock.Visibility = Visibility.Visible;

                _searchMusicians.Clear();

                foreach (var musicians in SqlDatabase.Instance.MusiciansAdapter.GetAllByName(SearchBox.Text))
                    _searchMusicians.Add(musicians);
            }
        }
    }
}
