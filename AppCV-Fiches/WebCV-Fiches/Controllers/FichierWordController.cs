using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using WebCV_Fiches.Helpers;

namespace WebCV_Fiches.Controllers
{
    public class FichierWordController : Controller
    {
        public string Index()
        {
            WordWriter wordWriter;
            Utilisateur utilisateur;

            UtilisateurGraphRepository utilisateurGraph = new UtilisateurGraphRepository();
            utilisateur = utilisateurGraph.GetAll().First();

            try
            {
                using (var document = WordprocessingDocument.Create($"C:\\Docs to zip\\{utilisateur.Nom}_Test.docx", WordprocessingDocumentType.Document))
                {
                    wordWriter = new WordWriter(document, utilisateur);
                    wordWriter.CreateDummyTest();
                }
            }
            catch (Exception ex)
            {
                return $"Error : { ex.Message}";
            }

            return "Finished";
        }
    }    
}