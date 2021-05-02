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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
namespace InteractiveLearningSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int ID = 0;
        //UserControl uc;
        public static T FindChild<T>(DependencyObject parent, string childName)
where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        public void ParseStuff(StreamReader reader, ref WrapPanel panel, Style style)
        {

            String s = reader.ReadLine();
            TextBlock largetext = new TextBlock();
            largetext.Style = style;
            largetext.TextWrapping = System.Windows.TextWrapping.Wrap;
            largetext.FontSize = largetext.FontSize * 1.5;
            largetext.Text = s;
            largetext.Text += "\n";
            largetext.FontWeight = FontWeights.Bold;
            panel.Children.Add(largetext);

            s = reader.ReadLine();
            largetext = new TextBlock();
            largetext.Style = style;
            largetext.TextWrapping = System.Windows.TextWrapping.Wrap;
            while (s != null)
            {
                if (s.Contains("<img>") || s.Contains("<list>") || s.Contains("<title>"))
                {
                    largetext.Text += "\n";
                    panel.Children.Add(largetext);
                    largetext = new TextBlock();
                    largetext.TextWrapping = System.Windows.TextWrapping.Wrap;
                    largetext.Style = style;
                    if (s.Contains("<title>"))
                    {
                        s = reader.ReadLine();
                        largetext.FontSize = largetext.FontSize * 1.25;
                        largetext.FontWeight = FontWeights.Bold;
                        largetext.Text = s;
                        panel.Children.Add(largetext);
                        largetext = new TextBlock();
                        largetext.TextWrapping = System.Windows.TextWrapping.Wrap;
                        largetext.Style = style;
                    }
                    else
                    {
                        if (s.Contains("<img>"))
                        {
                            s = reader.ReadLine();
                            s = "Resources/" + s;
                            Image img = new Image();
                            try
                            {
                                Uri fileUri = new Uri(@s, UriKind.Relative);
                                img.Source = new BitmapImage(fileUri);
                                img.Stretch = System.Windows.Media.Stretch.Uniform;
                                img.StretchDirection = System.Windows.Controls.StretchDirection.DownOnly;
                                panel.Children.Add(img);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        else
                        {
                            TextBlock lst = new TextBlock();
                            lst.TextWrapping = System.Windows.TextWrapping.Wrap;
                            lst.Style = style;
                            s = reader.ReadLine();
                            while (s != "</list>")
                            {
                                lst.Text += "--";
                                lst.Text += s;
                                lst.Text += "\n";
                                s = reader.ReadLine();
                            }
                            panel.Children.Add(lst);

                        }
                    }
                    
                }
                else
                {
                    largetext.Text += s;
                }
                largetext.Text += "\n";
                s = reader.ReadLine();
                
            }

            panel.Children.Add(largetext);
        }
        public void Update()
        {
            WrapPanel pnl = FindChild<WrapPanel>(this, "cont");
            pnl.Children.Clear();
            if (ID < 6)
            {
                string pth = "Resources/" + ID.ToString() + ".txt";
                StreamReader reader;
                try
                {
                    reader = new StreamReader(@pth);
                    Style style;
                    if (ID == 0)
                    { style = this.FindResource("mainPageStyle") as Style; }
                    else
                    { style = this.FindResource("contentPageStyle") as Style; }
                    ParseStuff(reader, ref pnl, style);
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Grid gr = new Grid();
                ColumnDefinition gridCol1 = new ColumnDefinition();
                gridCol1.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition gridCol2 = new ColumnDefinition();
                gridCol2.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition gridCol3 = new ColumnDefinition();
                gridCol3.Width = new GridLength(1, GridUnitType.Star);
                gr.ColumnDefinitions.Add(gridCol1);
                gr.ColumnDefinitions.Add(gridCol2);
                gr.ColumnDefinitions.Add(gridCol3);
                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(50);
                
                RowDefinition gridRow2 = new RowDefinition();
                MediaElement player = new MediaElement();
                gr.RowDefinitions.Add(gridRow1);
                gr.RowDefinitions.Add(gridRow2);

                string s = ID.ToString()+".mp4";
                s = "Videos/" + s;
                player.Source = new Uri(@s, UriKind.Relative);
                player.Name = "player";
                player.LoadedBehavior = System.Windows.Controls.MediaState.Manual;
                Grid.SetRow(player, 1);
                Grid.SetColumn(player, 0);
                Grid.SetColumnSpan(player, 3);
                gr.Children.Add(player);
                player.Play();
                player.Stop();

                Button play = new Button();
                play.Click += PlayButton_Click;
                Image playImg = new Image();
                playImg.Source=new BitmapImage(new Uri(@"/Images/control_play_blue.png", UriKind.Relative));
                play.Content = playImg;
                Grid.SetRow(play, 0);
                Grid.SetColumn(play, 1);
                gr.Children.Add(play);

                Button pause = new Button();
                pause.Click += PauseButton_Click;
                Image pauseImg = new Image();
                pauseImg.Source = new BitmapImage(new Uri(@"/Images/control_pause_blue.png", UriKind.Relative));
                pause.Content = pauseImg;
                Grid.SetRow(pause, 0);
                Grid.SetColumn(pause, 0);
                gr.Children.Add(pause);

                Button stop = new Button();
                stop.Click += StopButton_Click;
                Image stopImg = new Image();
                stopImg.Source = new BitmapImage(new Uri(@"/Images/control_stop_blue.png", UriKind.Relative));
                stop.Content = stopImg;
                Grid.SetRow(stop, 0);
                Grid.SetColumn(stop, 2);
                gr.Children.Add(stop);

                pnl.Children.Add(gr);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement player = FindChild<MediaElement>(this, "player");
            player.Play();
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement player = FindChild<MediaElement>(this, "player");
            player.Stop();
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement player = FindChild<MediaElement>(this, "player");
            player.Pause();
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Show();
            Update();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            ID = Convert.ToInt32(b.Name[1].ToString());
            Update();
        }
    }
}
