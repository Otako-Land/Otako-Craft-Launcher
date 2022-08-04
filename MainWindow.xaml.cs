using System;
using System.Windows;
using Squirrel;
using System.Windows.Input;
using System.Threading;

namespace OCM_Installer_V2
{
    public partial class MainWindow
    {
        UpdateManager manager;
        Inicio Inicio = new();

        public MainWindow()
        {
            InitializeComponent();
            manager = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher");
            try
            {
                UpdateHandler.HandleInstallEvents();
                UpdateProgressWindow.CheckAndInstall();

                CurrentVersion.Content = "v" + manager.CurrentlyInstalledVersion().ToString();
            }
            catch (Exception err)
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    ButtonLeftName = "Ok",
                    ButtonRightName = "Messirve"
                };
                messageBox.ButtonLeftClick += Util.MessageBox_LeftButtonClick;
                messageBox.ButtonRightClick += Util.MessageBox_RightButtonClick;
                messageBox.ResizeMode = ResizeMode.NoResize;
                messageBox.Show("Reporta este error a The Ghost por favor", err.ToString());
                return;
            }
            MainFrame.Navigate(Inicio);
            RootNavigation.Visibility = Visibility.Hidden;
            NavBackground.Visibility = Visibility.Hidden;
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
