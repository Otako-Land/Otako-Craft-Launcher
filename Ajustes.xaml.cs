using Squirrel;
using System.Threading;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using static OCM_Installer_V2.Util;
using System;

namespace OCM_Installer_V2
{
    public partial class Ajustes
    {
        public string file = Globals.AppDirectory + @"\CustomLocation.txt";

        public Ajustes()
        {
            InitializeComponent();
        }

        private void CustomInstallLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(file)) { var f = File.Create(file); f.Close(); }
                File.WriteAllText(file, fbd.SelectedPath);
                InstLocChangedNoti.Message = "Se ha cambiado la ubicación, pero es necesario\nreiniciar el launcher para aplicar el cambio.";
                InstLocChangedNoti.Show();
            }
        }

        private void ShowCustomInstallLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(file))
            {
                var fContent = File.ReadAllText(file);
                ShowMessageBox("Ubicación actual para las instalaciones y descargas", "La ubicación actual es:\n" + fContent);
            } else ShowMessageBox("No establecido", "No has establecido una ubicación personalizada.");
        }

        private void ResetCustomInstallLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(file)) ShowMessageBox("No establecido", "No has establecido una ubicación personalizada.");
            else { File.Delete(file); ShowMessageBox("Ubicación restablecida", "Se ha restablecido la ubicación."); }
        }

        private void RestartBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateManager.RestartApp();
        }
    }
}
