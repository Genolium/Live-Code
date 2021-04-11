using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;


namespace Trix_UPD
{
    /// <summary>
    /// Логика взаимодействия для Create_project_window.xaml
    /// </summary>
    public partial class Create_project_window : Window
    {
        private int choice;
        public Create_project_window()
        {
            InitializeComponent();
        }
        private bool DoesDirectoryExist(string dir)
        {
            return Directory.Exists(dir);
        }
        private bool DoesFileExist(string fil)
        {
            return File.Exists(fil);
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
        private bool IsStringLatin (string sentence)
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
        private void CreateFolder(string path, MainWindow main, ListView listView)
        {
            Directory.CreateDirectory(path);
            char a = '\u005c';
            int pos = path.LastIndexOf(a);
            path = path.Substring(0, pos);
            string[] files = Directory.GetFiles(path);
            listView.Items.Clear();
            
        }
        private void CreateErrorMessage(string Error_text)
        {
            MessageBoxResult result = MessageBox.Show(Error_text, "Ошибка", MessageBoxButton.OK);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (DoesDirectoryExist(Location.Text) == false)
            {
                CreateErrorMessage("Указанного пути не существует");
                return;
            }
            if(IsStringLatin(Name.Text) == false)
            {
                CreateErrorMessage("Название проекта не написано на латинице или пустое. Проверьте введенные данные еще раз");
                return;
            }
            if (IsStringLatin(Location.Text) == false)
            {
                CreateErrorMessage("В расположении проекта присутствует кириллица или указанной папки не существует. Проверьте введенные данные еще раз.");
                return;
            }
            MainWindow main = new MainWindow();
            if (Location.Text.Length <= 3)
                Location.Text = Location.Text.Remove(2);
            var path = CheckBox_create_new_folder.IsChecked == true ? $@"{Location.Text}\{Name.Text}" : $@"{Location.Text}";
            try
            {
                CreateFolder(path, main, main.List_files);
                main.Path_link.Text = path;
            }
            catch (Exception ex)
            {
                CreateErrorMessage($@"{ex.Message}");
                return;
            }
            path += @"\index.html";
            if(DoesFileExist(path) == true)
            {
                CreateErrorMessage("Проект или файл с данным названием уже существует. Измените название и попробуйте снова.");
                return;
            }
            string html = "";
            if (choice == 1)
            {
                if (CheckBox_utf_8.IsChecked == true)
                {
                    if (DoesFileExist("Demos\\empty_utf8.txt"))
                        html = File.ReadAllText("Demos\\empty_utf8.txt");
                    else
                        CreateErrorMessage("Файлы программы, используемые для шаблонов, повреждены. Будет создан пустой файл.");
                }
                else {
                    if (DoesFileExist("Demos\\empty.txt"))
                        html = File.ReadAllText("Demos\\empty.txt");
                    else
                        CreateErrorMessage("Файлы программы, используемые для шаблонов, повреждены. Будет создан пустой файл.");
                }
            }
            if (choice == 2)
            {
                if (CheckBox_utf_8.IsChecked == true)
                {
                    if (DoesFileExist("Demos\\bootstrap_utf8.txt"))
                        html = File.ReadAllText("Demos\\bootstrap_utf8.txt");
                    else
                        CreateErrorMessage("Файлы программы, используемые для шаблонов, повреждены. Будет создан пустой файл.");
                }
                else
                {
                    if (DoesFileExist("Demos\\bootstrap.txt"))
                        html = File.ReadAllText("Demos\\bootstrap.txt");
                    else
                        CreateErrorMessage("Файлы программы, используемые для шаблонов, повреждены. Будет создан пустой файл.");
                }
            }
            System.IO.File.WriteAllText(path, html);
            main.ReloadLV();
            CloseWindow(main, path);
            this.Owner.Close();
            this.Close();
        }

        private void Location_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Выберите папку, пожалуйста";
            dialog.UseDescriptionForTitle = true;
            if ((bool)dialog.ShowDialog(this))
                Location.Text = dialog.SelectedPath;
        }
        private void Empty_Selected(object sender, RoutedEventArgs e)
        {
            choice = 1;
        }
        private void Bootstrap_Selected(object sender, RoutedEventArgs e)
        {
            choice = 2;
        }
    }
}
