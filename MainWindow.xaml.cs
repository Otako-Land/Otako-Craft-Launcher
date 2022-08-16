using System;
using System.Windows;
using Squirrel;
using System.Windows.Input;

namespace OCM_Installer_V2
{
    public partial class MainWindow
    {
        UpdateManager manager = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher");
        Inicio Inicio = new();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                UpdateHandler.HandleInstallEvents();
                UpdateProgressWindow.CheckAndInstall();

                CurrentVersion.Content = "v" + manager.CurrentlyInstalledVersion().ToString();
                MainFrame.Navigate(Inicio);
                RootNavigation.Visibility = Visibility.Hidden;
                NavBackground.Visibility = Visibility.Hidden;
            }
            catch (Exception err)
            {
                new Util.Reporter().ReportError(err.ToString());
                return;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Permitir arrastrar la ventana desde cualquier lugar
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void NavigationItem_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content != Inicio)
                MainFrame.Navigate(Inicio);
        }

    }
}
