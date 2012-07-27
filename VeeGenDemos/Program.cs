#region
using System;
using System.Windows.Forms;

#endregion
namespace VeeGenDemos
{
    internal static class Program
    {
        /// <summary>
        ///   Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread] private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}