using DAL_CV_Fiches.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCV_Fiches.Models.CVViewModels;

namespace WebCV_Fiches.Helpers
{
    public class CVMapper
    {
        public CVViewModel Map(Conseiller conseiller)
        {
            int intForParse = 0;
            CVViewModel cVViewModel = new CVViewModel();

            cVViewModel.GraphId = conseiller.GraphKey;
            cVViewModel.Prenom = conseiller.Utilisateur.Prenom;
            cVViewModel.Nom = conseiller.Utilisateur.Nom;
            
            if(string.IsNullOrEmpty(cVViewModel.Prenom) && string.IsNullOrEmpty(cVViewModel.Nom))
                cVViewModel.Nom = conseiller.Utilisateur.NomComplet;

            cVViewModel.Fonction = conseiller.Fonction.Description;
            cVViewModel.Biographie = conseiller.CVs.First().ResumeExperience;

            foreach (DomaineDIntervention domaineDIntervention in conseiller.DomaineDInterventions)
                cVViewModel.DomainesDIntervention.Add(new DomaineDInterventionViewModel { GraphId = domaineDIntervention.GraphKey, Description = domaineDIntervention.Description });

            foreach (FormationScolaire formationScolaire in conseiller.FormationsScolaires)
            {
                FormationAcademiqueViewModel formationAcademique = new FormationAcademiqueViewModel();
                formationAcademique.Diplome = formationScolaire.Diplome;
                formationAcademique.Annee = formationScolaire.DateConclusion.Year;
                formationAcademique.Etablissement = formationScolaire.Ecole.Nom;

                cVViewModel.FormationsAcademique.Add(formationAcademique);
            }

            foreach (Formation certification in conseiller.Formations.Where(form => form.Type.Descriminator == "Formation" && form.Type.Description == "Certification"))
                cVViewModel.Certifications.Add(new CertificationViewModel { GraphId = certification.GraphKey, Description = certification.Description });

            foreach (Mandat mandat in conseiller.Mandats)
            {
                MandatViewModel mandatViewModel = new MandatViewModel();
                mandatViewModel.GraphId = mandat.GraphKey;
                mandatViewModel.NomClient = mandat.Projet.Client.Nom;

                int.TryParse(mandat.Numero, out intForParse);
                if(intForParse > 0)
                    mandatViewModel.NumeroMandat = intForParse;

                mandatViewModel.NomEntreprise = mandat.Projet.SocieteDeConseil.Nom;
                mandatViewModel.TitreProjet = mandat.Projet.Nom;
                mandatViewModel.TitreMandat = mandat.Titre;
                mandatViewModel.Envergure = mandat.Projet.Envergure;
                mandatViewModel.Efforts = mandat.Efforts;
                mandatViewModel.Fonction = mandat.Fonction.Description;
                mandatViewModel.ContexteProjet = mandat.Projet.Description;
                mandatViewModel.PorteeDesTravaux = mandat.Description;
                mandatViewModel.DebutProjet = mandat.Projet.DateDebut;
                mandatViewModel.FinProjet = mandat.Projet.DateFin;
                mandatViewModel.DebutMandat = mandat.DateDebut;
                mandatViewModel.FinMandat = mandat.DateFin;
                mandatViewModel.NomReference = mandat.Projet.NomReference;
                mandatViewModel.TelephoneReference = mandat.Projet.TelephoneReference;
                mandatViewModel.CellulaireReference = mandat.Projet.CellulaireReference;
                mandatViewModel.CourrielReference = mandat.Projet.CourrielReference;
                mandatViewModel.FonctionReference = mandat.Projet.FonctionReference;

                foreach (Technologie technologie in mandat.Projet.Technologies)
                    mandatViewModel.Technologies.Add(new TechnologieViewModel { GraphId = technologie.GraphKey, Description = technologie.Description });

                foreach (Tache tache in mandat.Taches)
                    mandatViewModel.Taches.Add(new TacheViewModel{ GraphId = tache.GraphKey, Description = tache.Description });

            }

            foreach (Technologie technologie in conseiller.Technologies)
                cVViewModel.Technologies.Add(new TechnologieViewModel { GraphId = technologie.GraphKey, Description = technologie.Nom, Mois = technologie.MoisDExperience });

            foreach (Formation certification in conseiller.Formations.Where(form => form.Type.Descriminator == "Formation" && form.Type.Description == "Perfectionnement"))
                cVViewModel.Perfectionnements.Add(new PerfectionnementViewModel { GraphId = certification.GraphKey, Description = certification.Description });

            foreach (Langue langue in conseiller.Langues)
            {
                LangueViewModel langueViewModel = new LangueViewModel();
                langueViewModel.GraphId = langue.GraphKey;
                langueViewModel.Nom = langue.Nom;
                langueViewModel.NiveauParle = langue.Parle.ToString();
                langueViewModel.NiveauLu = langue.Lu.ToString();
                langueViewModel.NiveauEcrit = langue.Ecrit.ToString();

                cVViewModel.Langues.Add(langueViewModel);
            }

            return cVViewModel;
        }

        public Conseiller Map(CVViewModel cvViewModel)
        {
            return null;           
        }
    }
}
