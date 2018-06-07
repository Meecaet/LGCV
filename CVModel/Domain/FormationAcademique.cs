using CVModel.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVModel.Domain
{
    public class FormationAcademique
    {
        public List<FormationAcademiqueItem> Itens { get; set; }

        public FormationAcademique()
        {
            Itens = new List<FormationAcademiqueItem>();
        }

        internal void AssamblerFormationEtCertifications(CVSection sectionFormation)
        {
            XmlDocTable tableFormation = (XmlDocTable)sectionFormation.Nodes.First(x => x is XmlDocTable);
            List<XmlDocParagraph> formationParagraphs = tableFormation.GetParagraphsFromColumn(1).Skip(1).ToList();

            for (int i = 0; i < formationParagraphs.Count; i = i+2)
            {
                FormationAcademiqueItem item = new FormationAcademiqueItem();
                item.Titre = formationParagraphs[i].GetParagraphText();
                item.Instituition = formationParagraphs[i+1].GetParagraphText();

                if (string.IsNullOrEmpty(item.Titre) || string.IsNullOrEmpty(item.Instituition))
                    continue;

                Itens.Add(item);
            }
        }
    }
}
