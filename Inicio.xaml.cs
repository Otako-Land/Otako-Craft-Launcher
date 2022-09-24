#pragma warning disable CS0649
using System;
using System.Windows;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.WinForm;
using Downloader;
using System.IO;
using System.Net.Http;
using static OCM_Installer_V2.Util;

namespace OCM_Installer_V2
{
    public partial class Inicio
    {
        readonly HttpClient httpClient = new();
        readonly bool packUpdate = false;
        readonly string installPath = Globals.CustomLocation;
        readonly string packVersionFile = Globals.CustomLocation + @"\Otako Craft Mods\PackVersion.txt";
        readonly string usernameFile = Globals.AppDirectory + @"\Username.txt";

        public Inicio()
        {
            InitializeComponent();
            try
            {
                string latestPackVersion = httpClient.GetStringAsync("https://otcr.tk/packversion.txt").Result;

                if (!File.Exists(packVersionFile))
                {
                    Directory.CreateDirectory(Globals.CustomLocation + @"\Otako Craft Mods");
                    var cf = File.Create(packVersionFile);
                    cf.Close();
                }
                string localPackVersion = File.ReadAllText(packVersionFile);

                if (File.Exists(Globals.AppDirectory + @"\CustomLocation.txt") && string.IsNullOrEmpty(localPackVersion)) File.WriteAllText(packVersionFile, latestPackVersion);
                if (!string.IsNullOrEmpty(localPackVersion) && !localPackVersion.Equals(latestPackVersion)) packUpdate = true;
                if (Directory.Exists(installPath + @"\Otako Craft Mods\mods") && !packUpdate && Directory.GetFiles(installPath + @"\Otako Craft Mods\mods").Length > 140) PlayButton.IsEnabled = true; else PlayButton.Content = "Instala todo primero";
                if (packUpdate) { PlayButton.Content = "Actualización disponible"; InstallAll.Content = "Actualizar y jugar"; }
            }
            catch (Exception err)
            {
                new Reporter().ReportError(err.ToString());
                return;
            }
        }

        private void Downloader_DownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
        {
            string prog = e.ProgressPercentage.ToString("0.0");
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ProgresoInstalaciones.Value = e.ProgressPercentage;
                InstallAll.Content = prog + "%";
            }));
        }

        private void Downloader_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { ProgresoInstalaciones.Value = 0; InstallAll.Content = "Cargando..."; }));
        }
        private void Launcher_FileChanged(CmlLib.Core.Downloader.DownloadFileChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { DownloadedFiles.Content = e.ProgressedFileCount + "/" + e.TotalFileCount; }));
        }
        private void Launcher_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                DownloadedFilePercentage.Content = e.ProgressPercentage + "%";
                ProgresoInstalacionesJugar.Value = e.ProgressPercentage;
            }));
        }

        private void InstallAllButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareToPlay(true);
        }


        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareToPlay(false);
        }

        public void PrepareToPlay(bool doInstallation)
        {
            InstallAll.IsEnabled = false;
            PlayButton.IsEnabled = false;
            PlayButton.Content = doInstallation ? "Instalación en curso" : "Cargando juego...";
            var computerMemory = GetMemoryMb();
            if (computerMemory == null) { computerMemory = 4096; return; }
            var max = computerMemory / 2;
            if (max <= 4096) max = 4096; else if (max > 8192) max = 8192;
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;
            var downloader = new DownloadService();
            var path = new MinecraftPath(installPath + @"\Otako Craft Mods");
            var launcher = new CMLauncher(installPath + @"\Otako Craft Mods");
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
            launcher.FileChanged += Launcher_FileChanged;
            launcher.ProgressChanged += Launcher_ProgressChanged;
            MicrosoftLoginForm loginWindow = new()
            {
                LoadingText = "Cargando...\nEspera un momento plis :D",
                Text = "Iniciar sesión | Recuerda que tienes que tener Minecraft comprado para que esto sirva"
            };

            Application.Current.Dispatcher.Invoke(new Action(async () =>
            {
                try
                {
                    if (Directory.Exists(installPath + @"\Otako Craft Mods\mods") && doInstallation)
                    {
                        Directory.Delete(installPath + @"\Otako Craft Mods\mods", true);
                        PlayButton.IsEnabled = false;
                    }

                    string customUsername = File.ReadAllText(usernameFile);
                    bool isUserPremium = false;

                    if (string.IsNullOrEmpty(customUsername) || IsLauncherPremiumOnly()) isUserPremium = true;

                    MSession account = isUserPremium ? await loginWindow.ShowLoginDialog() : MSession.GetOfflineSession(customUsername);
                    var premiumAccount = new MSession(account.Username, account.AccessToken, account.UUID);
                    loginWindow.Close();

                    if (doInstallation)
                    {
                        await downloader.DownloadFileTaskAsync("https://otcr.tk/pack.zip", installPath + @"\Otako Craft Mods\pack.zip");
                        System.IO.Compression.ZipFile.ExtractToDirectory(installPath + @"\Otako Craft Mods\pack.zip", installPath + @"\Otako Craft Mods", true);
                        DownloadedFiles.Visibility = Visibility.Visible;
                        DownloadedFilePercentage.Visibility = Visibility.Visible;
                        InstallAll.FontSize = 21.7;
                        InstallAll.Content = "Descargando y comprobando archivos";
                        File.Delete(installPath + @"\Otako Craft Mods\pack.zip");
                    }

                    WriteCustomModConfigs();

                    if (packUpdate)
                    {
                        string latestPackVersion = httpClient.GetStringAsync("https://otcr.tk/packversion.txt").Result;
                        File.WriteAllText(packVersionFile, latestPackVersion);
                    }

                    var process = await launcher.CreateProcessAsync("Otako Craft Mods", new MLaunchOption()
                    {
                        GameLauncherName = "Otako Craft Launcher",
                        Session = isUserPremium ? premiumAccount : MSession.GetOfflineSession(customUsername),
                        Path = path,
                        MaximumRamMb = (int)max,
                        VersionType = "Otako Craft",
                        StartVersion = new CmlLib.Core.Version.MVersion("Otako Craft Mods")
                    });
                    process.Start();
                    Environment.Exit(0);
                }
                catch (Exception err)
                {
                    if (!err.Message.Contains("The user has denied access") && !err.Message.Contains("User cancelled login") && !err.Message.Contains("mojang_noprofile"))
                    {
                        new Reporter().ReportError(err.ToString());
                    }
                    if (err.Message.Contains("mojang_noprofile"))
                    {
                        ShowMessageBox("No tienes Minecraft comprado", "Has iniciado sesión con una cuenta que no tiene Minecraft comprado.\nVe al apartado Cuenta y ponte un nombre de usuario para poder jugar.\nSi el servidor está en modo de solo cuentas compradas, no tendrás opción de jugar sin Minecraft comprado.");

                    }
                    return;
                }
            }));
        }
    }
}
