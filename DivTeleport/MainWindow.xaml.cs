using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System;

namespace DivTeleport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte[] fileBytes;
        long indX = 0;
        long indY = 0;
        long indM = 0;
        int XcorInMap = 0;
        int YcorInMap = 0;
        int selectWorld = 0;
        int world = 0;
        int XTileCor, YTileCor;
        int scrollOffset;
        int markYpos = 0;
        int markXpos = 0;
        double scaleImg;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("data.000"))
            {
                fileBytes = File.ReadAllBytes("data.000");
                long index = 0;
                for (int i = 0; i < fileBytes.Length - 8; i++)
                {
                    if (fileBytes[i + 0] == 0x6E &&
                        fileBytes[i + 1] == 0x65 &&
                        fileBytes[i + 2] == 0x77 &&
                        fileBytes[i + 3] == 0x20 &&
                        fileBytes[i + 4] == 0x6E &&
                        fileBytes[i + 5] == 0x70 &&
                        fileBytes[i + 6] == 0x63 &&
                        fileBytes[i + 7] == 0x00)
                    {
                        index = i;
                        break;
                    }
                }
                System.Diagnostics.Debug.WriteLine("index1 " + index);
                index += 12;
                indX = index;
                byte x1 = fileBytes[index++];
                byte x2 = fileBytes[index++];
                index += 2;
                indY = index;
                byte y1 = fileBytes[index++];
                byte y2 = fileBytes[index++];
                XcorInMap = x1 + x2 * 256;
                YcorInMap = y1 + y2 * 256;
                for (int i = (int)index; i < fileBytes.Length - 20; i++)
                {
                    if (fileBytes[i + 0] == 0xff &&
                        fileBytes[i + 1] == 0xff &&
                        fileBytes[i + 2] == 0xff &&
                        fileBytes[i + 3] == 0xff &&
                        fileBytes[i + 4] == 0xff &&
                        fileBytes[i + 5] == 0xff &&
                        fileBytes[i + 6] == 0xff &&
                        fileBytes[i + 7] == 0xff &&
                        fileBytes[i + 8] == 0xff &&
                        fileBytes[i + 9] == 0xff &&
                        fileBytes[i + 10] == 0xff &&
                        fileBytes[i + 11] == 0xff &&
                        fileBytes[i + 12] == 0xff &&
                        fileBytes[i + 13] == 0xff &&
                        fileBytes[i + 14] == 0xff &&
                        fileBytes[i + 15] == 0xff &&
                        fileBytes[i + 16] == 0xff &&
                        fileBytes[i + 17] == 0xff &&
                        fileBytes[i + 18] == 0xff &&
                        fileBytes[i + 19] == 0xff)
                    {
                        index = i;
                        System.Diagnostics.Debug.WriteLine("index2 " + index);
                        break;
                    }
                }
                index += 32;
                world = fileBytes[index];
                indM = index;
                System.Diagnostics.Debug.WriteLine("world " + world);
                System.Diagnostics.Debug.WriteLine("X " + XcorInMap + "  Y " + YcorInMap);
                System.Diagnostics.Debug.WriteLine("index3 " + index);
                XTileCor = XcorInMap / 64;
                YTileCor = YcorInMap / 64;
                scaleImg = ScrV.ActualWidth / 512;
                scrollOffset = (int)(ScrV.VerticalOffset / scaleImg);
                markYpos = (int)(YTileCor * scaleImg) + 3 + (int)ScrV.VerticalOffset;
                Canvas.SetTop(mark, markYpos - (int)ScrV.VerticalOffset);
                markXpos = (int)(XTileCor * scaleImg) - 7;
                Canvas.SetLeft(mark, markXpos);
                TextBox.Text = "X: " + XTileCor.ToString().PadLeft(4, ' ') + "   Y: " + (YTileCor + scrollOffset).ToString().PadLeft(4, ' ');
                if (world != 0) mark.Visibility = Visibility.Hidden;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("file not found");
            }
        }
        private void ScrV_MouseDown(object sender, MouseButtonEventArgs e)
        {
            scaleImg = ScrV.ActualWidth / 512;
            XTileCor = (int)(Mouse.GetPosition(ScrV).X / scaleImg);
            YTileCor = (int)(Mouse.GetPosition(ScrV).Y / scaleImg);

            scrollOffset = (int)(ScrV.VerticalOffset / scaleImg);
            if (YTileCor > 0) markYpos = (int)(YTileCor * scaleImg) + 3 + (int)ScrV.VerticalOffset;
            Canvas.SetTop(mark, markYpos - (int)ScrV.VerticalOffset);
            markXpos = (int)(XTileCor * scaleImg) - 7;
            Canvas.SetLeft(mark, markXpos);
            TextBox.Text = "X: " + XTileCor.ToString().PadLeft(4, ' ') + "   Y: " + (YTileCor + scrollOffset).ToString().PadLeft(4, ' ');
            world = selectWorld;
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }
        private void ScrV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Canvas.SetTop(mark, markYpos - (int)ScrV.VerticalOffset);
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }
        private void W0_Click(object sender, RoutedEventArgs e)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(@"map\world_0.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            WorldImg.Source = bi;
            selectWorld = 0;
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }

        private void W1_Click(object sender, RoutedEventArgs e)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(@"map\world_1.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            WorldImg.Source = bi;
            selectWorld = 1;
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }

        private void W2_Click(object sender, RoutedEventArgs e)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(@"map\world_2.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            WorldImg.Source = bi;
            selectWorld = 2;
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }

        private void W3_Click(object sender, RoutedEventArgs e)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(@"map\world_3.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            WorldImg.Source = bi;
            selectWorld = 3;
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //XcorInMap = x1 + x2 * 256;
            fileBytes[indX] = (byte)(XTileCor * 64 - (XTileCor * 64 / 256) * 256);
            fileBytes[indX + 1] = (byte)(XTileCor * 64 / 256);
            //System.Diagnostics.Debug.WriteLine(YTileCor * 64 / 256);
            fileBytes[indY] = (byte)((YTileCor + scrollOffset) * 64 - ((YTileCor + scrollOffset) * 64 / 256) * 256);
            fileBytes[indY + 1] = (byte)((YTileCor + scrollOffset) * 64 / 256);
            fileBytes[indM] = (byte)world;
            System.Diagnostics.Debug.WriteLine(world);
            File.WriteAllBytes("data.000", fileBytes);
            System.Diagnostics.Debug.WriteLine("OK");

            // 1234
            // 04d2 - d204
            // 
        }

        private void W4_Click(object sender, RoutedEventArgs e)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(@"map\world_4.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            WorldImg.Source = bi;
            selectWorld = 4;
            if (selectWorld != world) mark.Visibility = Visibility.Hidden;
            else mark.Visibility = Visibility.Visible;
        }
    }
}