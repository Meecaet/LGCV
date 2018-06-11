using System.Linq;
using XmlHandler.XmlEntities;
using XmlHandler.XmlIdentification;
using System.Collections.Generic;
using System;

namespace CVModel.Domain
{
    [Serializable]
    public class Langue
    {
        public string Nom { get; set; }
        public Niveau Parle { get; set; }
        public Niveau Ecrit { get; set; }
        public Niveau Lu { get; set; }

        public Langue()
        {
            Parle = Ecrit = Lu = Niveau.Avancé;
        }

        private static List<CVSection> GetLangueSections(CVSection langueSection)
        {
            IXmlToken formatationToken = new FormatationToken(langueSection.NameTable);
            ((FormatationToken)formatationToken).SetStyleParameter(new KeyValuePair<string, string>("w:val", "Puce1"));

            List<XmlDocNode> langNodes = new List<XmlDocNode>(), langSectionNodes = new List<XmlDocNode>();
            XmlDocNode first;
            List<CVSection> LangueSections = new List<CVSection>();
            CVSection currentLangueSection = new CVSection(); ;

            string identifiant = string.Empty;
            currentLangueSection.Identifiant = "w:b";

            langSectionNodes = langueSection.Nodes.Skip(1).ToList();
            while (langSectionNodes.Count > 0)
            {
                first = langSectionNodes.First();
                langNodes.Add(first);

                langSectionNodes.Remove(first);
                if (formatationToken.Match(first.OriginalNode, out identifiant))
                {
                    if (currentLangueSection.Nodes.Count > 0)
                        LangueSections.Add(currentLangueSection);

                    currentLangueSection = new CVSection();
                    currentLangueSection.Identifiant = identifiant;

                    identifiant = string.Empty;
                }

                currentLangueSection.AddNode(first.OriginalNode);
            }

            LangueSections.Add(currentLangueSection);
            return LangueSections;
        }

        //private static Niveau GetNiveauByText(string niveauText)
        //{
            
        //}

        internal static List<Langue> AssemblerLangues(CVSection langueSection)
        {
            List<Langue> langues = new List<Langue>();

            foreach (CVSection section in GetLangueSections(langueSection))
            {
                Langue curLangue = new Langue();
                XmlDocParagraph langueNom = (XmlDocParagraph)section.Nodes.First(x => x is XmlDocParagraph);

                curLangue.Nom = langueNom.GetParagraphText();

                if (section.Nodes.Skip(1).Count() > 0)
                {
                    foreach (XmlDocParagraph langueNiveau in section.Nodes.Skip(1).Cast<XmlDocParagraph>().ToList())
                    {
                        string[] niveau = langueNiveau.GetParagraphText().Split(':');

                        if (niveau[0].Contains("Parlé"))
                            curLangue.Parle = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim());

                        if (niveau[0].Contains("Écrit"))
                            curLangue.Ecrit = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim());

                        if (niveau[0].Contains("Lu"))
                            curLangue.Lu = (Niveau)System.Enum.Parse(typeof(Niveau), niveau[1].Trim());
                    }
                }
                else
                {
                    curLangue.Parle = curLangue.Ecrit = curLangue.Lu = Niveau.Avancé;
                }

                langues.Add(curLangue);
            }

            return langues;
        }

    }

    public enum Niveau
    {
        Basique = 0,
        Intermédiaire = 1,
        Avancé = 2
    }
}