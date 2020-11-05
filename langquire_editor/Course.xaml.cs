using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using Microsoft.VisualBasic;

struct lesson {
    public string title;
    public string path;
}

namespace langquire_editor {
    public partial class Course : Window {
        //Variables
        public StreamReader sr;
        public StreamWriter sw;
        private List<lesson> lsns = new List<lesson>();
        //Lambda voids
        public void initIdxReader() => sr = new StreamReader($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
        public void initIdxWriter() => sw = new StreamWriter($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
        private void btnRef_Click(object sender, RoutedEventArgs e) => reloadCourses();
        public Course() => InitializeComponent();

        //Random stuff
        public void skipLines(int lines) {
            for (int x = 0; x < lines; x++)
                sr.ReadLine();
        }

        //File manipulation
        public void reloadCourses() {
            CBCourse.Items.Clear();
            foreach (string c in Directory.GetDirectories($"{Environment.CurrentDirectory}\\courses"))
                CBCourse.Items.Add(Path.GetFileName(c));
        }

        public void reloadLessons() {
            Lessons.Items.Clear();
            if (File.Exists($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs")) {
                lsns.Clear();
                sr = new StreamReader($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
                while (!sr.EndOfStream) {
                    lesson a;
                    a.title = sr.ReadLine();
                    a.path = sr.ReadLine();
                    lsns.Add(a);
                }
                sr.Close();
                sr.Dispose();
                foreach (lesson l in lsns)
                    Lessons.Items.Add($"[{l.path}] {l.title}");
            } else {
                MessageBox.Show("Couldn't find this course's index.lqs file!", "Langquire Editor Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //User interaction
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Title = $"Langquire Editor [Course editor] ({Environment.UserName})";
            if (Properties.Settings.Default.dark) {
                Background = new SolidColorBrush() { Color = Colors.DimGray };
                Lessons.Background = new SolidColorBrush() { Color = Colors.Gray };
            }
            reloadCourses();
        }

        private void btnAddCrs_Click(object sender, RoutedEventArgs e) {
            string crs = Interaction.InputBox("Choose a course name.", "Langquire Editor: New course", "");
            if (crs != null && !crs.Equals("")) {
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\courses\\{crs}");
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\courses\\{crs}\\lessons");
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\courses\\{crs}\\practice");
                File.Create($"{Environment.CurrentDirectory}\\courses\\{crs}\\lessons\\index.lqs");
                File.Create($"{Environment.CurrentDirectory}\\courses\\{crs}\\practice\\struct.lqs");
                reloadCourses();
            }
        }

        private void btnDelCrs_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show($"Are you sure you want to delete the \"{CBCourse.SelectedItem.ToString()}\" course?", "Langquire Editor: Delete course", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                Directory.Delete($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}", true);
                reloadCourses();
            }
        }


        private void btnAddLssn_Click(object sender, RoutedEventArgs e) {
            string t = Interaction.InputBox("Choose a lesson name.", "Langquire Editor: New lesson");
            if (t != null && !t.Equals("")) {
                string p = Interaction.InputBox("Choose a path and filename.", "Langquire Editor: New lesson");
                if (p != null && !p.Equals("") && p.EndsWith(".lqs")) {
                    int i = Lessons.SelectedIndex != -1 ? Lessons.SelectedIndex + 1 : Lessons.Items.Count;
                    lesson a;
                    a.title = t;
                    a.path = p;
                    lsns.Insert(i, a);
                    sw = new StreamWriter($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\{p}");
                    sw.WriteLine(t);
                    sw.WriteLine("10001");
                    sw.WriteLine("l");
                    sw.WriteLine("WIP");
                    sw.WriteLine("This lesson isn't done yet!");
                    sw.Close();
                    sw.Dispose();
                    File.Delete($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
                    initIdxWriter();
                    foreach (lesson l in lsns) {
                        sw.WriteLine(l.title);
                        sw.WriteLine(l.path);
                    }
                    sw.Close();
                    sw.Dispose();
                    reloadLessons();
                    Lessons.SelectedIndex = i;
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            Editor ed = new Editor(CBCourse.SelectedItem.ToString(), lsns[Lessons.SelectedIndex].path);
            ed.Show();
        }

        private void btnDelLssn_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show($"Are you sure you want to delete the \"{lsns[Lessons.SelectedIndex].title}\" lesson?", "Langquire Editor: Lesson deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                int i = Lessons.SelectedIndex;
                File.Delete($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\{lsns[Lessons.SelectedIndex].path}");
                List<string> idx = new List<string>();
                initIdxReader();
                for (int x = 0; x < Lessons.SelectedIndex * 2; x++)
                    idx.Add(sr.ReadLine());
                skipLines(2);
                while (!sr.EndOfStream)
                    idx.Add(sr.ReadLine());
                sr.Close();
                sr.Dispose();
                File.Delete($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
                initIdxWriter();
                foreach (string s in idx)
                    sw.WriteLine(s);
                sw.Close();
                sw.Dispose();
                reloadLessons();
                Lessons.SelectedIndex = i == 0 ? i : i - 1;
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e) {
            int p = Lessons.SelectedIndex;
            lsns.Insert(p - 1, lsns[p]);
            lsns.RemoveAt(p + 1);
            File.Delete($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
            initIdxWriter();
            foreach (lesson l in lsns) {
                sw.WriteLine(l.title);
                sw.WriteLine(l.path);
            }
            sw.Close();
            sw.Dispose();
            reloadLessons();
            Lessons.SelectedIndex = p - 1;
        }

        private void btnDn_Click(object sender, RoutedEventArgs e) {
            int p = Lessons.SelectedIndex;
            lsns.Insert(p + 2, lsns[p]);
            lsns.RemoveAt(p);
            File.Delete($"{Environment.CurrentDirectory}\\courses\\{CBCourse.SelectedItem.ToString()}\\lessons\\index.lqs");
            initIdxWriter();
            foreach (lesson l in lsns) {
                sw.WriteLine(l.title);
                sw.WriteLine(l.path);
            }
            sw.Close();
            sw.Dispose();
            reloadLessons();
            Lessons.SelectedIndex = p + 1;
        }

        private void btnFld_Click(object sender, RoutedEventArgs e) {
            FolderMgr f = new FolderMgr(CBCourse.SelectedItem.ToString());
            f.Show();
        }

        private void CBCourse_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (CBCourse.SelectedIndex != -1) {
                btnDelCrs.IsEnabled = true;
                btnAddLssn.IsEnabled = true;
                btnFld.IsEnabled = true;
                reloadLessons();
            } else {
                btnDelCrs.IsEnabled = false;
                btnAddLssn.IsEnabled = false;
                btnFld.IsEnabled = false;
                Lessons.Items.Clear();
            }
        }

        private void Lessons_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (Lessons.SelectedIndex != -1) {
                btnEdit.IsEnabled = true;
                btnDelLssn.IsEnabled = true;
                btnUp.IsEnabled = Lessons.SelectedIndex == 0 ? false : true;
                btnDn.IsEnabled = Lessons.SelectedIndex == Lessons.Items.Count - 1 ? false : true;
            } else {
                btnEdit.IsEnabled = false;
                btnDelLssn.IsEnabled = false;
                btnUp.IsEnabled = false;
                btnDn.IsEnabled = false;
            }
        }
    }
}
