using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace GestionBDDApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TODO le nom doit etre Bacchus ou Baccus ?
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

    }
}
