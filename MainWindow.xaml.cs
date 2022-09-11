using System;
using System.Windows;
using Squirrel;
using System.Windows.Input;
using static OCM_Installer_V2.Util;

namespace OCM_Installer_V2
{
    public partial class MainWindow
    {
        readonly UpdateManager manager = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher");
        readonly Inicio Inicio = new();
        readonly Cuenta Cuenta = new();
        readonly Ajustes Ajustes = new();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                UpdateHandler.HandleInstallEvents();
                UpdateProgressWindow.CheckAndInstall();

                CurrentVersion.Content = "v" + manager.CurrentlyInstalledVersion().ToString();
                MainFrame.Navigate(Inicio);

                if (!IsLauncherPremiumOnly()) Cuenta.CustomUsername.Visibility = Visibility.Visible;
            }
            catch (Exception err)
            {
                new Reporter().ReportError(err.ToString());
                return;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Permitir arrastrar la ventana desde cualquier lugar
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void NavigationItem_Inicio_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content != Inicio) MainFrame.Navigate(Inicio);
        }

        private void NavigationItem_Cuenta_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content != Cuenta) MainFrame.Navigate(Cuenta);
        }

        private void NavigationItem_Ajustes_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content != Ajustes) MainFrame.Navigate(Ajustes);
        }
    }
}
