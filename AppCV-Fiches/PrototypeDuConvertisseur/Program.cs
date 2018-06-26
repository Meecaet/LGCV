using System;
using XmlHandler.Generation;

namespace PrototypeDuConvertisseur
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Appuyer sur un bouton pour demarrer");
           // Console.ReadKey();

            CVGenerator generator = new CVGenerator();
            generator.ProcessCV(args[0]);

            Console.WriteLine("Convertion finalisée");
            Console.WriteLine("Appuyer sur un bouton pour sortir");
            Console.ReadKey();
        }
    }
}
