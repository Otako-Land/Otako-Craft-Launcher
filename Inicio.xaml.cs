#pragma warning disable CS8618
#pragma warning disable CS0649
using System;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
using Downloader;
using Squirrel;
using System.Threading;
using System.IO;

namespace OCM_Installer_V2
{
    public partial class Inicio
    {
        UpdateManager man;

        private void MessageBox_LeftButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }

        private void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }

        public Inicio()
        {
            InitializeComponent();
            man = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher");
            var launcherLocation = man.AppDirectory;
            if (Directory.Exists(launcherLocation + @"\Otako Craft Mods\mods"))
            {
                PlayButton.IsEnabled = true;
            }
        }

        private void Downloader_DownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            string prog = e.ProgressPercentage.ToString("0.0");
            Application.Current.Dispatcher.Invoke(new Action(() => { ProgresoInstalaciones.Value = e.ProgressPercentage; ProgresoLabel.Visibility = Visibility.Visible; ProgresoLabel.Content = prog + "%"; }));
        }

        private void Downloader_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { ProgresoInstalaciones.Value = 0; ProgresoLabel.Visibility = Visibility.Hidden; }));
        }
        private void Launcher_FileChanged(CmlLib.Core.Downloader.DownloadFileChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                DownloadedFiles.Content = e.ProgressedFileCount + "/" + e.TotalFileCount;
            }));
        }
        private void Launcher_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                DownloadedFilePercentage.Content = e.ProgressPercentage + "%";
                ProgresoInstalacionesJugar.Value = e.ProgressPercentage;
            }));
        }

        private async void InstallAllButton_Click(object sender, RoutedEventArgs e)
        {
            var computerMemory = Util.GetMemoryMb();
            if (computerMemory == null)
            {
                computerMemory = 4096;
                return;
            }
            var max = computerMemory / 2;
            if (max <= 4096) max = 4096; else if (max > 8192) max = 8192;
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ButtonLeftName = "Ok",
                ButtonRightName = "Messirve"
            };
            messageBox.ButtonLeftClick += Util.MessageBox_LeftButtonClick;
            messageBox.ButtonRightClick += Util.MessageBox_RightButtonClick;
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;
            var launcherLocation = man.AppDirectory;
            var downloader = new DownloadService();
            var path = new MinecraftPath(launcherLocation + @"\Otako Craft Mods");
            var launcher = new CMLauncher(launcherLocation + @"\Otako Craft Mods");
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
            launcher.FileChanged += Launcher_FileChanged;
            launcher.ProgressChanged += Launcher_ProgressChanged;
            MicrosoftLoginWindow loginWindow = new()
            {
                LoadingText = "Cargando...\nEspera un momento plis :D",
                Title = "Iniciar sesión con tu cuenta de Microsoft | ¿Para qué? Para abrir el juego usando tu cuenta :)"
            };
            MSession session = await loginWindow.ShowLoginDialog();
            loginWindow.Close();

            Application.Current.Dispatcher.Invoke(new Action(async () =>
            {
                try
                {
                    Instalar_todo.FontSize = 17;
                    Instalar_todo.Content = "Descargando mods y otros archivos";
                    Instalar_todo.IsEnabled = false;
                    await downloader.DownloadFileTaskAsync("https://otcr.tk/pack.zip", launcherLocation + @"\Otako Craft Mods\pack.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory(launcherLocation + @"\Otako Craft Mods\pack.zip", launcherLocation + @"\Otako Craft Mods", true);
                    DownloadedFiles.Visibility = Visibility.Visible;
                    DownloadedFilePercentage.Visibility = Visibility.Visible;
                    Instalar_todo.FontSize = 13.5;
                    Instalar_todo.Content = "Descargando y comprobando archivos del juego";
                    var ses = new MSession(session.Username, session.AccessToken, session.UUID);
                    var process = await launcher.CreateProcessAsync("Otako Craft Mods", new MLaunchOption()
                    {
                        GameLauncherName = "Otako Craft Launcher",
                        Session = ses,
                        Path = path,
                        MaximumRamMb = (int)max,
                        VersionType = "Otako Craft",
                        StartVersion = new CmlLib.Core.Version.MVersion("Otako Craft Mods")
                    });
                    process.Start();
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
                catch (Exception err)
                {
                    messageBox.Show("Reporta este error a The Ghost por favor", err.ToString());
                    return;
                }
            }));
        }


        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var computerMemory = Util.GetMemoryMb();
            if (computerMemory == null)
            {
                computerMemory = 4096;
                return;
            }
            var max = computerMemory / 2;
            if (max <= 4096) max = 4096; else if (max > 8192) max = 8192;
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ButtonLeftName = "Ok",
                ButtonRightName = "Messirve"
            };
            messageBox.ButtonLeftClick += Util.MessageBox_LeftButtonClick;
            messageBox.ButtonRightClick += Util.MessageBox_RightButtonClick;
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;
            var launcherLocation = man.AppDirectory;
            var path = new MinecraftPath(launcherLocation + @"\Otako Craft Mods");
            var launcher = new CMLauncher(launcherLocation + @"\Otako Craft Mods");
            MicrosoftLoginWindow loginWindow = new()
            {
                LoadingText = "Cargando...\nEspera un momento plis :D",
                Title = "Iniciar sesión con tu cuenta de Microsoft | ¿Para qué? Para abrir el juego usando tu cuenta :)"
            };
            MSession session = await loginWindow.ShowLoginDialog();
            loginWindow.Close();

            Application.Current.Dispatcher.Invoke(new Action(async () =>
            {
                try
                {
                    var ses = new MSession(session.Username, session.AccessToken, session.UUID);
                    var process = await launcher.CreateProcessAsync("Otako Craft Mods", new MLaunchOption()
                    {
                        GameLauncherName = "Otako Craft Launcher",
                        Session = ses,
                        Path = path,
                        MaximumRamMb = 4096,
                        VersionType = "Otako Craft",
                        StartVersion = new CmlLib.Core.Version.MVersion("Otako Craft Mods")
                    });
                    PlayButton.IsEnabled = false;
                    PlayButton.Content = "Comprobando archivos";
                    process.Start();
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
                catch (Exception err)
                {
                    messageBox.Show("Reporta este error a The Ghost por favor", err.ToString());
                    return;
                }
            }));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ButtonLeftName = "Ok",
                ButtonRightName = "Messirve"
            };
            messageBox.ButtonLeftClick += Util.MessageBox_LeftButtonClick;
            messageBox.ButtonRightClick += Util.MessageBox_RightButtonClick;
            try
            {
                MicrosoftLoginWindow loginWindow = new()
                {
                    LoadingText = "Cargando...\nEspera un momento plis :D",
                    Title = "Iniciar sesión con tu cuenta de Microsoft | ¿Para qué? Para abrir el juego usando tu cuenta :)"
                };
                MSession session = await loginWindow.ShowLoginDialog();
                loginWindow.Close();
                messageBox.Show("Bromita", "Hola " + session.Username + "\nGracias por regalarme tu cuenta de Minecraft :D" + "\nxd");
                loginWindow.Close();
            }
            catch (Exception err)
            {
                messageBox.Show("Reporta este error a The Ghost por favor", err.ToString());
                return;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ButtonLeftName = "Ok",
                ButtonRightName = "Messirve"
            };
            messageBox.ButtonLeftClick += Util.MessageBox_LeftButtonClick;
            messageBox.ButtonRightClick += Util.MessageBox_RightButtonClick;
            try
            {
                MicrosoftLoginWindow logoutWindow = new()
                {
                    LoadingText = "Cerrando sesión...\nEspera un momento plis :D",
                    Title = "Cerrar sesión de tu cuenta de Microsoft"
                };
                logoutWindow.ShowLogoutDialog();
                messageBox.Show("Sesión cerrada", "Has cerrado sesión.");
                logoutWindow.Close();
            }
            catch (Exception err)
            {
                messageBox.Show("Reporta este error a The Ghost por favor", err.ToString());
                return;
            }
        }
    }
}
