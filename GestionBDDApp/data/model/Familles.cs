namespace GestionBDDApp.data.model
{
    public class Familles
    {
        //auto-increment
        public int? Id { get; set; }

        public string Nom { get; set; }
        
        public Familles(int? Id, string Nom)
        {
            this.Id = Id;
            this.Nom = Nom;
        }
    }
}
