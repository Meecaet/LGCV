using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlHandler.XmlIdentification
{
    class MandatTextToken : TextToken
    {
        private Dictionary<string, string[]> mandatTextTokens;

        private MandatTextToken()
        {
            mandatTextTokens = new Dictionary<string, string[]>
            {
                { "ENVIRONNEMENT TECHNOLOGIQUE", new string[] { "ENVIRONNEMENT TECHNOLOGIQUE" } },
                { "LES TÂCHES SUIVANTES", new string[] { "LES TÂCHES SUIVANTES", "PROJET CONSISTAIT À" , "LES RESPONSABILITÉS SUIVANTES", "A ÉTÉ RESPONSABLE DE" } }
            };
        }


        protected override Dictionary<string, string[]> GetTokens()
        {
            return mandatTextTokens;
        }

        protected override bool Comparaison(KeyValuePair<string, string[]> token, string innerText)
        {
            if(token.Key == "LES TÂCHES SUIVANTES")
              return token.Value.Any(x => innerText.ToUpper().Contains(x));
            if(token.Key == "ENVIRONNEMENT TECHNOLOGIQUE")
              return token.Value.Any(x => innerText.ToUpper().StartsWith(x));

            return false;
        }

        public static MandatTextToken CreateTextToken()
        {
            return new MandatTextToken();
        }

    }
}
