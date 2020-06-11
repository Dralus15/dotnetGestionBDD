using System;

namespace GestionBDDApp.data
{
    [Serializable]
    internal class ParsingException : Exception
    {

        public ParsingException(string Error) : base(Error)
        {

        }

    }
}
