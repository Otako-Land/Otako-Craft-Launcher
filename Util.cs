#pragma warning disable CS8618
#pragma warning disable SYSLIB0014
using System;
using System.Collections.Specialized;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using Squirrel;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace OCM_Installer_V2
{
    public partial class Util
    {
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
            private readonly WebClient dWebClient;
            private static NameValueCollection discord = new NameValueCollection();

            public Reporter()
            {
                dWebClient = new WebClient();
            }

            public void ReportError(string error)
            {
                discord.Add("username", "Error");
                discord.Add("content", error);

                dWebClient.UploadValues("https://discord.com/api/webhooks/925151107926327366/mnU5JgkoPo3bdrQe1HnkAbSXDD_3rZvXZ27n6KEkJRcOeXYQorQcuB6QD9hHwD41Usgj", discord);
            }

            public void Dispose()
            {
                dWebClient.Dispose();
            }
        }

        public static async void WriteCustomModConfigs()
        {
            UpdateManager man = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher");
            HttpClient httpClient = new();
            var configsD = await httpClient.GetStringAsync("https://otcr.tk/customConfigs.txt");
            var launcherLocation = man.AppDirectory;
            var config = launcherLocation + @"\Otako Craft Mods\config\";
            string regFile = config + "NoMeElimines.txt";
            string[] configFiles =
            {
                config + @"dsurround\dsurround.cfg",
                config + @"travelersbackpack.cfg",
                config + @"ichunutil.cfg",
                launcherLocation + @"\Otako Craft Mods\options.txt",
                config + @"securitycraft.cfg",
                config + @"emoticons\config.json"
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
                        File.Create(config + "quark.cfg");
                    cf.Close();
                    Directory.CreateDirectory(config + "dsurround");
                    Directory.CreateDirectory(config + "emoticons");
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
    }
}
