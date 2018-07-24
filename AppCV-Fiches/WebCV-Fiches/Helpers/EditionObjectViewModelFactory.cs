using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Helpers
{

    public class EditionObjectViewModelFactory<T> where T : ViewModel, new()
    {
        private List<EditionObject> Editions;

        public EditionObjectViewModelFactory()
        {
            Editions = new List<EditionObject>();
        }

        public List<EditionObjecViewModel> GetEditions(List<GraphObject> graphObjects)
        {
            foreach (var obj in graphObjects)
            {
                AddEdtions(obj.EditionObjects);
            }

            return GetList();
        }

        public List<EditionObjecViewModel> GetEditions(params GraphObject[] graphObjects)
        {
            return GetEditions(graphObjects.ToList());
        }

        private void AddEdtions(List<EditionObject> editions)
        {
            if (editions != null)
            {
                foreach (var edition in editions)
                {
                    T type = new T();
                    if (type.HasEdtion(edition))
                    {
                        Editions.Add(edition);
                    }
                }
            }
        }

        private List<EditionObjecViewModel> GetList()
        {
            return Editions.Select(x =>
              new EditionObjecViewModel
              {
                  GraphIdEdition = x.GraphKey,
                  EditionId = GetEditionid(x),
                  Etat = x.Etat,
                  Observacao = x.Observacao,
                  ProprieteNom = x.ProprieteNom,
                  ProprieteValeur = x.ProprieteValeur,
                  Type = GetTypeFrom(x)
              }
             ).ToList();
        }

        private string GetEditionid(EditionObject edition)
        {
            if (!string.IsNullOrEmpty(edition.ObjetAjouteId))
                return edition.ObjetAjouteId;
            else if (!string.IsNullOrEmpty(edition.ObjetSupprimeId))
                return edition.ObjetSupprimeId;
            else
                return edition.NoeudModifie.GraphKey;
        }
        private string GetTypeFrom(EditionObject edtion)
        {
            if (edtion.Type == EditionObjectType.ChangementPropriete)
                return EditionObjectType.ChangementPropriete;

            if (edtion.Type == EditionObjectType.ChangementRelation)
            {
                if (!string.IsNullOrEmpty(edtion.ObjetAjouteId) && !string.IsNullOrEmpty(edtion.ObjetSupprimeId))
                    return EditionObjectType.ChangementRelation;

                if (!string.IsNullOrEmpty(edtion.ObjetAjouteId))
                    return "Addition";

                if (!string.IsNullOrEmpty(edtion.ObjetSupprimeId))
                    return "Enlever";
            }

            return string.Empty;
        }

    }
}
