using AIDMusicApp.Models;
using AIDMusicApp.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIDMusicApp.Admin.Controls.Skills
{
    /// <summary>
    /// Логика взаимодействия для SkillBoxItemControl.xaml
    /// </summary>
    public partial class SkillBoxItemControl : UserControl
    {
        public SkillBoxItemControl()
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            SkillComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var skill in SqlDatabase.Instance.SkillsListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = skill.Name, Tag = skill };
                SkillComboBox.Items.Add(item);
            }
        }

        public SkillBoxItemControl(int skillId)
        {
            InitializeComponent();

            RemoveButton.Click += RemoveButton_Click;

            SkillComboBox.PreviewMouseWheel += (o, e) =>
            {
                e.Handled = !((ComboBox)o).IsDropDownOpen;
            };

            foreach (var skill in SqlDatabase.Instance.SkillsListAdapter.GetAll())
            {
                var item = new ComboBoxItem { Content = skill.Name, Tag = skill };
                if (skill.Id == skillId)
                    item.IsSelected = true;
                SkillComboBox.Items.Add(item);
            }
        }

        public Skill SkillItem => (Skill)((ComboBoxItem)SkillComboBox.SelectedItem)?.Tag ?? null;

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
