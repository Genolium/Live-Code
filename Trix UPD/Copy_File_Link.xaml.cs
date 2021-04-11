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

namespace Trix_UPD
{
    /// <summary>
    /// Логика взаимодействия для Copy_File_Link.xaml
    /// </summary>
    public partial class Copy_File_Link : Window
    {
        public Copy_File_Link()
        {
            InitializeComponent();
        }
        private void CreateErrorMessage(string Error_text)
        {
            MessageBoxResult result = MessageBox.Show(Error_text, "Ошибка", MessageBoxButton.OK);
        }
        private bool IsSiteWorking(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch
            {
                CreateErrorMessage("Не получилось скопировать сайт.");
                return false;
            }
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
        private string CorrectString(string sentence)
        {
            char a = '\u005c';
            string replacedname = sentence;
            replacedname = replacedname.Remove(0, check.Length);
            replacedname = replacedname.Replace(".", "");
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
            if (IsStringLatin(Name.Text) == false)
            {
                CreateErrorMessage("Название проекта не написано на латинице или пустое. Проверьте введенные данные еще раз");
                return;
            }
            string urlAddress = Name.Text;
            if (IsSiteWorking(urlAddress) == true)
            {
                string html = GetHTML(urlAddress);
                HTML = html;
                this.DialogResult = true;
            }
            else
            {
                CreateErrorMessage("Отсутствует интернет соединение или сайт недоступен");
                return;
            }
        }
        public string HTML { get; set; }
        public string FileName
        {
            get { return $"{CorrectString(Name.Text)}.html"; }
        }
    }
}
    

