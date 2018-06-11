using CVModel.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ApplicationDeConversion
{
    public class Program
    {      
        static void Main(string[] args)
        {
            string path = args[0];

            CVGenerator generator = new CVGenerator();
            generator.ProcessCV(path);
        }      
    }
}
