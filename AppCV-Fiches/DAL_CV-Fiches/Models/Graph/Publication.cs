namespace DAL_CV_Fiches.Models.Graph
{
    public class Publication : GraphObject
    {
        public string Description { get; set; }

        public Publication()
        { }

        public static Publication CreatePublication(string publication)
        {
            Publication plb = new Publication();
            plb.Description = publication;

            return plb;
        }
    }
}