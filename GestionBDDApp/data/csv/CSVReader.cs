using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.csv
{
    class CSVReader
    {
        public static void ReadFile(String Path, Action<string[]> WhatToDo)
        {
            try
            {
                using (StreamReader Reader = new StreamReader(Path))
                {
                    if (! Reader.EndOfStream)
                    {
                        //Definition des colones
                        Reader.ReadLine();
                    }
                    while (! Reader.EndOfStream)
                    {
                        string Line = Reader.ReadLine();
                        WhatToDo.Invoke(Line.Split(';'));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
