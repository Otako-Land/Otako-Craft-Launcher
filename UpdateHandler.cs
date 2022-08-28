using System;
using System.Windows;
using Squirrel;

namespace OCM_Installer_V2
{
    class UpdateHandler
    {

        public static void HandleInstallEvents()
        {
            SquirrelAwareApp.HandleEvents(onInitialInstall: OnAppInstall, onAppUpdate: OnAppUpdate, onAppUninstall: OnAppUninstall, onEveryRun: OnAppRun);

            static void OnAppInstall(SemanticVersion version, IAppTools tools)
            {
                tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
            }

            static void OnAppUninstall(SemanticVersion version, IAppTools tools)
            {
                tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
            }

            static void OnAppUpdate(SemanticVersion version, IAppTools tools)
            {
                return;
            }

            static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
            {
                tools.SetProcessAppUserModelId();
            }
        }
    }
}
