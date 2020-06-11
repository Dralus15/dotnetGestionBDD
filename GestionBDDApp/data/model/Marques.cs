namespace GestionBDDApp.data.model
{
    public class Marques
    {
        //auto-increment
        public int? Id { get; set; }

        public string Nom { get; set; }

        public Marques(int? Id, string Nom)
        {
            this.Nom = Nom;
            this.Id = Id;
        }
    }
}
