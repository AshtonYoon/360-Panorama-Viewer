using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaeMatGo
{
    /// <summary>
    /// SelectWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectWindow : Window
    {
        public Image SelectedCard { get; set; }
        public int selectedIndex { get; private set; }
        Image[] CardImageList = null;

        public SelectWindow()
        {
            InitializeComponent();
            this.Loaded += SelectWindow_Loaded;
        }

        private void SelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UIElementCollection eleCol = CardList.Children;
            CardImageList = new Image[eleCol.Count];

            int cnt = 0;
            foreach (UIElement ele in eleCol)
            {
                Image img = (Image)ele;

                CardImageList[cnt] = img;
                CardImageList[cnt++].MouseDown += Image_Select;
            }
        }

        private void Image_Select(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectedCard = sender as Image;
                selectedIndex = 0;

                foreach (var a in CardImageList)
                {
                    if (a.Source == SelectedCard.Source) break;
                    selectedIndex++;
                }

                this.Close();
            }
            catch
            {

            }
        }
    }
}
