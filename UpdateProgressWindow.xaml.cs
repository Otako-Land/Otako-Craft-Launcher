using Squirrel;
using System;
using System.Windows;

namespace OCM_Installer_V2
{
    public partial class UpdateProgressWindow : Window
    {
        public UpdateProgressWindow()
        {
            InitializeComponent();
        }

        public static async void CheckAndInstall()
        {
            UpdateManager manager = new GithubUpdateManager(@"https://github.com/Otako-Land/Otako-Craft-Launcher");
            try
            {
                var updateInfo = await manager.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    Application.Current.MainWindow.Hide();
                    UpdateProgressWindow upw = new();
                    upw.Show();
                    var update = await manager.UpdateApp();
                    if (update != null) UpdateManager.RestartApp();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Reporta este error a The Ghost por favor");
                return;
            }
        }
    }
}
