using System;
using System.IO;
using System.Text;

namespace GestionBDDApp.data.csv
{
    class CsvReader
    {
        public static void ReadFile(string Path, Action<string[], int> WhatToDo)
        {
            try
            {
                using (var Reader = new StreamReader(Path, Encoding.Default))
                {
                    if (! Reader.EndOfStream)
                    {
                        //Definition des colones
                        Reader.ReadLine();
                    }

                    var LineNumber = 0;
                    while (! Reader.EndOfStream)
                    {
                        var Line = Reader.ReadLine();
                        if (Line != null)
                        {
                            WhatToDo.Invoke(Line.Split(';'), LineNumber);
                        }

                        LineNumber++;
                    }
                }
            }
            catch (Exception Exception)
            {
                Console.WriteLine(Exception);
                throw;
            }
        }
    }
}
