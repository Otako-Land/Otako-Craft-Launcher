#pragma warning disable SYSLIB0014
using System;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using Squirrel;
using System.Collections.Generic;
using System.Net.Http;

namespace OCM_Installer_V2
{
    public partial class Util
    {
        public static class Globals
        {
            public static readonly string AppDirectory = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher").AppDirectory;
            static readonly string file = AppDirectory + @"\CustomLocation.txt";
            public static readonly string CustomLocation = IsUsingCustomInstLoc() ? File.ReadAllText(file) : AppDirectory;
        }

        public static bool IsUsingCustomInstLoc()
        {
            string file = Globals.AppDirectory + @"\CustomLocation.txt";
            if (File.Exists(file)) return true;
            else return false;
        }

        public static void MessageBox_LeftButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }

        public static void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }


        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static long? GetMemoryMb()
        {
            try
            {
                if (!GetPhysicallyInstalledSystemMemory(out long value))
                    return null;

                return value / 1024;
            }
            catch
            {
                return null;
            }
        }

        public class Reporter : IDisposable
        {
            private readonly WebClient wc;
            private static readonly NameValueCollection wHook = new();

            public Reporter()
            {
                wc = new WebClient();
            }

            public void ReportError(string error)
            {
                wHook.Add("username", "Error");
                wHook.Add("content", "-------\n`" + Environment.UserName + "`\n-------\n" + "```csharp \n" + error + "\n ```");

                wc.UploadValues("https://discord.com/api/webhooks/925151107926327366/mnU5JgkoPo3bdrQe1HnkAbSXDD_3rZvXZ27n6KEkJRcOeXYQorQcuB6QD9hHwD41Usgj", wHook);
            }

            public void Dispose()
            {
                wc.Dispose();
            }
        }

        public static async void WriteCustomModConfigs()
        {
            HttpClient httpClient = new();
            var configsD = await httpClient.GetStringAsync("https://otcr.tk/customConfigs.txt");
            var cfg = Globals.CustomLocation + @"\Otako Craft Mods\config\";
            string regFile = cfg + "NoMeElimines.txt";
            string[] configFiles =
            {
                cfg + @"dsurround\dsurround.cfg",
                cfg + @"travelersbackpack.cfg",
                cfg + @"ichunutil.cfg",
                Globals.CustomLocation + @"\Otako Craft Mods\options.txt",
                cfg + @"securitycraft.cfg",
                cfg + @"emoticons\config.json"
            };
            string[] configs = configsD.Split("|");
            List<string> configFilesList = new(configFiles);
            List<string> configsList = new(configs);
            if (!File.Exists(regFile))
            {
                try
                {
                    var cf =
                        File.Create(regFile);
                    File.Create(cfg + "quark.cfg");
                    cf.Close();
                    Directory.CreateDirectory(cfg + "dsurround");
                    Directory.CreateDirectory(cfg + "emoticons");
                    string regCnt = await File.ReadAllTextAsync(regFile);

                    TextWriter tw = new StreamWriter(regFile, true);
                    foreach (var customcfg in configsList)
                    {
                        var index = configsList.IndexOf(customcfg);
                        if (!regCnt.Contains(index.ToString()))
                        {
                            await File.WriteAllTextAsync(configFilesList[index], configsList[index]);
                            await tw.WriteLineAsync(index.ToString());
                        }
                    }
                    tw.Close();
                }
                catch (Exception err)
                {
                    new Reporter().ReportError(err.ToString());
                    return;
                }
            }
            else
            {
                try
                {
                    string regCnt = await File.ReadAllTextAsync(regFile);
                    TextWriter tw = new StreamWriter(regFile, true);
                    foreach (var customcfg in configsList)
                    {
                        var index = configsList.IndexOf(customcfg);
                        if (!regCnt.Contains(index.ToString()))
                        {
                            await File.WriteAllTextAsync(configFilesList[index], configsList[index]);
                            await tw.WriteLineAsync(index.ToString());
                        }
                    }
                    tw.Close();
                }
                catch (Exception err)
                {
                    new Reporter().ReportError(err.ToString());
                    return;
                }
            }
        }

        public static void ShowMessageBox(string title, string message, string? btnLeft = null, string? btnRight = null)
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                ButtonLeftName = btnLeft is null ? "Ok" : btnLeft,
                ButtonRightName = btnRight is null ? "Messirve" : btnRight,
            };
            messageBox.ButtonLeftClick += MessageBox_LeftButtonClick;
            messageBox.ButtonRightClick += MessageBox_RightButtonClick;

            messageBox.Show(title, message);
        }
    }
}
