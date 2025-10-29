
using FontAwesome.Sharp;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace school_management.view
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Allow dragging the window by the title bar
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double-click to maximize/restore
                btnMaximize_Click(sender, null);
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                // Single click to drag
                try
                {
                    DragMove();
                }
                catch (Exception)
                {
                    // Ignore exception when window is maximized
                }
            }
        }

        // Close button - closes the application
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
                Application.Current.Shutdown();
        }

        // Maximize/Restore button - toggles window state
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;

                // Change icon to maximize icon
                var icon = btnMaximize.Content as IconImage;
                if (icon != null)
                {
                    icon.Icon = IconChar.Square;
                }
            }
            else
            {
                WindowState = WindowState.Maximized;

                // Change icon to restore icon
                var icon = btnMaximize.Content as IconImage;
                if (icon != null)
                {
                    icon.Icon = IconChar.WindowRestore;
                }
            }
        }

        // Minimize button - minimizes the window
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Handle window state changed event to update maximize button icon
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            var icon = btnMaximize.Content as IconImage;
            if (icon != null)
            {
                if (WindowState == WindowState.Maximized)
                {
                    icon.Icon = IconChar.WindowRestore;
                }
                else
                {
                    icon.Icon = IconChar.Square;
                }
            }
        }
    }
}
