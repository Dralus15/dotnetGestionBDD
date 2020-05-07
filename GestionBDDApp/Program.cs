using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
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

        private static void ConnectToDatabase()
        {
            var Connection = CreateConnection();
            CreateTable(Connection);

        }

        private static SQLiteConnection CreateConnection()
        {
            SQLiteConnection Result;
            try
            {
                //TODO Wtf is that
                Result = new SQLiteConnection("Data Source=database.db;version=3;Compress=True");
                Result.Open();
            }
            catch (Exception Exception)
            {
                Console.WriteLine(Exception);
                throw;
            }
            return Result;
        }

        private static SQLiteConnection CreateTable(SQLiteConnection Connection)
        {

            return null;
        }
    }
}
