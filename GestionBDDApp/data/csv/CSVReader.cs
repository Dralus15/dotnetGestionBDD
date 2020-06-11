using System;
using System.IO;
using System.Text;

namespace GestionBDDApp.data.csv
{
    /// <summary>
    /// Permet de lire un fichier CSV simplement.
    /// </summary>
    class CsvReader
    {
        /// <summary>
        /// Lit le fichier CSV dont le chemin est passé en paramètre (<paramref name="Path"/>),
        /// chaque ligne est séparée par des points virgules puis est envoyée dans la fonction de retour <paramref name="WhatToDo"/>.
        /// </summary>
        /// <param name="Path">Le chemin vers le fichier à lire.</param>
        /// <param name="WhatToDo">Fonction définissant le traitement à faire sur chaque ligne lue.</param>
        public static void ReadFile(string Path, Action<string[], int> WhatToDo)
        {
            try
            {
                using (var Reader = new StreamReader(Path, Encoding.Default))
                {
                    if (! Reader.EndOfStream)
                    {
                        //Definition des colonnes.
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
