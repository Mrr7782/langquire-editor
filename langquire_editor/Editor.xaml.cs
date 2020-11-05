using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

struct question {
    public string type;
    public string title;
    public string text;
    public string[] ans;
}

namespace langquire_editor {
    public partial class Editor : Window {
        //Variables
        private List<question> qstn = new List<question>();
        public Regex rgx = new Regex("^[0-9]+$");
        public StreamReader sr;
        public StreamWriter sw;
        public string ot = "0";
        public string crs;
        public string lsn;
        //Lambda voids
        private void Button_Click_2(object sender, RoutedEventArgs e) => addQuestion("l");
        private void Button_Click_3(object sender, RoutedEventArgs e) => addQuestion("d");
        private void Button_Click_4(object sender, RoutedEventArgs e) => addQuestion("q");
        private void Button_Click_10(object sender, RoutedEventArgs e) => editAns(0);
        private void Button_Click_11(object sender, RoutedEventArgs e) => editAns(1);
        private void Button_Click_12(object sender, RoutedEventArgs e) => editAns(2);
        private void Button_Click_13(object sender, RoutedEventArgs e) => editAns(3);
        private void Button_Click_8(object sender, RoutedEventArgs e) => editAns(0);
        private void Button_Click_9(object sender, RoutedEventArgs e) => editAns(1);
        private void Button_Click_1(object sender, RoutedEventArgs e) => revert();

        //init
        public Editor(string course, string path) {
            InitializeComponent();
            crs = course;
            lsn = path;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Title = $"Langquire Editor [Lesson editor] ({Environment.UserName})";
            if (Properties.Settings.Default.dark) {
                Background = new SolidColorBrush() { Color = Colors.DimGray };
                txtAcc.Background = new SolidColorBrush() { Color = Colors.Gray };
                lbx.Background = new SolidColorBrush() { Color = Colors.Gray };
                qTitle.Foreground = new SolidColorBrush() { Color = Colors.White };
                tTitle.Background = new SolidColorBrush() { Color = Colors.Gray };
                qText.Foreground = new SolidColorBrush() { Color = Colors.White };
                tText.Background = new SolidColorBrush() { Color = Colors.Gray };
            }
            revert();
        }

        //Lesson manipulation
        public void revert() {
            sr = new StreamReader($"{Environment.CurrentDirectory}\\courses\\{crs}\\lessons\\{lsn}");
            sr.ReadLine();
            ot = sr.ReadLine();
            txtAcc.Text = ot;
            qstn.Clear();
            while (!sr.EndOfStream) {
                question q = new question();
                q.type = sr.ReadLine();
                q.title = sr.ReadLine();
                q.text = sr.ReadLine();
                switch (q.type) {
                    case "l":
                        break;
                    case "d":
                        q.ans = new string[2] { sr.ReadLine(), sr.ReadLine() };
                        break;
                    case "q":
                        q.ans = new string[4] { sr.ReadLine(), sr.ReadLine(), sr.ReadLine(), sr.ReadLine() };
                        break;
                }
                qstn.Add(q);
            }
            sr.Close();
            sr.Dispose();
            reload();
        }

        public void reload() {
            lbx.Items.Clear();
            foreach (question q in qstn)
                lbx.Items.Add($"[{q.type.ToUpper()}] {q.title}");
        }

        public void addQuestion(string type) {
            question q = new question();
            q.type = type;
            q.title = "";
            q.text = "";
            switch (type) {
                case "l":
                    break;
                case "d":
                    q.ans = new string[2] { "", "" };
                    break;
                case "q":
                    q.ans = new string[4] { "", "", "", "" };
                    break;
            }
            int i = lbx.SelectedIndex;
            if (i == -1)
                qstn.Add(q);
            else
                qstn.Insert(i + 1, q);
            reload();
            lbx.SelectedIndex = i == -1 ? lbx.Items.Count - 1 : i + 1;
        }

        public void editAns(int ans) {
            int i = lbx.SelectedIndex;
            string a = Interaction.InputBox("Enter new answer text.", "Langquire Editor: Edit answer", qstn[i].ans[ans]);
            if (a != null && !a.Equals("")) {
                qstn[i].ans[ans] = a;
                reload();
            }
            lbx.SelectedIndex = i;
        }

        //Interaction
        private void txtAcc_TextChanged(object sender, TextChangedEventArgs e) {
            if (!rgx.IsMatch(txtAcc.Text)) {
                txtAcc.Text = ot;
            } else {
                if (Convert.ToInt32(txtAcc.Text) >= 0 && Convert.ToInt32(txtAcc.Text) <= 10001)
                    ot = txtAcc.Text;
                else
                    txtAcc.Text = ot;
            }
        }

        private void lbx_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lbx.SelectedIndex != -1) {
                btnUp.IsEnabled = lbx.SelectedIndex == 0 ? false : true;
                btnDn.IsEnabled = lbx.SelectedIndex == lbx.Items.Count - 1 ? false : true;
                btnDl.IsEnabled = true;
                tTitle.Visibility = Visibility.Hidden;
                tText.Visibility = Visibility.Hidden;
                qTitle.Visibility = Visibility.Visible;
                qText.Visibility = Visibility.Visible;
                qTitle.Text = qstn[lbx.SelectedIndex].title;
                qText.Text = qstn[lbx.SelectedIndex].text;
                gridL.Visibility = Visibility.Hidden;
                gridD.Visibility = Visibility.Hidden;
                gridQ.Visibility = Visibility.Hidden;
                switch (qstn[lbx.SelectedIndex].type) {
                    case "l":
                        gridL.Visibility = Visibility.Visible;
                        break;
                    case "d":
                        gridD.Visibility = Visibility.Visible;
                        duoA.Text = qstn[lbx.SelectedIndex].ans[0];
                        duoB.Text = qstn[lbx.SelectedIndex].ans[1];
                        break;
                    case "q":
                        gridQ.Visibility = Visibility.Visible;
                        quadA.Text = qstn[lbx.SelectedIndex].ans[0];
                        quadB.Text = qstn[lbx.SelectedIndex].ans[1];
                        quadC.Text = qstn[lbx.SelectedIndex].ans[2];
                        quadD.Text = qstn[lbx.SelectedIndex].ans[3];
                        break;
                }
            } else {
                btnUp.IsEnabled = false;
                btnDn.IsEnabled = false;
                btnDl.IsEnabled = false;
                qTitle.Text = "";
                qText.Text = "";
                tTitle.Visibility = Visibility.Hidden;
                tText.Visibility = Visibility.Hidden;
                qTitle.Visibility = Visibility.Visible;
                qText.Visibility = Visibility.Visible;
                gridL.Visibility = Visibility.Hidden;
                gridD.Visibility = Visibility.Hidden;
                gridQ.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            File.Delete($"{Environment.CurrentDirectory}\\courses\\{crs}\\lessons\\{lsn}");
            sw = new StreamWriter($"{Environment.CurrentDirectory}\\courses\\{crs}\\lessons\\{lsn}");
            sw.WriteLine(qstn.Count);
            sw.WriteLine(txtAcc.Text);
            foreach (question q in qstn) {
                sw.WriteLine(q.type);
                sw.WriteLine(q.title);
                sw.WriteLine(q.text);
                switch (q.type) {
                    case "l":
                        break;
                    case "d":
                        sw.WriteLine(q.ans[0]);
                        sw.WriteLine(q.ans[1]);
                        break;
                    case "q":
                        for (int x = 0; x < 4; x++)
                            sw.WriteLine(q.ans[x]);
                        break;
                }
            }
            sw.Close();
            sw.Dispose();
        }


        private void Button_Click_5(object sender, RoutedEventArgs e) {
            int i = lbx.SelectedIndex;
            qstn.Insert(i - 1, qstn[i]);
            qstn.RemoveAt(i + 1);
            reload();
            lbx.SelectedIndex = i - 1;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) {
            int i = lbx.SelectedIndex;
            qstn.Insert(i + 2, qstn[i]);
            qstn.RemoveAt(i);
            reload();
            lbx.SelectedIndex = i + 1;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e) {
            int i = lbx.SelectedIndex;
            qstn.RemoveAt(i);
            reload();
            lbx.SelectedIndex = i == 0 ? i : i - 1;
        }
        
        private void Grid_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return || e.Key == Key.Enter) {
                tTitle.Visibility = Visibility.Hidden;
                tText.Visibility = Visibility.Hidden;
                qTitle.Visibility = Visibility.Visible;
                qText.Visibility = Visibility.Visible;
                if (lbx.SelectedIndex != -1) {
                    int i = lbx.SelectedIndex;
                    reload();
                    lbx.SelectedIndex = i;
                }
            }
        }

        private void Viewbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            tTitle.Text = qTitle.Text;
            tTitle.Visibility = Visibility.Visible;
            qTitle.Visibility = Visibility.Hidden;
            tTitle.Focus();
            tTitle.CaretIndex = tTitle.Text.Length;
        }

        private void qText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            tText.Text = qText.Text;
            tText.Visibility = Visibility.Visible;
            qText.Visibility = Visibility.Hidden;
            tText.Focus();
            tText.CaretIndex = tText.Text.Length;
        }

        private void tTitle_TextChanged(object sender, TextChangedEventArgs e) {
            question q = qstn[lbx.SelectedIndex];
            q.title = tTitle.Text;
            qstn[lbx.SelectedIndex] = q;
            qTitle.Text = tTitle.Text;
        }

        private void tText_TextChanged(object sender, TextChangedEventArgs e) {
            question q = qstn[lbx.SelectedIndex];
            q.text = tText.Text;
            qstn[lbx.SelectedIndex] = q;
            qText.Text = tText.Text;
        }
    }
}
