// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TransferPaper
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Microsoft.Win32;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants

        private const double OpacityMaximumThreshold = 1.0;

        private const double OpacityMinimumThreshold = 0.1;

        #endregion

        #region Fields

        private readonly OpenFileDialog BrowseDecalDialog;

        private Key OpacityDirection;

        private Key WindowDirection;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();

            this.BrowseDecalDialog = new OpenFileDialog();
            this.BrowseDecalDialog.Filter =
                "Image Files (*.jpg; *.jpeg; *.gif; *.png; *.bmp)|*.jpg; *.jpeg; *.gif; *.png; *.bmp";
        }

        #endregion

        #region Methods

        private void ButtonCloseCross_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonCloseCross_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void ButtonCloseCross_OnTouchUp(object sender, TouchEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void ButtonLoadDecal_Click(object sender, RoutedEventArgs e)
        {
            if (true == this.BrowseDecalDialog.ShowDialog())
            {
                this.LoadDecal(this.BrowseDecalDialog.FileName);
            }
        }

        private void CommandBinding_CanExecute_Close(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void LoadDecal(string path)
        {
            try
            {
                var decalBitmap = new BitmapImage(new Uri(path));
                this.Width = Math.Min(decalBitmap.Width, SystemParameters.PrimaryScreenWidth);
                this.Height = Math.Min(decalBitmap.Height, SystemParameters.PrimaryScreenHeight);
                this.BorderBackground.Background = new ImageBrush(decalBitmap);
                this.Topmost = true;

                this.ButtonLoadDecal.Visibility = Visibility.Hidden;
                this.TextBlockHelp.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hey, Dummy! That was not an image!\r\n\r\nDetails:\r\n" + ex.Message, 
                    "Invalid input", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);

            if (filenames != null)
            {
                this.LoadDecal(filenames[0]);
                this.Cursor = Cursors.Arrow;
            }

            e.Handled = true;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
            {
                return;
            }

            if (e.Key == Key.Add || e.Key == Key.Subtract)
            {
                this.OpacityDirection = e.Key;
                this.UpdateOpacity();
            }

            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                this.WindowDirection = e.Key;
                this.UpdateWindowPosition();
            }

            if (e.Key == Key.Escape || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Return
                || e.Key == Key.Space)
            {
                this.UnloadDecal();
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
            {
                return;
            }

            if (e.Key != Key.Add && e.Key != Key.Subtract)
            {
                return;
            }
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var window = sender as Window;
            if (window != null && e.LeftButton == MouseButtonState.Pressed)
            {
                window.DragMove();
            }
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
            {
                return;
            }

            if (e.Delta > 0 && this.BorderBackground.Opacity < OpacityMaximumThreshold)
            {
                this.OpacityDirection = Key.Add;
            }
            else if (e.Delta < 0 && this.BorderBackground.Opacity > OpacityMinimumThreshold)
            {
                this.OpacityDirection = Key.Subtract;
            }
            else
            {
                return;
            }

            this.UpdateOpacity();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)        
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.Cursor = Cursors.SizeAll;
        }

        private void MainWindow_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void UnloadDecal()
        {
            this.Width = 640;
            this.Height = 480;
            this.BorderBackground.Background = new SolidColorBrush(Colors.White);
            this.Topmost = false;

            this.BorderBackground.Opacity = 0.9;

            this.ButtonLoadDecal.Visibility = Visibility.Visible;
            this.TextBlockHelp.Visibility = Visibility.Visible;
        }

        private void UpdateOpacity()
        {
            if (this.OpacityDirection == Key.Add && this.BorderBackground.Opacity < OpacityMaximumThreshold)
            {
                this.BorderBackground.Opacity = Math.Round(this.BorderBackground.Opacity * 100 + 5) / 100;
            }

            if (this.OpacityDirection == Key.Subtract && this.BorderBackground.Opacity > OpacityMinimumThreshold)
            {
                this.BorderBackground.Opacity = Math.Round(this.BorderBackground.Opacity * 100 - 5) / 100;
            }
        }

        private void UpdateWindowPosition()
        {
            if (this.WindowDirection == Key.Up)
            {
                this.Top -= 1;
            }

            if (this.WindowDirection == Key.Down)
            {
                this.Top += 1;
            }

            if (this.WindowDirection == Key.Left)
            {
                this.Left -= 1;
            }

            if (this.WindowDirection == Key.Right)
            {
                this.Left += 1;
            }
        }

        #endregion
    }
}