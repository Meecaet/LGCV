namespace CVModel.Domain
{
    public class Lange
    {
        public string Nom { get; set; }
        public Niveau Parle { get; set; }
        public Niveau Ecrit { get; set; }
        public Niveau Lu { get; set; }
    }

    public enum Niveau
    {
        Basique = 0,
        Intermediaire = 1,
        Avance = 2
    }
}