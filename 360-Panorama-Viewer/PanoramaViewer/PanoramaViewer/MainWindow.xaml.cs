using System.Windows;
using Microsoft.Win32;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;

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

        private enum MediaType { Image, Video, Else };
        private MediaType CheckMediaType(string filename)
        {
            Regex imageRegex = new Regex("\\.(jpe?g|png|bmp)$");
            Regex videoRegex = new Regex("\\.(avi|mp4|gif|flv|wmv|)$");

            if (imageRegex.IsMatch(filename))
                return MediaType.Image;
            else if (videoRegex.IsMatch(filename))
                return MediaType.Video;
            else
                return MediaType.Else;
        }

        private void LoadByLocal_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif"+
            //    "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
            //              " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; ";


            Nullable<bool> OpenFileDialogResult = openFileDialog.ShowDialog();
            if (OpenFileDialogResult == true)
            {
                PanoramaView panoramaView = new PanoramaView();

                if (CheckMediaType(openFileDialog.FileName) == MediaType.Image)
                {
                    panoramaView.PanoramaImage = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                    RenderOptions.SetBitmapScalingMode(panoramaView.PanoramaImage, BitmapScalingMode.HighQuality);
                }
                else if (CheckMediaType(openFileDialog.FileName) == MediaType.Video)
                {
                    panoramaView.panoramaVideo = new MediaElement
                    {
                        Source = new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute),
                        LoadedBehavior = MediaState.Play,
                        IsMuted = false
                    };
                }
                else
                {

                }
                ViewPortGrid.Children.Add(panoramaView);

                subLoadingText.Visibility = Visibility.Visible;
                subLoadingText2.Visibility = Visibility.Visible;
                LoadingText.Visibility = Visibility.Visible;
                LoadingImage.Visibility = Visibility.Visible;

                MenuGrid.Visibility = Visibility.Collapsed;
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

                subLoadingText.Visibility = Visibility.Visible;
                subLoadingText2.Visibility = Visibility.Visible;
                LoadingText.Visibility = Visibility.Visible;
                LoadingImage.Visibility = Visibility.Visible;

                MenuGrid.Visibility = Visibility.Collapsed;
                Panel.SetZIndex(MenuGrid, -1);
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
            if (ViewPortGrid.Children.Count > 0)
            {
                ViewPortGrid.Children.RemoveAt(ViewPortGrid.Children.Count - 1);

                subLoadingText.Visibility = Visibility.Hidden;
                subLoadingText2.Visibility = Visibility.Hidden;
                LoadingText.Visibility = Visibility.Hidden;
                LoadingImage.Visibility = Visibility.Hidden;

                MenuGrid.Visibility = Visibility.Visible;
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

        public void MoveAnim(Grid target, double newX, double newY)
        {
            Vector offset = VisualTreeHelper.GetOffset(target);

            var top = offset.Y;
            var left = offset.X;

            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;

            DoubleAnimation anim1 = new DoubleAnimation(0, newY - top, TimeSpan.FromSeconds(0.8));
            DoubleAnimation anim2 = new DoubleAnimation(0, newX - left, TimeSpan.FromSeconds(0.8));

            anim1.AccelerationRatio = 0.1;
            anim1.DecelerationRatio = 0.9;

            anim2.AccelerationRatio = 0.1;
            anim2.DecelerationRatio = 0.9;

            trans.BeginAnimation(TranslateTransform.YProperty, anim1);
            trans.BeginAnimation(TranslateTransform.XProperty, anim2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MoveAnim(load_from_local_file, 0, 0);
            MoveAnim(load_from_url, 0, 0);
        }
    }
}
