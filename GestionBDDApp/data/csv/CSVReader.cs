using System;
using System.IO;
using System.Text;

namespace GestionBDDApp.data.csv
{
    class CSVReader
    {
        public static void ReadFile(String Path, Action<string[], int> WhatToDo)
        {
            try
            {
                using (StreamReader Reader = new StreamReader(Path, Encoding.Default))
                {
                    if (! Reader.EndOfStream)
                    {
                        //Definition des colones
                        Reader.ReadLine();
                    }

                    int LineNumber = 0;
                    while (! Reader.EndOfStream)
                    {
                        string Line = Reader.ReadLine();
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
