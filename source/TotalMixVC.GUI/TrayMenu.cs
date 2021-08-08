﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WindowsShortcutFactory;

namespace TotalMixVC.GUI
{
    /// <summary>
    /// Implements the system tray menu functionality.
    /// </summary>
    public class TrayMenu
    {
        /// <summary>
        /// Indicates whether the application will start automatically when Windows starts.
        /// </summary>
        public bool RunOnStartup
        {
            get
            {
                string shortcutPath = Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                    "TotalMix Volume Control.lnk");

                return File.Exists(shortcutPath);
            }

            set
            {
                string shortcutPath = Path.Join(
                    Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                    "TotalMix Volume Control.lnk");
                string appExecutablePath = Process.GetCurrentProcess().MainModule.FileName;

                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
                else
                {
                    using WindowsShortcut shortcut = new()
                    {
                        Description = "TotalMix Volume Control",
                        WorkingDirectory = Directory.GetParent(appExecutablePath).FullName,
                        Path = appExecutablePath,
                        IconLocation = new IconLocation(appExecutablePath, index: 0)
                    };

                    shortcut.Save(shortcutPath);
                }
            }
        }

        /// <summary>
        /// Gets the Exit tray icon command which shuts down the application.
        /// </summary>
        public ICommand ExitAppCommand => new DelegateCommand
        {
            CommandAction = () => Application.Current.Shutdown()
        };
    }
}
