using Squirrel;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using static OCM_Installer_V2.Util;

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
            }
            else ShowMessageBox("No establecido", "No has establecido una ubicación personalizada.");
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

        private void ShowShadersFolder_Click(object sender, RoutedEventArgs e)
        {

            // ACCESO DENEGADO

            try
            {
                Process.Start(Globals.CustomLocation + @"\Otako Craft Mods\shaderpacks");
            }
            catch (Exception err)
            {
                new Reporter().ReportError(err.ToString());
                return;
            }
        }

        private void SelectShadersFile_Click(object sender, RoutedEventArgs e)
        {

            // ACCESO DENEGADO

            OpenFileDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string selectedFile = fbd.FileName;
                    File.Move(selectedFile, Globals.CustomLocation + @"\Otako Craft Mods\shaderpacks");
                    ShowMessageBox("Operación terminada", "Se ha movido el archivo seleccionado a la carpeta de shaders.");
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
