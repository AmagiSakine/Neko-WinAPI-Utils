using System;
using System.Windows;
using LLama.Native;

namespace NEKO_UI
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                string binDir = AppDomain.CurrentDomain.BaseDirectory;

                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}