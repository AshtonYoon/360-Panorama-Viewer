using System.Windows;
using Microsoft.Win32;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;

namespace PanoramaViewer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadByLocal_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            Nullable<bool> OpenFileDialogResult = openFileDialog.ShowDialog();
            if (OpenFileDialogResult == true)
            {
                PanoramaView panoramaView = new PanoramaView();
                panoramaView.PanoramaImage = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                RenderOptions.SetBitmapScalingMode(panoramaView.PanoramaImage, BitmapScalingMode.HighQuality);

                ViewPortGrid.Children.Add(panoramaView);

                Panel.SetZIndex(MenuGrid, -1);
            }
            else
            {
                //Doing Nothing
            }
        }

        public static string ImageUrl { get; set; }
        private void LoadByUri_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputUri inputUri = new InputUri();
                inputUri.ShowDialog();                

                PanoramaView panoramaView = new PanoramaView();
                panoramaView.PanoramaImage = new BitmapImage(new Uri(ImageUrl, UriKind.RelativeOrAbsolute));
                RenderOptions.SetBitmapScalingMode(panoramaView.PanoramaImage, BitmapScalingMode.HighQuality);

                ViewPortGrid.Children.Add(panoramaView);

                Panel.SetZIndex(MenuGrid, -1);

                media.Visibility = Visibility.Visible;
            }
            catch(Exception er)
            {
                MessageBox.Show(er.StackTrace + "\n올바른 주소를 입력해주세요");
            }
        }       

        private void MinimizeButtonGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        bool IsMaximized = false;
        private void MaximizeButtonGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsMaximized)
            {
                this.WindowState = WindowState.Maximized;
                IsMaximized = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                IsMaximized = false;
            }
        }

        private void CloseButtonGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                App.Current.Windows[intCounter].Close();
        }

        //Back Button Click
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            media.Visibility = Visibility.Hidden;
            if (ViewPortGrid.Children.Count > 0)
            {
                ViewPortGrid.Children.RemoveAt(ViewPortGrid.Children.Count - 1);
                Panel.SetZIndex(MenuGrid, 0);
            }
            else
            {
                //Doing Nothing
            }
        }

        private void MenuGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            media.Position = new TimeSpan(0, 0, 1);
            media.Play();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            media.Visibility = Visibility.Hidden;
            media.Play();
        }
    }
}
