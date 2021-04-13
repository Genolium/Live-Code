using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using CefSharp;
using Ookii.Dialogs.Wpf;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Trix_UPD
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public class PrFiles
    {
        public string FileLocation { get; set; }
        public PrFiles(string filel)
        {
            FileLocation = filel;
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLastProject();
        }
        public bool IsFileEmpty(string fileloc)
        {
            string a = fileloc;
            a.Replace(" ", "");
            if (a == "")
                return true;
            else
                return false;
        }
        public void SaveJson(string fileloc)
        {
            string extension = System.IO.Path.GetExtension(fileloc);
            if (extension == ".html")
            {
                PrFiles pr = new PrFiles(fileloc);
                string serialized = JsonConvert.SerializeObject(pr);
                File.WriteAllText("trix.json", serialized);
            }
        }
        public void ShowLastProject()
        {
            try
            {
                string json = File.ReadAllText("trix.json");
                PrFiles pr = JsonConvert.DeserializeObject<PrFiles>(json);
                MenuItem menui = new MenuItem();
                menui.Header = pr.FileLocation;
                menui.Click += new RoutedEventHandler(menui_Click);
                LastPr.Items.Add(menui);
            }
            catch { }
        }
        public void menui_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menui = e.Source as MenuItem;
                Adress.Content = menui.Header.ToString();
                TextRange range;
                FileStream fStream;
                range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                fStream = new FileStream(Adress.Content.ToString(), FileMode.Open);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                Browser.Address = Adress.Content.ToString();
                Browser.Reload();
                ReloadLV();
                SaveJson(Adress.Content.ToString());
                ClosePr.IsEnabled = true;
                EnableScreen(Adress.Content.ToString());
                Browser.Reload();
                ReloadLV();
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Во время открытия проекта произошла непредвиденная ошибка. Указанный файл не существует или перемещен.", "Ошибка", MessageBoxButton.OK);
                DisableScreen();
                return;
            }
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            DisableScreen();
            Application.Current.Shutdown();
        }
        System.Windows.Threading.DispatcherTimer timer2 = new System.Windows.Threading.DispatcherTimer();
        private void listviewitem_click(object sender, RoutedEventArgs e)
        {
            ListViewItem lvs = e.Source as ListViewItem;
            if (lvs.IsEnabled)
            {
                RTB.IsEnabled = false;
                AddFile.IsEnabled = true;
                Add_new_file.IsEnabled = true;
                Copy_file.IsEnabled = true;
                Adress.Content = "";
                paragr.Text = " ";
                Adress.Content = lvs.Content.ToString();
                RTB.IsEnabled = true;
                TextRange range;
                FileStream fStream;
                range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                fStream = new FileStream(lvs.Content.ToString(), FileMode.Open);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                Browser.Address = lvs.Content.ToString();
                lvs.IsEnabled = false;
                timer2.Interval = TimeSpan.FromSeconds(1);
                timer2.Tick += (o, args) => lvs.IsEnabled = true;
                timer2.Start();
                ClosePr.IsEnabled = true;
            }
        }
        private void DisableScreen()
        {
            SaveJson(Adress.Content.ToString());
            Browser.Visibility = Visibility.Hidden;
            for (int i = 0; i < MainGrid.Children.Count; i++)
            {
                MainGrid.Children[i].IsEnabled = false;
            }
            for (int i = 0; i < GridSecond.Children.Count; i++)
            {
                GridSecond.Children[i].IsEnabled = false;
            }

            AddFile.IsEnabled = false;
            Add_new_file.IsEnabled = false;
            Copy_file.IsEnabled = false;

            ClosePr.IsEnabled = false;
            Save.IsEnabled = false;

            List_files.Items.Clear();
            Adress.Content = "";
            Path_link.Text = "";
            Browser.Address = "";
            paragr.Text = "Документ не открыт";
            GridThird.IsEnabled = true;
            ShowLastProject();
        }
        private void EnableScreen(string filename)
        {
            Browser.Visibility = Visibility.Visible;
            Browser.Address = filename;
            Adress.Content = filename;
            Save.IsEnabled = true;
            MainGrid.IsEnabled = true;
            ClosePr.IsEnabled = true;
            AddFile.IsEnabled = true;
            Add_new_file.IsEnabled = true;
            Copy_file.IsEnabled = true;
            Browser.Reload();
            paragr.Text = File.ReadAllText(filename);
            string path = Adress.Content.ToString();
            char a = '\u005c';
            int pos = path.LastIndexOf(a);
            path = path.Substring(0, pos);
            Console.WriteLine(path);
            string[] files = Directory.GetFiles(path);
            List_files.Items.Clear();
            for (int x = 0; x < files.Length; x++)
            {
                string extension = System.IO.Path.GetExtension(files[x]);
                ListViewItem listview_item = new ListViewItem();
                listview_item.KeyDown += ListViewItem_KeyDown;
                listview_item.MouseDoubleClick += listviewitem_click;
                listview_item.Content = files[x].ToString();
                List_files.Items.Add(listview_item);
            }
            for (int i = 0; i < MainGrid.Children.Count; i++)
            {
                MainGrid.Children[i].IsEnabled = true;
            }
            for (int i = 0; i < GridSecond.Children.Count; i++)
            {
                GridSecond.Children[i].IsEnabled = true;
            }
            GridThird.IsEnabled = true;
        }
        private void Create_project_click(object sender, RoutedEventArgs e)
        {
            Create_project_window createPr = new Create_project_window();
            createPr.Show();
            createPr.Owner = this;
        }

        private void RTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RTB.IsEnabled == true)
            {
                try
                {
                    TextRange range;
                    FileStream fStream;
                    range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                    fStream = new FileStream(Adress.Content.ToString(), FileMode.Create);
                    range.Save(fStream, DataFormats.Text);
                    fStream.Close();
                    Browser.Reload();
                }
                catch
                {}
            }
        }

        private void Open_project_click(object sender, RoutedEventArgs e)
        {
            if (paragr.Text != "Документ не открыт")
                paragr.Text = "";
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "Все файлы (*.html*)|*.html*";
            if ((bool)dialog.ShowDialog(this))
            {
                if (dialog.FileName == "")
                    return;
                Path_link.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
                EnableScreen(dialog.FileName);
                RTB.IsEnabled = true;
            }
            ClosePr.IsEnabled = true;
            Save.IsEnabled = true;
        }

        private void Link_click(object sender, RoutedEventArgs e)
        {
            Link link = new Link();
            link.Show();
            link.Owner = this;
        }

        private void Settings_click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.tblock.Text = paragr.FontSize.ToString();
            if (settingsWindow.ShowDialog() == true)
            {
                int a;
                if (int.TryParse(settingsWindow.result, out a))
                {
                    if(a>0)
                        paragr.FontSize = Convert.ToDouble(a);
                    else
                        CreateErrorMessage("Число должно быть больше 0");
                }
                else
                    CreateErrorMessage("Неправильно введено число");
            }
        }
        System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        public void ReloadLV()
        {
            if (Reload_btn.IsEnabled == true && Path_link.Text != "")
            {
                List_files.Items.Clear();
                string[] files = Directory.GetFiles(Path_link.Text.ToString());
                for (int x = 0; x < files.Length; x++)
                {
                    string extension = System.IO.Path.GetExtension(files[x]);
                    ListViewItem listview_item = new ListViewItem();
                    listview_item.KeyDown += ListViewItem_KeyDown;
                    listview_item.MouseDoubleClick += listviewitem_click;
                    listview_item.Content = files[x].ToString();
                    List_files.Items.Add(listview_item);
                }
                Reload_btn.IsEnabled = false;
                timer1.Interval = TimeSpan.FromSeconds(1.3);
                timer1.Tick += (o, args) => Reload_btn.IsEnabled = true;
                timer1.Start();
            }
        }
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            ReloadLV();
        }

        private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            ListViewItem lvs = e.Source as ListViewItem;
            if(e.Key == Key.Delete)
            {
                string messageBoxText = $"Вы собираетесь удалить файл {lvs.Content.ToString()}.\nВы точно этого хотите?";
                string caption = "Trix";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                // Process message box results
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        File.Delete(lvs.Content.ToString());
                        ReloadLV();
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }
        }

        private void Path_link_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Path_link.Text != "")
            {
                for (int i = 0; i < GridSecond.Children.Count; i++)
                {
                    GridSecond.Children[i].IsEnabled = true;
                }
                ReloadLV();
            }
        }

        private void RTB_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (RTB.IsEnabled)
            {
                for (int i = 0; i < MainGrid.Children.Count; i++)
                {
                    MainGrid.Children[i].IsEnabled = true;
                }
                for (int i = 0; i < GridSecond.Children.Count; i++)
                {
                    GridSecond.Children[i].IsEnabled = true;
                }
                ReloadLV();
            }
        }

        private void ClosePr_Click(object sender, RoutedEventArgs e)
        {
            DisableScreen();
        }
        private bool DoesFileExist(string fil)
        {
            return File.Exists(fil);
        }
        private void CreateErrorMessage(string Error_text)
        {
            MessageBoxResult result = MessageBox.Show(Error_text, "Ошибка", MessageBoxButton.OK);
        }
        private void Add_new_file_Click(object sender, RoutedEventArgs e)
        {
            Add_File addf = new Add_File();
            if (addf.ShowDialog() == true)
            {
                if (!DoesFileExist($"{Path_link.Text}\\{addf.FileName}"))
                    File.Create($"{Path_link.Text}\\{addf.FileName}").Close();
                else
                    MessageBox.Show("Файл с таким именем уже существует");
                TextRange range;
                FileStream fStream;
                range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                fStream = new FileStream($"{Path_link.Text}\\{addf.FileName}", FileMode.Open);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                Browser.Address = $"{Path_link.Text}\\{addf.FileName}";
                Adress.Content = $"{Path_link.Text}\\{addf.FileName}";
                Browser.Reload();
                ReloadLV();
            }
            else
            {
                MessageBox.Show("Файл создать не удалось");
            }
        }
        private void Copy_file_Click(object sender, RoutedEventArgs e)
        {
            Copy_File_Link cfl = new Copy_File_Link();
            if (cfl.ShowDialog() == true)
            {
                if (!DoesFileExist($"{Path_link.Text}\\{cfl.FileName}"))
                {
                    string path1 = $"{Path_link.Text}\\{cfl.FileName}";
                    System.IO.File.WriteAllText(path1, cfl.HTML);
                }
                else
                    MessageBox.Show("Файл с таким именем уже существует");
                TextRange range;
                FileStream fStream;
                range = new TextRange(RTB.Document.ContentStart, RTB.Document.ContentEnd);
                fStream = new FileStream($"{Path_link.Text}\\{cfl.FileName}", FileMode.Open);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
                Adress.Content = $"{Path_link.Text}\\{cfl.FileName}";
                Browser.Address = Adress.Content.ToString();
                Browser.Reload();
                ReloadLV();
            }
            else
            {
                MessageBox.Show("Файл создать не удалось");
            }
        }
    }
}
