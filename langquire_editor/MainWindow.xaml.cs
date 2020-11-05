using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;

namespace langquire_editor {
    public partial class MainWindow : Window {
        public MainWindow() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Title = $"Langquire Editor [Welcome] ({Environment.UserName})";
            wTBlock.Text = $"Welcome to Langquire Editor, {Environment.UserName}!";
            if (Properties.Settings.Default.dark) {
                Background = new SolidColorBrush() { Color = Colors.DimGray };
                wTBlock.Foreground = new SolidColorBrush() { Color = Colors.White };
            }
            if (!Directory.Exists($"{Environment.CurrentDirectory}\\courses"))
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\courses");
            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Tick += Tmr_Tick;
            tmr.Interval = new TimeSpan(0, 0, 0, 1);
            tmr.Start();
        }

        private void Tmr_Tick(object sender, EventArgs e) {
            if (Properties.Settings.Default.reloadMain) {
                Properties.Settings.Default.reloadMain = false;
                if (Properties.Settings.Default.dark) {
                    Background = new SolidColorBrush() { Color = Colors.DimGray };
                    wTBlock.Foreground = new SolidColorBrush() { Color = Colors.White };
                } else {
                    Background = new SolidColorBrush() { Color = Colors.White };
                    wTBlock.Foreground = new SolidColorBrush() { Color = Colors.Black };
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Options o = new Options();
            o.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Course c = new Course();
            c.Show();
        }
    }
}
