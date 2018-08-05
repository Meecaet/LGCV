using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches.Repositories.Graph;
using WebCV_Fiches.Helpers;
using Microsoft.AspNetCore.Routing;
using System.IO;

namespace WebCV_Fiches.Controllers
{
    [Route("api/FichierWord")]
    public class FichierWordController : ControllerBase
    {
        [Route("{utilisateurId}")]
        public async System.Threading.Tasks.Task<IActionResult> IndexAsync(string utilisateurId)
        {
            WordWriter wordWriter;
            Utilisateur utilisateur;

            UtilisateurGraphRepository utilisateurGraph = new UtilisateurGraphRepository();
            utilisateur = utilisateurGraph.GetOne(utilisateurId);

            var path = "Files";
            var fileDirectory = new DirectoryInfo(path);
            if (!fileDirectory.Exists)
                fileDirectory.Create();

            var fileName = $"{utilisateur.Nom}.docx";
            var filePath = $"{path}\\{fileName}";
            using (var document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                wordWriter = new WordWriter(document, utilisateur);
                wordWriter.CreateDummyTest();

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                System.IO.File.Delete(filePath);
                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                return File(memory, "application/vnd.ms-word", fileName);
            }

        }
    }
}