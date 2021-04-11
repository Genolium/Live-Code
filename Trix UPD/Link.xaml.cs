using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;

namespace Trix_UPD
{
    /// <summary>
    /// Логика взаимодействия для Link.xaml
    /// </summary>
    public partial class Link : Window
    {
        public Link()
        {
            InitializeComponent();
        }
        private bool IsStringLatin(string sentence)
        {
            int engCount = 0;
            int rusCount = 0;
            foreach (char c in sentence)
            {
                if ((c > 'а' && c < 'я') || (c > 'А' && c < 'Я'))
                    rusCount++;
                else if ((c > 'a' && c < 'z') || (c > 'A' && c < 'Z') || (c > '0' && c < '9'))
                    engCount++;
            }
            if (rusCount > 0 || engCount == 0)
                return false;
            else
                return true;
        }
        private bool DoesDirectoryExist(string dir)
        {
            return Directory.Exists(dir);
        }
        private bool DoesFileExist(string fil)
        {
            return File.Exists(fil);
        }
        private void CreateErrorMessage(string Error_text)
        {
            MessageBoxResult result = MessageBox.Show(Error_text, "Ошибка", MessageBoxButton.OK);
        }
        private bool IsSiteWorking(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
        private string GetHTML(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;
            if (response.CharacterSet == null)
                readStream = new StreamReader(receiveStream);
            else
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            string html = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return html;
        }
        private string check = "https://";
        private char a = '\u005c';
        private string CorrectString(string sentence)
        {
            char a = '\u005c';
            string replacedname = sentence;
            replacedname = replacedname.Remove(0, check.Length);
            replacedname = replacedname.Replace("/", "");
            replacedname = replacedname.Replace(":", "");
            replacedname = replacedname.Replace("?", "");
            replacedname = replacedname.Replace("\"", "");
            replacedname = replacedname.Replace("<", "");
            replacedname = replacedname.Replace(">", "");
            replacedname = replacedname.Replace("|", "");
            replacedname = replacedname.Replace(a.ToString(), "");
            return replacedname;
        }
        private void CloseWindow(MainWindow main, string path)
        {
            main.Show();
            main.Browser.Address = path;
            main.paragr.Text = File.ReadAllText(path);
            main.Adress.Content = path.ToString();
            for (int i = 0; i < main.MainGrid.Children.Count; i++)
            {
                main.MainGrid.Children[i].IsEnabled = true;
            }
        }

        private void Location_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Выберите папку, пожалуйста";
            dialog.UseDescriptionForTitle = true;
            if ((bool)dialog.ShowDialog(this))
                Location.Text = dialog.SelectedPath;
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string checkedlabel;
            try
            {
                checkedlabel = Name.Text.Substring(0, check.Length);
            }
            catch
            {
                CreateErrorMessage("Адрес сайта должен начинаться с \"https://\" Проверьте введенные данные еще раз.");
                return;
            }
            if (check != checkedlabel)
            {
                CreateErrorMessage("Адрес сайта должен начинаться с \"https://\" Проверьте введенные данные еще раз.");
                return;
            }
            if (DoesDirectoryExist(Location.Text) == false)
            {
                CreateErrorMessage("Указанного пути не существует");
                return;
            }
            if (IsStringLatin(Name.Text) == false)
            {
                CreateErrorMessage("Название проекта не написано на латинице или пустое. Проверьте введенные данные еще раз");
                return;
            }
            else
            {
                if (IsStringLatin(Location.Text) == false)
                {
                    CreateErrorMessage("В расположении проекта присутствует кириллица или указанной папки не существует. Проверьте введенные данные еще раз.");
                    return;
                }
                MainWindow main = new MainWindow();
                if (Location.Text.Length <= 3)
                    Location.Text = Location.Text.Remove(2);
                string urlAddress = Name.Text;
                if (IsSiteWorking(urlAddress) == true)
                {
                    string html = GetHTML(urlAddress);
                    string path;
                    if (CheckBox.IsChecked == true)
                    {
                        path = $@"{Location.Text}\{CorrectString(Name.Text)}";
                        Directory.CreateDirectory(path);
                        main.Path_link.Text = path;
                        string[] files = Directory.GetFiles(path);
                        for (int x = 0; x < files.Length; x++)
                        {
                            string extension = System.IO.Path.GetExtension(files[x]);
                            main.List_files.Items.Add(files[x]);
                        }
                        int pos = path.LastIndexOf(a);
                        path = path.Substring(0, pos);
                        main.List_files.Items.Clear();
                        path = $@"{Location.Text}\{CorrectString(Name.Text)}\index.html";
                    }
                    else
                    {
                        path = $@"{Location.Text}";
                        Directory.CreateDirectory(path);
                        main.Path_link.Text = path;
                        string[] files = Directory.GetFiles(path);
                        for (int x = 0; x < files.Length; x++)
                        {
                            string extension = System.IO.Path.GetExtension(files[x]);
                            main.List_files.Items.Add(files[x]);
                        }
                        main.List_files.Items.Clear();
                        path = $@"{Location.Text}\index.html";
                    }
                    if (DoesFileExist(path) == true)
                    {
                        CreateErrorMessage("Проект или файл с данным названием уже существует. Измените название и попробуйте снова.");
                        return;
                    }
                    System.IO.File.WriteAllText(path, html);
                    main.Show();
                    main.Browser.Address = path;
                    main.paragr.Text = File.ReadAllText(path);
                    main.Adress.Content = path.ToString();
                    for (int i = 0; i < main.MainGrid.Children.Count; i++)
                    {
                        main.MainGrid.Children[i].IsEnabled = true;
                    }
                    main.ReloadLV();
                    this.Owner.Close();
                    this.Close();
                }
                else
                {
                    CreateErrorMessage("Отсутствует интернет соединение или сайт недоступен");
                    return;
                }
            }
        }
    }
}
