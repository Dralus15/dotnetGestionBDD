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

        private Familles Marque;

        private float Prix;

        private int Quantite;

        public static Articles FromRawData(string RefStr, string DescStr, string SousFamilleStr, string MarqueStr,
            string PrixStr, string QuantityStr)
        {
            int Quantity;
            float Prix;
            try
            {
                Quantity = int.Parse(QuantityStr);
                Prix = float.Parse(PrixStr);
            }
            catch (FormatException e)
            {
                throw new ParsingException("Valeur incorrecte pour la colonne quantité ou prix");
            }

            return new Articles();
        }

    }
}
