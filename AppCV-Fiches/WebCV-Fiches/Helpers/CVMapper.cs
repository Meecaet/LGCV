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
        public CVViewModel Map(Utilisateur utilisateur)
        {
            int intForParse = 0;
            CVViewModel cVViewModel = new CVViewModel();

            cVViewModel.GraphIdConseiller = utilisateur.Conseiller.GraphKey;
            cVViewModel.GraphIdUtilisateur = utilisateur.GraphKey;
            cVViewModel.GraphIdFonction = utilisateur.Conseiller.Fonction.GraphKey;
            cVViewModel.GraphIdCV = utilisateur.Conseiller.CVs.First().GraphKey;

            cVViewModel.Prenom = utilisateur.Prenom;
            cVViewModel.Nom = utilisateur.Nom;
            
            if(string.IsNullOrEmpty(cVViewModel.Prenom) && string.IsNullOrEmpty(cVViewModel.Nom))
                cVViewModel.Nom = utilisateur.NomComplet;

            cVViewModel.Fonction = utilisateur.Conseiller.Fonction.Description;
            cVViewModel.Biographie = utilisateur.Conseiller.CVs.First().ResumeExperience;

            foreach (DomaineDIntervention domaineDIntervention in utilisateur.Conseiller.DomaineDInterventions)
                cVViewModel.DomainesDIntervention.Add(new DomaineDInterventionViewModel { GraphId = domaineDIntervention.GraphKey, Description = domaineDIntervention.Description });

            foreach (FormationScolaire formationScolaire in utilisateur.Conseiller.FormationsScolaires)
            {
                FormationAcademiqueViewModel formationAcademique = new FormationAcademiqueViewModel();
                formationAcademique.GraphId = formationScolaire.GraphKey;
                formationAcademique.GraphIdEtablissement = formationScolaire.Ecole.GraphKey;
                formationAcademique.Diplome = formationScolaire.Diplome;
                formationAcademique.Annee = formationScolaire.DateConclusion.Year;
                formationAcademique.Etablissement = formationScolaire.Ecole.Nom;

                cVViewModel.FormationsAcademique.Add(formationAcademique);
            }

            foreach (Formation certification in utilisateur.Conseiller.Formations.Where(form => form.Type.Descriminator == "Formation" && form.Type.Description == "Certification"))
                cVViewModel.Certifications.Add(new CertificationViewModel { GraphId = certification.GraphKey, Description = certification.Description, GraphIdGenre = certification.Type.GraphKey });

            foreach (Mandat mandat in utilisateur.Conseiller.Mandats)
            {
                MandatViewModel mandatViewModel = new MandatViewModel();
                mandatViewModel.GraphId = mandat.GraphKey;
                mandatViewModel.GraphIdProjet = mandat.Projet.GraphKey;
                mandatViewModel.GraphIdClient = mandat.Projet.Client.GraphKey;
                mandatViewModel.GraphIdSocieteDeConseil = mandat.Projet.SocieteDeConseil.GraphKey;

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

            foreach (Technologie technologie in utilisateur.Conseiller.Technologies)
                cVViewModel.Technologies.Add(new TechnologieViewModel { GraphId = technologie.GraphKey, Description = technologie.Nom, Mois = technologie.MoisDExperience });

            foreach (Formation certification in utilisateur.Conseiller.Formations.Where(form => form.Type.Descriminator == "Formation" && form.Type.Description == "Perfectionnement"))
                cVViewModel.Perfectionnements.Add(new PerfectionnementViewModel { GraphId = certification.GraphKey, Description = certification.Description });

            foreach (Langue langue in utilisateur.Conseiller.Langues)
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

        public Utilisateur Map(CVViewModel cVViewModel)
        {
            Utilisateur utilisateur = new Utilisateur();

            utilisateur.GraphKey = cVViewModel.GraphIdUtilisateur;
            utilisateur.Prenom = cVViewModel.Prenom;
            utilisateur.Nom = cVViewModel.Nom;

            utilisateur.Conseiller = new Conseiller { GraphKey = cVViewModel.GraphIdConseiller };


            utilisateur.Conseiller.Fonction = new Fonction();
            utilisateur.Conseiller.Fonction.GraphKey = cVViewModel.GraphIdFonction;
            utilisateur.Conseiller.Fonction.Description = cVViewModel.Fonction;

            utilisateur.Conseiller.CVs.Add(new CV());
            utilisateur.Conseiller.CVs.First().GraphKey = cVViewModel.GraphIdCV;
            utilisateur.Conseiller.CVs.First().ResumeExperience = cVViewModel.Biographie;

            foreach (DomaineDInterventionViewModel domaineDIntervention in cVViewModel.DomainesDIntervention)
                utilisateur.Conseiller.DomaineDInterventions.Add(new DomaineDIntervention { GraphKey = domaineDIntervention.GraphId, Description = domaineDIntervention.Description });

            foreach (FormationAcademiqueViewModel formationAcademique in cVViewModel.FormationsAcademique)
            {
                FormationScolaire formationScolaire = new FormationScolaire();
                formationScolaire.GraphKey = formationAcademique.GraphId;
                formationScolaire.Diplome = formationAcademique.Diplome;
                formationScolaire.DateConclusion = DateTime.Parse($"{formationAcademique.Annee}-01-01");
                formationScolaire.Ecole = new Instituition();
                formationScolaire.Ecole.GraphKey = formationAcademique.GraphIdEtablissement;
                formationScolaire.Ecole.Nom = formationAcademique.Etablissement;

                utilisateur.Conseiller.FormationsScolaires.Add(formationScolaire);
            }

            foreach (CertificationViewModel certification in cVViewModel.Certifications)
                utilisateur.Conseiller.Formations.Add(new Formation { GraphKey = certification.GraphId, Description = certification.Description, Type = new Genre { GraphKey = certification.GraphIdGenre, Descriminator = "Formation", Description = "Certification" }});

            foreach (MandatViewModel mandatViewModel in cVViewModel.Mandats)
            {
                Mandat mandat = new Mandat();
                mandat.GraphKey = mandatViewModel.GraphId;

                mandat.Projet = new Projet { GraphKey = mandatViewModel.GraphIdProjet };
                mandat.Projet.Client = new Client { GraphKey = mandatViewModel.GraphIdClient };
                mandat.Projet.Client.Nom = mandatViewModel.NomClient;

                mandat.Numero = mandatViewModel.NumeroMandat.ToString();

                mandat.Projet.SocieteDeConseil = new Employeur { GraphKey = mandatViewModel.GraphIdSocieteDeConseil };
                mandat.Projet.SocieteDeConseil.Nom = mandatViewModel.NomEntreprise;
                mandat.Projet.Nom = mandatViewModel.TitreProjet;
                mandat.Titre = mandatViewModel.TitreMandat;
                mandat.Projet.Envergure = mandatViewModel.Envergure;
                mandat.Efforts = mandatViewModel.Efforts;

                mandat.Fonction = new Fonction { GraphKey = mandatViewModel.GraphIdFonction };
                mandat.Fonction.Description = mandatViewModel.Fonction;
                mandat.Projet.Description = mandatViewModel.ContexteProjet;
                mandat.Description = mandatViewModel.PorteeDesTravaux;
                mandat.Projet.DateDebut = mandatViewModel.DebutProjet;
                mandat.Projet.DateFin = mandatViewModel.FinProjet;
                mandat.DateDebut = mandatViewModel.DebutMandat;
                mandat.DateFin = mandatViewModel.FinMandat;
                mandat.Projet.NomReference = mandatViewModel.NomReference;
                mandat.Projet.TelephoneReference = mandatViewModel.TelephoneReference;
                mandat.Projet.CellulaireReference = mandatViewModel.CellulaireReference;
                mandat.Projet.CourrielReference = mandatViewModel.CourrielReference;
                mandat.Projet.FonctionReference = mandatViewModel.FonctionReference;

                foreach (TechnologieViewModel technologie in mandatViewModel.Technologies)
                    mandat.Projet.Technologies.Add(new Technologie { GraphKey = technologie.GraphId, Nom = technologie.Description });

                foreach (TacheViewModel tache in mandatViewModel.Taches)
                    mandat.Taches.Add(new Tache { GraphKey = tache.GraphId, Description = tache.Description });
            }

            foreach (TechnologieViewModel technologie in cVViewModel.Technologies)
                utilisateur.Conseiller.Technologies.Add(new Technologie { GraphKey = technologie.GraphId, Nom = technologie.Description, MoisDExperience = technologie.Mois});

            foreach (PerfectionnementViewModel perfeccionnement in cVViewModel.Perfectionnements)
                utilisateur.Conseiller.Formations.Add(new Formation { GraphKey = perfeccionnement.GraphId, Description = perfeccionnement.Description, Type = new Genre { GraphKey = perfeccionnement.GraphIdGenre, Descriminator = "Formation", Description = "Perfectionnement" } });


            foreach (LangueViewModel langueViewModel in cVViewModel.Langues)
            {
                Langue langue = new Langue();
                langue.GraphKey = langueViewModel.GraphId;
                langue.Nom = langueViewModel.Nom;
                langue.Parle = (Niveau)System.Enum.Parse(typeof(Niveau),langueViewModel.NiveauParle);
                langue.Ecrit = (Niveau)System.Enum.Parse(typeof(Niveau), langueViewModel.NiveauEcrit);
                langue.Lu = (Niveau)System.Enum.Parse(typeof(Niveau), langueViewModel.NiveauLu);


                utilisateur.Conseiller.Langues.Add(langue);
            }

            return utilisateur;           
        }
    }
}
