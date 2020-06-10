using System;

namespace GestionBDDApp.data
{
    [Serializable]
    class ParsingException : Exception
    {

        public ParsingException(string Error)
            : base(Error)
        {

        }

    }
}
