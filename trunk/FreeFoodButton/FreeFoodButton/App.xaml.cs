using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace FreeFoodButton
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private KeyboardManager keyboard;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            keyboard = new KeyboardManager();
            keyboard.DisableSystemKeys();
            keyboard.KeyPressed += KeyPressed;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            keyboard.EnableSystemKeys();
        }

        void KeyPressed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(ShowMainWindow));
        }

        private void ShowMainWindow()
        {
            KeyboardManager.ActivateWindow(Application.Current.MainWindow);
        }
    }
}
