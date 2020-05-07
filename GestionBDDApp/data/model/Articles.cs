using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionBDDApp.data.model
{
    class Articles
    {
        private string RefArticle;

        private string Description;

        private SousFamilles SousFamille;

        private Marques Marque;

        private float Prix;

        private int Quantite;

        public static Articles FromRawData(string RefStr, string DescStr, string SousFamilleStr, string MarqueStr,
            string PrixStr, string QuantityStr)
        {
            int Quantity;
            try
            {
                Quantity = int.Parse(QuantityStr);
            }
            catch (FormatException e)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne quantité");
            }

            return new Articles();
        }

    }
}
