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

namespace Trix_UPD
{
    /// <summary>
    /// Логика взаимодействия для Add_File.xaml
    /// </summary>
    public partial class Add_File : Window
    {
        public Add_File()
        {
            InitializeComponent();
        }
        private void CloseWindow(MainWindow main, string path)
        {
            main.Show();
            main.ReloadLV();
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
        private void CreateErrorMessage(string Error_text)
        {
            MessageBoxResult result = MessageBox.Show(Error_text, "Ошибка", MessageBoxButton.OK);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsStringLatin(namefilelabel.Text) == false)
            {
                CreateErrorMessage("Название проекта не написано на латинице или пустое. Проверьте введенные данные еще раз");
                return;
            }
            if(extensionList.SelectedItem != null) {
                this.DialogResult = true;
            }
        }
        public string FileName
        {
            get { return $"{namefilelabel.Text}{extensionList.Text}"; }
        }
        public ComboBoxItem selectedItem;
        private void extensionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            selectedItem = (ComboBoxItem)comboBox.SelectedItem;
        }
    }
}
