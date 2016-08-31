using System;
using System.ComponentModel;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Collections.Generic;
using DaeMatGo;
using System.Collections;
using System.Diagnostics;


/// <summary>
/// 해야할 일 : 카드 순회할때 추가 판까지(내 패에서 꺼낼 때 순회, 드로우한 카드에 대한 순회), 간헐적으로 내 패에서 낸 카드의 인덱스가 한 칸씩 밀려서 적용됨, 
/// 내 패에서 카드꺼낼 때 판에 안 내지고 그대로 씹히는 버그 
/// </summary>

namespace ImageAnimation
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window1 : Window
    {
        public Image SelectedImage { get; set; }

        //클래스로부터 리소스를 가져올 객체
        ResourceCard resourceCard = new ResourceCard();

        //이미지 경로
        string[] MyPath = new string[10];
        string[] EnemyPath = new string[10];
        string[] MatchPath = new string[8];

        public Window1()
        {
            InitializeComponent();
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            #region ShowAnim
            MoveAnim(InfoGrid, 1085, 0);
            #endregion

            #region SetResources
            resourceCard.ResourceCardList.AddRange(ResourceCard.ResourceCardImage);
            resourceCard.ResourceCardListCopy.AddRange(ResourceCard.ResourceCardImage);
            #endregion

            #region SetTransparentGrid
            //matchstack2
            for (int i = 0; i < 8; i++)
                match_stack2.Children.Add(new Image
                {
                    //Source = new BitmapImage(new Uri("Resources/Zion.T.JPG", UriKind.RelativeOrAbsolute)),
                    Width = 75,
                    Height = 75 * 1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = "This is None Image :)"
                });

            //matchstack3
            for (int i = 0; i < 8; i++)
                match_stack3.Children.Add(new Image
                {
                    //Source = new BitmapImage(new Uri("Resources/화투_뒷면.jpg", UriKind.RelativeOrAbsolute)),
                    Width = 75,
                    Height = 75 * 1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = "This is None Image :)"
                });

            #endregion

            #region CardAnimation
            Storyboard MenuReset = new Storyboard();

            DoubleAnimation withAnimation = new DoubleAnimation(60, new Duration(TimeSpan.Parse("0:0:0.2")));
            DoubleAnimation heightAnimation = new DoubleAnimation(90, new Duration(TimeSpan.Parse("0:0:0.2")));

            Storyboard.SetTargetProperty(withAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));

            MenuReset.Children.Add(withAnimation);
            MenuReset.Children.Add(heightAnimation);

            Storyboard ClickMenu = new Storyboard();

            DoubleAnimation _withAnimation = new DoubleAnimation(60 * 1.3, new Duration(TimeSpan.Parse("0:0:0.2")));
            DoubleAnimation _heightAnimation = new DoubleAnimation(90 * 1.3, new Duration(TimeSpan.Parse("0:0:0.2")));

            Storyboard.SetTargetProperty(_withAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTargetProperty(_heightAnimation, new PropertyPath(HeightProperty));

            ClickMenu.Children.Add(_withAnimation);
            ClickMenu.Children.Add(_heightAnimation);

            BeginStoryboard eventStoryboard1 = new BeginStoryboard();
            BeginStoryboard eventStoryboard2 = new BeginStoryboard();

            eventStoryboard1.Storyboard = ClickMenu;
            eventStoryboard2.Storyboard = MenuReset;

            // 이벤트 트리거
            EventTrigger eventTrigger = new EventTrigger(MouseEnterEvent);
            eventTrigger.Actions.Add(eventStoryboard1);

            EventTrigger eventTrigger2 = new EventTrigger(MouseLeaveEvent);
            eventTrigger2.Actions.Add(eventStoryboard2);
            #endregion

            #region SetEnemyDeck
            for (int i = 0; i < EnemyPath.Length; i++)
            {
                int idx = random.Next(0, resourceCard.ResourceCardListCopy.Count);
                EnemyPath[i] = resourceCard.ResourceCardListCopy[idx];
                resourceCard.ResourceCardListCopy.RemoveAt(idx);
                resourceCard.ResourceCardListCopy.Sort();

                for (int j = 0; j < i; j++)
                {
                    if (EnemyPath[j] == EnemyPath[i])
                    {
                        i--;
                        break;
                    }
                }
            }

            foreach (string s in EnemyPath)
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(s, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill,
                    Width = 60,
                    Height = 90,
                    Margin = new Thickness(-20, 0, 0, 0)
                };

                image.MouseEnter += new MouseEventHandler(MyImageMouseEnter);
                image.MouseLeave += new MouseEventHandler(MyImageMouseLeave);

                image.MouseLeftButtonUp += Enemy_Image_Click;
                
                // 이벤트 트리거 설정
                image.Triggers.Add(eventTrigger);
                image.Triggers.Add(eventTrigger2);

                your_stack.Children.Add(image);

                UpdateLayout();
            }
            #endregion

            #region SetPlayerStack
            for (int i = 0; i < MyPath.Length; i++)
            {
                int idx = random.Next(0, resourceCard.ResourceCardListCopy.Count);
                MyPath[i] = resourceCard.ResourceCardListCopy[idx];
                resourceCard.ResourceCardListCopy.RemoveAt(idx);
                resourceCard.ResourceCardListCopy.Sort();

                //remove repeatation
                for (int j = 0; j < i; j++)
                {
                    if (MyPath[i] == MyPath[j])
                    {
                        i--;
                        break;
                    }
                }
            }

            foreach (string s in MyPath)
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(s, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill,
                    Width = 60,
                    Height = 90,
                    Margin = new Thickness(-20, 0, 0, 0)
                };
                image.MouseEnter += new MouseEventHandler(MyImageMouseEnter);
                image.MouseLeave += new MouseEventHandler(MyImageMouseLeave);

                // 이벤트 트리거 설정
                image.Triggers.Add(eventTrigger);
                image.Triggers.Add(eventTrigger2);

                image.MouseLeftButtonUp += Image_Click;

                my_stack.Children.Add(image);
            }

            my_stack.Margin = new Thickness(20, 0, 20, 0);
            #endregion

            #region SetMatchStack
            BitmapImage MatchCardImage = null;

            for (int i = 0; i < MatchPath.Length; i++)
            {
                bool _isOverlap = false;

                int idx = random.Next(0, resourceCard.ResourceCardListCopy.Count);
                MatchPath[i] = resourceCard.ResourceCardListCopy[idx];
                resourceCard.ResourceCardListCopy.RemoveAt(idx);
                resourceCard.ResourceCardListCopy.Sort();


                //바닥에 뿌려질 카드와 내 덱의 중복제거
                foreach (string item in MyPath)
                {
                    if (MatchPath[i] == item)
                    {
                        _isOverlap = true;
                    }
                }

                //바닥에 뿌려질 카드와 상대 덱의 중복제거
                foreach (string item in EnemyPath)
                {
                    if (MatchPath[i] == item)
                    {
                        _isOverlap = true;
                    }
                }

                if (_isOverlap)
                {
                    i = i > 0 ? i-- : 0;
                }

                for (int j = 0; j < i; j++)
                {
                    if (MatchPath[i] == MatchPath[j])
                    {
                        i--;
                        break;
                    }
                }

            }

            foreach (var s in MatchPath)
            {
                MatchCardImage = new BitmapImage(new Uri(s, UriKind.RelativeOrAbsolute));

                Image image = new Image
                {
                    Source = MatchCardImage,
                    Stretch = Stretch.Fill,
                    Width = 75,
                    Height = 75 * 1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = MatchCardImage.ToString()
                };

                match_stack.Children.Add(image);
            }
            #endregion

            #region TurnOnOff
            my_turn_button.IsEnabled = false;
            your_turn_button.IsEnabled = false;

            switch (random.Next(0, 2))
            {
                case 0:
                    my_turn_button.IsChecked = true;
                    your_turn_button.IsChecked = false;

                    Panel.SetZIndex(your_stack, -1);
                    Panel.SetZIndex(my_stack, 3);
                    break;
                case 1:
                    my_turn_button.IsChecked = false;
                    your_turn_button.IsChecked = true;

                    Panel.SetZIndex(your_stack, 3);
                    Panel.SetZIndex(my_stack, -1);
                    break;
            }
            #endregion
            ShowGettableCard();
        }

        private void MyImageMouseLeave(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex((sender as Image), 0);
        }

        private void MyImageMouseEnter(object sender, MouseEventArgs e)
        {
            Panel.SetZIndex((sender as Image), 1);
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            bool _isCompleted = false; // 바닥에 놓인 카드가 하나라도 맞는게 있으면 True로 바뀜 없으면 함수를 종료시킴( 카드를 안냄 )
            Image[] CanMatchingCards = new Image[4]; //이 배열에 맞출수 있는 카드들이 저장될 곳, 월 하나당 4가지의 카드를 가지고 있음

            Image image = null;

            #region 내가 클릭한 이미지를 가져옴
            image = new Image
            {
                Source = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                Width = 75,
                Height = 75 * 1.5,
                Margin = new Thickness(97, 20, 97, 20),
                Tag = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)).ToString()
            };
            #endregion

            #region 판에서 맞는 월의 카드를 모두 가져옴
            int cnt = 0;
            foreach (Image element in match_stack.Children)
            {
                //이제 여긴 리스트만 저장시키고 작업을 계속할건지 지정 (_isCompleted를 통해)
                try
                {
                    if (element.Tag.ToString().Substring(0, 13) == image.Tag.ToString().Substring(0, 13))
                    {
                        CanMatchingCards[cnt++] = element;
                        if (!_isCompleted)
                        {
                            _isCompleted = true;
                        }
                    }
                }
                catch (Exception er)
                {
                    Debug.WriteLine(er.Message); // 에러메세지를 띄우지말고 디버깅용으로만 사용 ( 예외처리 차원 )
                }
            }
            #endregion

            #region 여기서 선택
            int eleCount = 0;
            while (CanMatchingCards[eleCount] != null) eleCount++;

            int selectIdx = 0;
            if (eleCount > 1)
            {
                selectIdx = CreateSelectWindow(CanMatchingCards, eleCount);
            }

            Image ChoosedCardImage = new Image
            {
                Width = 40,
                Height = 60,
                Margin = new Thickness(5, 0, 0, 0)
            };
            #endregion

            ChoosedCardImage = CanMatchingCards[selectIdx];

            #region 여기서 카드를 맞춤
            if (_isCompleted == true && ChoosedCardImage != null)
            {
                //카드가 붙음
                match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), image);
                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage) + 1);

                //matchstack에서 월이 맞아 붙은 카드
                Image GetImage = new Image
                {
                    Width = 40,
                    Height = 60,
                    Source = ChoosedCardImage.Source,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                //matchstack용 빈 카드
                Image NoneImage = new Image
                {
                    Width = 75,
                    Height = 75*1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = null
                };

                //matchstack2용 빈 카드
                Image NoneImage2 = new Image
                {
                    Width = 75,
                    Height = 75 * 1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = null
                };

                //판에 있던 맞은 카드
                switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(ChoosedCardImage.Tag.ToString())])
                {
                    case "광":
                        //광스택에 추가하고
                        myGwangStack.Children.Add(GetImage);

                        //match stack, match stack2에 각각 빈 공간을 차지하기위한 빈 이미지를 넣는다
                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        //낸 카드의 자리를 지워줌
                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        //판에 있던 맞은 카드를 지워줌
                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "띠":
                        myDdiStack.Children.Add(GetImage);

                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "끗":
                        myGgeutStack.Children.Add(GetImage);

                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "피":
                        myPiStack.Children.Add(GetImage);

                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "피 또는 끗":
                        Window2 window2 = new Window2();
                        window2.Show();
                        break;
                    default:
                        break;
                }

                //내 패에서 꺼낸 카드
                Image MyImage = new Image
                {
                    Source = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill,
                    Width = 40,
                    Height = 60,
                    Margin = new Thickness(5, 0, 0, 0),
                    Tag = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)).ToString(),
                };

                switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(MyImage.Tag.ToString())])
                {
                    case "광":
                        myGwangStack.Children.Add(MyImage);
                        break;
                    case "띠":
                        myDdiStack.Children.Add(MyImage);
                        break;
                    case "끗":
                        myGgeutStack.Children.Add(MyImage);
                        break;
                    case "피":
                        myPiStack.Children.Add(MyImage);
                        break;
                    case "피 또는 끗":
                        Window2 window2 = new Window2();
                        window2.Show();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region 판에 맞는 카드가 없는 경우
            if (!_isCompleted)
            {
                //이 안에서 카드를 놓았을 경우 true, 여기서도 못 넣으면 false
                bool __isComplete = false;

                for(int i = 0; i<match_stack.Children.Count; i++)
                {
                    //판이 비어있으면
                    if ((match_stack.Children[i] as Image).Source == null)
                    {
                        //넣어주고
                        match_stack.Children.Insert(match_stack.Children.IndexOf(match_stack.Children[i]), new Image
                        {
                            Width = 75,
                            Height = 75 * 1.5,
                            Source = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                            Margin = new Thickness(97, 20, 97, 20)
                        });
                        //밀려난 빈 이미지는 지워주기
                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(match_stack.Children[i]) + 1);

                        __isComplete = true;
                        break;
                    }
                }
                if (!__isComplete)
                {
                    //꽉찼으면
                    //첫 번째 추가판도 꽉 찼을 떄
                    //추가 판은 비어있는 이미지를 사용하지 않음 따라서 자식의 수로 판별가능
                    if (additional_match_stack.Children.Count >= 2)
                        _additional_match_stack.Children.Add(new Image
                        {
                            Width = 75,
                            Height = 75 * 1.5,
                            Source = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                            Margin = new Thickness(0, 20, 0, 20)
                        });
                    //첫 번째 추가판에 추가
                    else
                        _additional_match_stack.Children.Add(new Image
                        {
                            Width = 75,
                            Height = 75 * 1.5,
                            Source = new BitmapImage(new Uri(MyPath[my_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                            Margin = new Thickness(0, 20, 0, 20)
                        });
                }
            }
            #endregion

            //내 패에서 낸 카드를 없애줌
            my_stack.Children.RemoveAt(my_stack.Children.IndexOf(sender as UIElement));

            #region 드로우
            //Dummy_Image에서 랜덤으로 카드를 뽑음
            int idx = new Random().Next(0, resourceCard.ResourceCardListCopy.Count);
            string newCard = resourceCard.ResourceCardListCopy[idx];

            resourceCard.ResourceCardListCopy.RemoveAt(idx);
            resourceCard.ResourceCardListCopy.Sort();

            Image DrawnImage = new Image
            {
                Source = new BitmapImage(new Uri(newCard, UriKind.RelativeOrAbsolute)),
                Width = 40,
                Height = 60,
                Tag = newCard,
                Margin = new Thickness(5, 0, 0, 0)
            };

            MessageBox.Show(DrawnImage.Source.ToString());
            #endregion

            bool draw_found = false; // 바닥에 놓인 카드가 하나라도 맞는게 있으면 True로 바뀜 없으면 함수를 종료시킴( 카드를 안냄 )

            #region 판에서 가져올게 있으면 드로우한 카드를 추가함
            try
            {
                for(int i = 0; i<match_stack.Children.Count; i++)
                {
                    if ((match_stack.Children[i] as Image).Tag.ToString().Substring(0, 13) == DrawnImage.Tag.ToString().Substring(0, 13))
                    {
                        draw_found = true;                    
                        switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(DrawnImage.Tag.ToString())])
                        {
                            case "광":
                                //판에 추가하고
                                match_stack2.Children.Insert(i, DrawnImage);
                                match_stack2.Children.RemoveAt(i + 1);

                                //내 패로 가져오기
                                match_stack2.Children.Insert(i, new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = null
                                });
                                match_stack2.Children.RemoveAt(i + 1);
                                myGwangStack.Children.Add(DrawnImage);
                                break;
                            case "띠":
                                match_stack2.Children.Insert(i, DrawnImage);
                                match_stack2.Children.RemoveAt(i + 1);

                                match_stack2.Children.Insert(i, new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = null
                                });
                                match_stack2.Children.RemoveAt(i + 1);
                                myDdiStack.Children.Add(DrawnImage);
                                break;
                            case "끗":
                                match_stack2.Children.Insert(i, DrawnImage);
                                match_stack2.Children.RemoveAt(i + 1);

                                match_stack2.Children.Insert(i, new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = null
                                });
                                match_stack2.Children.RemoveAt(i + 1);
                                myGgeutStack.Children.Add(DrawnImage);
                                break;
                            case "피":
                                match_stack2.Children.Insert(i, DrawnImage);
                                match_stack2.Children.RemoveAt(i + 1);

                                match_stack2.Children.Insert(i, new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = null
                                });
                                match_stack2.Children.RemoveAt(i + 1);
                                myPiStack.Children.Add(DrawnImage);
                                break;
                            case "피 또는 끗":
                                Window2 window2 = new Window2();
                                window2.Show();
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }           
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
            }
            #endregion

            #region 드로우한 카드와 같은 것을 찾아서 판에 있는 카드를 추가함
            try
            {
                for(int i = 0; i<match_stack.Children.Count; i++)
                {
                    if ((match_stack.Children[i] as Image).Tag.ToString().Substring(0, 13) == DrawnImage.Tag.ToString().Substring(0, 13))
                    {
                        switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf((match_stack.Children[i] as Image).Tag.ToString())])
                        {
                            case "광":
                                //matchstack에 있는 카드의 정보를 받아서 새로운 사이즈로 만들어서 '광'자리에 집어 넣어줌
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = (match_stack.Children[i] as Image).Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = (match_stack.Children[i] as Image).Tag
                                });

                                //빈 공간을 만들어줄 카드
                                match_stack.Children.Insert(match_stack.Children.IndexOf((match_stack.Children[i] as Image)), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf((match_stack.Children[i] as Image)) + 1);
                                break;
                            case "띠":
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = (match_stack.Children[i] as Image).Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = (match_stack.Children[i] as Image).Tag
                                });

                                match_stack.Children.Insert(match_stack.Children.IndexOf((match_stack.Children[i] as Image)), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf((match_stack.Children[i] as Image)) + 1);
                                break;
                            case "끗":
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = (match_stack.Children[i] as Image).Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = (match_stack.Children[i] as Image).Tag
                                });

                                match_stack.Children.Insert(match_stack.Children.IndexOf((match_stack.Children[i] as Image)), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf((match_stack.Children[i] as Image)) + 1);
                                break;
                            case "피":
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = (match_stack.Children[i] as Image).Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = (match_stack.Children[i] as Image).Tag
                                });

                                match_stack.Children.Insert(match_stack.Children.IndexOf((match_stack.Children[i] as Image)), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf((match_stack.Children[i] as Image)) + 1);
                                break;
                            case "피 또는 끗":
                                Window2 window2 = new Window2();
                                window2.Show();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
            }
            #endregion

            #region 판에서 가져올게 없을 경우
            bool match_stack_found = false;
            bool addtional_match_stack_found = false;
            if (!draw_found)
            {
                //match_stack에 비어있는 공간이 있는지 확인한다.
                foreach(Image element in match_stack.Children)
                {
                    if(element.Source == null)
                    {
                        match_stack_found = true;
                        match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                        {
                            Source = DrawnImage.Source,
                            Width = 75,
                            Height = 75 * 1.5,
                            Margin = new Thickness(97, 20, 97, 20),
                            Tag = DrawnImage.Source.ToString()
                        });
                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                        break;
                    }
                }

                //match_stack에 빈 공간이 없으면 addtional_match_stack을 순회한다
                if (!match_stack_found)
                {
                    MessageBox.Show("addtional_match_stack 순회중 match_stack에 빈 공간 없음");
                    foreach(Image element in additional_match_stack.Children)
                    {
                        if(element.Source == null)
                        {
                            addtional_match_stack_found = true;
                            additional_match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                            {
                                Source = DrawnImage.Source,
                                Width = 75,
                                Height = 75 * 1.5,
                                Margin = new Thickness(97, 20, 97, 20),
                                Tag = DrawnImage.Source.ToString()
                            });
                            additional_match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                            break;
                        }
                    }
                }

                //additional_match_stack에 빈 공간이 없으면 _addtional_match_stack을 순회한다
                if (!addtional_match_stack_found)
                {
                    foreach (Image element in _additional_match_stack.Children)
                    {
                        if (element.Source == null)
                        {
                            MessageBox.Show("여기도 빈공간 있음!!");
                            _additional_match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                            {
                                Source = DrawnImage.Source,
                                Width = 75,
                                Height = 75 * 1.5,
                                Margin = new Thickness(97, 20, 97, 20),
                                Tag = DrawnImage.Source.ToString()
                            });
                            _additional_match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                            break;
                        }
                    }
                }
            }
            #endregion

            #region ShowGotCardCount
            MyGwangCount.Content = myGwangStack.Children.Count.ToString();
            MyDdiCount.Content = myDdiStack.Children.Count.ToString();
            MyGgeutCount.Content = myGgeutStack.Children.Count.ToString();
            MyPiCount.Content = myPiStack.Children.Count.ToString();

            yourGwangCount.Content = yourGwangStack.Children.Count.ToString();
            yourDdiCount.Content = yourDdiStack.Children.Count.ToString();
            yourGgeutCount.Content = yourGgeutStack.Children.Count.ToString();
            yourPiCount.Content = yourPiStack.Children.Count.ToString();

            #endregion

            #region 내 턴 끝남
            my_turn_button.IsChecked = false;
            your_turn_button.IsChecked = true;

            myDisableRectangle.Width = my_stack.Children.Count * 20 + 20;

            Panel.SetZIndex(my_stack, -1);
            Panel.SetZIndex(your_stack, 3);
            #endregion

            UpdateLayout();
        }

        private void Enemy_Image_Click(object sender, MouseButtonEventArgs e)
        {
            bool _isCompleted = false; // 바닥에 놓인 카드가 하나라도 맞는게 있으면 True로 바뀜 없으면 함수를 종료시킴( 카드를 안냄 )
            Image[] CanMatchingCards = new Image[4]; //이 배열에 맞출수 있는 카드들이 저장될 곳, 월 하나당 4가지의 카드를 가지고 있음

            Image image = null;

            #region 상대의 패에서 낸 카드의 정보를 가져옴
            image = new Image
            {
                Source = new BitmapImage(new Uri(EnemyPath[your_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                Width = 75,
                Height = 75 * 1.5,
                Margin = new Thickness(97, 20, 97, 20),
                Tag = new BitmapImage(new Uri(EnemyPath[your_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)).ToString()
            };
            #endregion

            #region 판에서 맞는 월의 카드를 모두 가져옴
            int cnt = 0;
            foreach (Image element in match_stack.Children)
            {
                //이제 여긴 리스트만 저장시키고 작업을 계속할건지 지정 (_isCompleted를 통해)
                try
                {
                    if (element.Tag.ToString().Substring(0, 13) == image.Tag.ToString().Substring(0, 13))
                    {
                        CanMatchingCards[cnt++] = element;
                        if (!_isCompleted)
                        {
                            _isCompleted = true;
                        }
                    }
                }
                catch (Exception er)
                {
                    Debug.WriteLine(er.Message); // 에러메세지를 띄우지말고 디버깅용으로만 사용 ( 예외처리 차원 )
                }
            }
            #endregion

            #region 여기서 선택
            int eleCount = 0;
            while (CanMatchingCards[eleCount] != null) eleCount++;

            int selectIdx = 0;
            if (eleCount > 1)
            {
                selectIdx = CreateSelectWindow(CanMatchingCards, eleCount);
            }

            Image ChoosedCardImage = new Image
            {
                Width = 40,
                Height = 60,
                Margin = new Thickness(5, 0, 0, 0)
            };
            #endregion

            ChoosedCardImage = CanMatchingCards[selectIdx];

            #region 여기서 카드를 맞춤
            if (_isCompleted == true && ChoosedCardImage != null)
            {
                //카드가 붙음
                match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), image);
                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage) + 1);

                //matchstack에서 월이 맞아 붙은 카드
                Image GetImage = new Image
                {
                    Width = 40,
                    Height = 60,
                    Source = ChoosedCardImage.Source,
                    Margin = new Thickness(5, 0, 0, 0)
                };

                //matchstack용 빈 카드
                Image NoneImage = new Image
                {
                    Width = 75,
                    Height = 75 * 1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = null
                };

                //matchstack2용 빈 카드
                Image NoneImage2 = new Image
                {
                    Width = 75,
                    Height = 75 * 1.5,
                    Margin = new Thickness(97, 20, 97, 20),
                    Tag = null
                };

                //판에 있던 맞은 카드
                switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(ChoosedCardImage.Tag.ToString())])
                {
                    case "광":
                        //광스택에 추가하고
                        yourGwangStack.Children.Add(GetImage);

                        //match stack, match stack2에 각각 빈 공간을 차지하기위한 빈 이미지를 넣는다
                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        //낸 카드의 자리를 지워줌
                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        //판에 있던 맞은 카드를 지워줌
                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "띠":
                        yourDdiStack.Children.Add(GetImage);

                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "끗":
                        yourGgeutStack.Children.Add(GetImage);

                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "피":
                        yourPiStack.Children.Add(GetImage);

                        match_stack2.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage2);
                        match_stack.Children.Insert(match_stack.Children.IndexOf(ChoosedCardImage), NoneImage);

                        match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));

                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(ChoosedCardImage));
                        break;
                    case "피 또는 끗":
                        Window2 window2 = new Window2();
                        window2.Show();
                        break;
                    default:
                        break;
                }

                //내 패에서 꺼낸 카드
                Image YourImage = new Image
                {
                    Source = new BitmapImage(new Uri(EnemyPath[your_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill,
                    Width = 40,
                    Height = 60,
                    Margin = new Thickness(5, 0, 0, 0),
                    Tag = new BitmapImage(new Uri(MyPath[your_stack.Children.IndexOf(sender as UIElement)], UriKind.RelativeOrAbsolute)).ToString(),
                };

                switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(YourImage.Tag.ToString())])
                {
                    case "광":
                        myGwangStack.Children.Add(YourImage);
                        break;
                    case "띠":
                        myDdiStack.Children.Add(YourImage);
                        break;
                    case "끗":
                        myGgeutStack.Children.Add(YourImage);
                        break;
                    case "피":
                        myPiStack.Children.Add(YourImage);
                        break;
                    case "피 또는 끗":
                        Window2 window2 = new Window2();
                        window2.Show();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region 드로우
            //Dummy_Image에서 랜덤으로 카드를 뽑음
            int idx = new Random().Next(0, resourceCard.ResourceCardListCopy.Count);

            //뽑아준 것은 카드목록에서 제거해줌
            resourceCard.ResourceCardListCopy.RemoveAt(idx);
            resourceCard.ResourceCardListCopy.Sort();

            Image DrawnImage = new Image
            {
                Source = new BitmapImage(new Uri(resourceCard.ResourceCardListCopy[idx], UriKind.RelativeOrAbsolute)),
                Width = 40,
                Height = 60,
                Tag = resourceCard.ResourceCardListCopy[idx],
                Margin = new Thickness(5, 0, -30, 0)
            };

            MessageBox.Show(DrawnImage.Source.ToString());
            #endregion

            bool draw_found = false; // 바닥에 놓인 카드가 하나라도 맞는게 있으면 True로 바뀜 없으면 함수를 종료시킴( 카드를 안냄 )

            #region 판에서 가져올게 있으면 '드로우한 카드'를 추가함
            try
            {
                foreach(Image element in match_stack.Children)
                {
                    if (element.Source.ToString().Substring(0, 13) == DrawnImage.Source.ToString().Substring(0, 13))
                    {
                        draw_found = true;
                        switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(DrawnImage.Tag.ToString())])
                        {
                            case "광":
                                //드로우한 카드를 판에 추가하고
                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), DrawnImage);
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);

                                //내 패로 가져오기
                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                });

                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                yourGwangStack.Children.Add(DrawnImage);
                                break;
                            case "띠":
                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), DrawnImage);
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);

                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                });
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                yourDdiStack.Children.Add(DrawnImage);
                                break;
                            case "끗":
                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), DrawnImage);
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);

                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                });
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                yourGgeutStack.Children.Add(DrawnImage);
                                break;
                            case "피":
                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), DrawnImage);
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);

                                match_stack2.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                });
                                match_stack2.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                yourPiStack.Children.Add(DrawnImage);
                                break;
                            case "피 또는 끗":
                                Window2 window2 = new Window2();
                                window2.Show();
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
            }
            #endregion

            #region 드로우한 카드와 같은 것을 찾아서 '판에 있는 카드'를 추가함
            try
            {
                foreach(Image element in match_stack.Children)
                {
                    if (element.Source.ToString().Substring(0, 13) == DrawnImage.Source.ToString().Substring(0, 13))
                    {
                        switch (ResourceCard.ResourceCardProperty[resourceCard.ResourceCardList.IndexOf(element.Source.ToString())])
                        {
                            case "광":
                                //matchstack에 있는 카드의 정보를 받아서 새로운 사이즈로 만들어서 '광'자리에 집어 넣어줌
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = element.Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = element.Tag
                                });

                                //빈 공간을 만들어줄 카드
                                match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                break;
                            case "띠":
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = element.Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = element.Tag
                                });

                                match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                break;
                            case "끗":
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = element.Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = element.Tag
                                });

                                match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                break;
                            case "피":
                                myGwangStack.Children.Add(new Image
                                {
                                    Width = 40,
                                    Height = 60,
                                    Source = element.Source,
                                    Margin = new Thickness(5, 0, -30, 0),
                                    Tag = element.Tag
                                });

                                match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                                {
                                    Width = 75,
                                    Height = 75 * 1.5,
                                    Margin = new Thickness(97, 20, 97, 20),
                                    Tag = "This is None Image :)"
                                });

                                match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                                break;
                            case "피 또는 끗":
                                Window2 window2 = new Window2();
                                window2.Show();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);
            }
            #endregion

            #region 판에서 가져올게 없을 경우
            bool match_stack_found = false;
            bool addtional_match_stack_found = false;
            if (!draw_found)
            {
                //match_stack에 비어있는 공간이 있는지 확인한다.
                foreach (Image element in match_stack.Children)
                {
                    if (element.Source == null)
                    {
                        match_stack_found = true;
                        match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                        {
                            Source = DrawnImage.Source,
                            Width = 75,
                            Height = 75 * 1.5,
                            Margin = new Thickness(97, 20, 97, 20),
                            Tag = DrawnImage.Source.ToString()
                        });
                        match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                        break;
                    }
                }

                //match_stack에 빈 공간이 없으면 addtional_match_stack을 순회한다
                if (!match_stack_found)
                {
                    MessageBox.Show("addtional_match_stack 순회중 match_stack에 빈 공간 없음");
                    foreach (Image element in additional_match_stack.Children)
                    {
                        if (element.Source == null)
                        {
                            addtional_match_stack_found = true;
                            additional_match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                            {
                                Source = DrawnImage.Source,
                                Width = 75,
                                Height = 75 * 1.5,
                                Margin = new Thickness(97, 20, 97, 20),
                                Tag = DrawnImage.Source.ToString()
                            });
                            additional_match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                            break;
                        }
                    }
                }

                //additional_match_stack에 빈 공간이 없으면 _addtional_match_stack을 순회한다
                if (!addtional_match_stack_found)
                {
                    foreach (Image element in _additional_match_stack.Children)
                    {
                        if (element.Source == null)
                        {
                            MessageBox.Show("여기도 빈공간 있음!!");
                            _additional_match_stack.Children.Insert(match_stack.Children.IndexOf(element), new Image
                            {
                                Source = DrawnImage.Source,
                                Width = 75,
                                Height = 75 * 1.5,
                                Margin = new Thickness(97, 20, 97, 20),
                                Tag = DrawnImage.Source.ToString()
                            });
                            _additional_match_stack.Children.RemoveAt(match_stack.Children.IndexOf(element) + 1);
                            break;
                        }
                    }
                }
            }
            #endregion

            #region ShowGotCardCount
            yourGwangCount.Content = yourGwangStack.Children.Count.ToString();
            yourDdiCount.Content = yourDdiStack.Children.Count.ToString();
            yourGgeutCount.Content = yourGgeutStack.Children.Count.ToString();
            yourPiCount.Content = yourPiStack.Children.Count.ToString();

            yourGwangCount.Content = yourGwangStack.Children.Count.ToString();
            yourDdiCount.Content = yourDdiStack.Children.Count.ToString();
            yourGgeutCount.Content = yourGgeutStack.Children.Count.ToString();
            yourPiCount.Content = yourPiStack.Children.Count.ToString();

            #endregion

            #region 상대 턴 끝남
            your_turn_button.IsChecked = false;
            my_turn_button.IsChecked = true;

            yourDisableRectangle.Width = your_stack.Children.Count * 20 + 20;

            Panel.SetZIndex(your_stack, -1);
            Panel.SetZIndex(my_stack, 3);
            #endregion
        }

        private int CreateSelectWindow(Image[] CanSelectCardList, int idx)
        {
            SelectWindow sWindow = new SelectWindow();

            // 왼쪽 여백 + 50 * 카드 장수 + 오른쪽 여백
            sWindow.Width = 20 + 210 * idx + 20;
            sWindow.Height = 300;
            sWindow.Background = Brushes.Gray;
            sWindow.ResizeMode = ResizeMode.NoResize;
            sWindow.WindowStyle = WindowStyle.None;
            sWindow.Left = (this.Width / 2) - sWindow.Width / 2;

            double sideMargin = 10;
            double tbMargin = 10;

            foreach (Image resource in CanSelectCardList)
            {
                if (resource == null) break;

                Image nImage = new Image
                {
                    Width = 200,
                    Height = 250,
                    Margin = new Thickness(sideMargin, tbMargin, sideMargin, tbMargin),
                    Source = resource.Source,
                };
                sWindow.CardList.Children.Add(nImage);
            }

            sWindow.ShowDialog();

            return sWindow.selectedIndex;
        }

        #region Anims
        public void MoveAnim(Grid target, double newX, double newY)
        {
            Vector offset = VisualTreeHelper.GetOffset(target);

            var top = offset.Y;
            var left = offset.X;

            TranslateTransform trans = new TranslateTransform();
            target.RenderTransform = trans;

            DoubleAnimation anim1 = new DoubleAnimation(0, newY - top, TimeSpan.FromSeconds(0.5));
            DoubleAnimation anim2 = new DoubleAnimation(0, newX - left, TimeSpan.FromSeconds(0.5));

            trans.BeginAnimation(TranslateTransform.YProperty, anim1);
            trans.BeginAnimation(TranslateTransform.XProperty, anim2);
        }
        #endregion

        private void ShowGettableCard()
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.StartPoint = new Point(0, 0);
            linearGradientBrush.EndPoint = new Point(1, 1);

            GradientStop gradientStop = new GradientStop();
            gradientStop.Offset = 0;
            gradientStop.Color = Color.FromArgb(120, (byte)((int)0), (byte)((int)0), (byte)((int)0));

            GradientStop _gradientStop = new GradientStop();
            _gradientStop.Offset = 0.5;
            _gradientStop.Color = Color.FromArgb(120, (byte)((int)0), (byte)((int)0), (byte)((int)0));

            GradientStopCollection gradientStopCollection = new GradientStopCollection();
            gradientStopCollection.Add(gradientStop);
            gradientStopCollection.Add(_gradientStop);

            linearGradientBrush.GradientStops = gradientStopCollection;

            foreach (Image element in my_stack.Children)
            {
                foreach(Image match_element in match_stack.Children)
                {
                    if(element.Source.ToString().Substring(0, 13) == match_element.Source.ToString().Substring(0, 13))
                    {
                        if (element.OpacityMask == null)
                        {
                            element.OpacityMask = linearGradientBrush;
                            break;
                        }
                    }
                }
            }

            foreach(Image element in my_stack.Children)
            {
                if(element.OpacityMask == null)
                {
                    element.OpacityMask = linearGradientBrush;
                }
                else
                {
                    element.OpacityMask = null;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ScoreGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Panel.SetZIndex((sender as UIElement), 3);

            DropShadowBitmapEffect myDropShadowEffect = new DropShadowBitmapEffect();
            // Set the color of the shadow to Black.
            Color myShadowColor = new Color();
            myShadowColor.ScA = 1;
            myShadowColor.ScB = 0;
            myShadowColor.ScG = 0;
            myShadowColor.ScR = 0;
            myDropShadowEffect.Color = myShadowColor;

            // Set the direction of where the shadow is cast to 320 degrees.
            myDropShadowEffect.Direction = 320;

            // Set the depth of the shadow being cast.
            myDropShadowEffect.ShadowDepth = 5;

            // Set the shadow softness to the maximum (range of 0-1).
            myDropShadowEffect.Softness = 1;
            // Set the shadow opacity to half opaque or in other words - half transparent.
            // The range is 0-1.
            myDropShadowEffect.Opacity = 0.5;

            (sender as UIElement).BitmapEffect = myDropShadowEffect;
        }

        private void ScoreGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Panel.SetZIndex((sender as UIElement), 1);

            (sender as UIElement).BitmapEffect = null;
        }
    }
}