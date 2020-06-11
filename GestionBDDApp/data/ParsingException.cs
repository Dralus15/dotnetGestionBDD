using System;

namespace GestionBDDApp.data
{
    /// <summary>
    /// Une exception représentant une erreur dans l'analyse du fichier d'import de base
    /// </summary>
    [Serializable]
    internal class ParsingException : Exception
    {

        /// <summary>
        /// Crée une nouvelle erreur d'analyse avec un message d'erreur
        /// </summary>
        /// <param name="Error">Le message d'erreur</param>
        public ParsingException(string Error) : base(Error)
        {

        }

    }
}
