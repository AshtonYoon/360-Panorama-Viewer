using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PanoramaViewer;
using System.Security.Policy;

namespace PanoramaViewer
{
    /// <summary>
    /// InputUri.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputUri : Window
    {
        public InputUri()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ImageUrl = Uri.Text;
            this.Close();
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(Uri.IsFocused && e.Key == Key.Enter)
            {
                MainWindow.ImageUrl = Uri.Text;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //AppDomain appdomain = AppDomain.CreateDomain("dependencyData", null);
            //appdomain.SetData(typeof(Point).ToString(), null);
            this.Close();
        }

        private void Uri_GotFocus(object sender, RoutedEventArgs e)
        {
            Uri.Text = null;
        }
    }
}
