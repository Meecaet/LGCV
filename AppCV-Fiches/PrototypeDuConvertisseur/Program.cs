using DAL_CV_Fiches;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using XmlHandler.Generation;

namespace PrototypeDuConvertisseur
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var endpoint = config["GraphConnectionEndPoint"];
            var primaryKey = config["GraphConnectionPrimaryKey"];
            var database = config["GraphConnectionDatabase"];
            var collection = config["GraphConnectionCollection"];
            GraphConfig.SetGraphDataBaseConnection(endpoint, primaryKey, database, collection);
            CVGenerator generator = new CVGenerator();
            generator.ProcessCV(args[0]);


            Console.WriteLine("Convertion finalisée");
            Console.WriteLine("Appuyer sur un bouton pour sortir");
            Console.ReadKey();
        }
    }
}
