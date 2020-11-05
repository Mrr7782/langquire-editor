using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using Microsoft.VisualBasic;

namespace langquire_editor {
    public partial class FolderMgr : Window {
        public string course;
        private void btnRef_Click(object sender, RoutedEventArgs e) => reload();

        public FolderMgr(string courseName) {
            InitializeComponent();
            course = courseName;
        }

        public void reload() {
            lbx.Items.Clear();
            foreach (string d in Directory.GetDirectories($"{Environment.CurrentDirectory}\\courses\\{course}\\lessons", "*", SearchOption.AllDirectories))
                lbx.Items.Add(d.Substring(Environment.CurrentDirectory.Length + course.Length + 18, d.Length - (Environment.CurrentDirectory.Length + course.Length + 18)));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Title = $"Langquire Editor [Folder manager] ({Environment.UserName})";
            if (Properties.Settings.Default.dark) {
                Background = new SolidColorBrush() { Color = Colors.DimGray };
                lbx.Background = new SolidColorBrush() { Color = Colors.Gray };
            }
            reload();
        }

        private void lbx_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lbx.SelectedIndex != -1)
                btnDel.IsEnabled = true;
            else
                btnDel.IsEnabled = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            string f = Interaction.InputBox("Choose a folder path.", "Langquire Editor: Folder manager", "");
            if (f != null && !f.Equals(""))
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\courses\\{course}\\lessons\\{f}");
            reload();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e) {
            if (Directory.GetFiles($"{Environment.CurrentDirectory}\\courses\\{course}\\lessons\\{lbx.SelectedItem.ToString()}", "*.lqs").Length == 0) {
                if (MessageBox.Show($"Are you sure you want to delete \"{lbx.SelectedItem.ToString()}\" and all of its contents?", "Langquire Editor: Folder manager", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    Directory.Delete($"{Environment.CurrentDirectory}\\courses\\{course}\\lessons\\{lbx.SelectedItem.ToString()}", true);
                reload();
            } else {
                MessageBox.Show("You can't delete a folder that contains lessons. Please delete all the lessons inside this folder using the Course editor first.", "Langquire Editor Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
