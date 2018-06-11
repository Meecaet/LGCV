using XmlHandler.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class Mandat
    {
        public string Numero { get; set; }
        public string Projet { get; set; }
        public string Envenrgure { get; set; }
        public string Fonction { get; set; }
        public string Periode { get; set; }
        public string Efforts { get; set; }
        public string Reference { get; set; }

        public string Description { get; set; }

        internal void AssemblerMandat(List<XmlDocNode> mandatNodes)
        {
            List<XmlDocParagraph> infoParagraphs = new List<XmlDocParagraph>(), infoParagraphsSecondColumn = new List<XmlDocParagraph>();

            XmlDocTable infoTable = (XmlDocTable)mandatNodes.First(x => x is XmlDocTable);
            infoParagraphs.AddRange(infoTable.GetParagraphsFromColumn(1));
            infoParagraphsSecondColumn.AddRange(infoTable.GetParagraphsFromColumn(2));

            for (int i = 0; i < infoParagraphs.Count; i++)
            {
                if (infoParagraphs[i].GetParagraphText().Contains("Projet"))
                    Projet = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Mandat"))
                    Numero = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Envergure"))
                    Envenrgure = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Fonction"))
                    Fonction = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Période"))
                    Periode = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Efforts"))
                    Efforts = infoParagraphsSecondColumn[i].GetParagraphText();

                if (infoParagraphs[i].GetParagraphText().Contains("Référence"))
                    Reference = infoParagraphsSecondColumn[i].GetParagraphText();
            }

            infoParagraphsSecondColumn.Clear();
            infoParagraphsSecondColumn = null;

            infoParagraphs.Clear();
            infoParagraphs = mandatNodes.SkipWhile(x => x is XmlDocTable).Cast<XmlDocParagraph>().ToList();
            infoParagraphs.ForEach(x => Description += x.GetParagraphText());
        }
    }
}
