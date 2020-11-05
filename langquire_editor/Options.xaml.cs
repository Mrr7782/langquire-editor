using System;
using System.Windows;
using System.Windows.Media;

namespace langquire_editor {
    public partial class Options : Window {
        private void Button_Click(object sender, RoutedEventArgs e) => Close();
        public Options() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Title = $"Langquire Editor [Options] ({Environment.UserName})";
            if (Properties.Settings.Default.dark) {
                Background = new SolidColorBrush() { Color = Colors.DimGray };
                CBDark.IsChecked = true;
            }
        }
        
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.dark = (bool)CBDark.IsChecked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.reloadMain = true;
            Close();
        }
    }
}
