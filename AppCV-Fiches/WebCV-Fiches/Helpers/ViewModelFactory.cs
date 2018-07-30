using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Helpers
{
    public class ViewModelFactory<T,V> where  T : GraphObject, new() where V : ViewModel, new()
    {
        public static List<ViewModel> GetViewModels(string utilisateurId, List<GraphObject> noeudsModifie, List<GraphObject> graphObjects, Func<GraphObject, ViewModel> map)
        {
            var graphObjectsViewModel = graphObjects.Select(map).ToList();

            var type = typeof(EditionObjectViewModelFactory<>).MakeGenericType(typeof(V));
            dynamic factory = Activator.CreateInstance(type);
            var editionObjecViewModels = (List<EditionObjecViewModel>)factory.GetEditions(noeudsModifie);

            dynamic repo = GraphRepositoy.GetGenericRepository(typeof(T));

            foreach (var edition in editionObjecViewModels)
            {
                var viewModel = graphObjectsViewModel.Find(x => x.GraphId == edition.EditionId);
                if (viewModel != null)
                    viewModel.editionObjecViewModels.Add(edition);
                else
                {
                    //var newGraphObject = repo.GetElementsFromTransversal(edition.GraphIdEdition, "Ajoute").First();
                    var editionModel = new EditionObjectGraphRepository().GetOne(edition.GraphIdEdition);
                    dynamic newGraphObject = editionModel.ObjetAjoute;
                    if (newGraphObject == null) continue;
                    var newGraphObjectViewModel = map.Invoke(newGraphObject);
                    newGraphObjectViewModel.editionObjecViewModels.Add(edition);
                    graphObjectsViewModel.Add(newGraphObjectViewModel);
                }

            }

            return graphObjectsViewModel;
        }
    }
}
