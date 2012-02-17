using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace FreeFoodButton
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            KeyboardManager.DisableSystemKeys();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            KeyboardManager.EnableSystemKeys();
            base.OnExit(e);
        }
    }
}
