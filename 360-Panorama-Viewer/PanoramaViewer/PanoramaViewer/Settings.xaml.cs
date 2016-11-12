using System;
using System.Collections.Generic;
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

namespace PanoramaViewer
{
    /// <summary>
    /// Settings.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            media.Play();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            media.Position = new TimeSpan(0, 0, 1);
            media.Play();
        }
    }
}
