﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
