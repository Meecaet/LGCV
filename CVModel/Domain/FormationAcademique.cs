using CVModel.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    [Serializable]
    public class FormationAcademique
    {
        public string Titre { get; set; }
        public string Instituition { get; set; }

        internal static List<FormationAcademique> AssamblerFormationEtCertifications(CVSection sectionFormation)
        {
            List<FormationAcademique> formations = new List<FormationAcademique>();
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(1).Skip(1).ToList();
            formationParagraphs.RemoveAll(x => string.IsNullOrEmpty(x.GetParagraphText()));

            for (int i = 0; i < formationParagraphs.Count; i = i + 2)
            {
                FormationAcademique item = new FormationAcademique();
                item.Titre = formationParagraphs[i].GetParagraphText();
                item.Instituition = formationParagraphs[i + 1].GetParagraphText();

                if (string.IsNullOrEmpty(item.Titre) || string.IsNullOrEmpty(item.Instituition))
                    continue;

                formations.Add(item);
            }

            return formations;
        }
    }

}
