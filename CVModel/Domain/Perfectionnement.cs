using CVModel.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class Perfectionnement
    {
        public string An { get; set; }
        public string Description { get; set; }

        static internal List<Perfectionnement> AssemblerPerfectionnement(CVSection sectionPerfectionnement)
        {
            List<Perfectionnement> perfectionnements = new List<Perfectionnement>();
            XmlDocTable perfcTable = (XmlDocTable)sectionPerfectionnement.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> firstColumn = new List<XmlDocParagraph>(), secondColumn = new List<XmlDocParagraph>();

            firstColumn.AddRange(perfcTable.GetParagraphsFromColumn(1));
            secondColumn.AddRange(perfcTable.GetParagraphsFromColumn(2));

            for (int i = 0; i < firstColumn.Count; i++)
            {
                Perfectionnement Perfc = new Perfectionnement();
                Perfc.An = firstColumn[i].GetParagraphText();
                Perfc.Description = secondColumn[i].GetParagraphText();

                perfectionnements.Add(Perfc);
            }

            return perfectionnements;
        }
    }
}
