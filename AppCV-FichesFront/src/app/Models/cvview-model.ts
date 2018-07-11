import { DomaineDInterventionViewModel } from "./DomaineDIntervention-model";
import { FormationAcademiqueViewModel } from "./FormationAcademique-model";
import { CertificationViewModel } from "./Certification-model";
import { MandatViewModel } from "./Mandat-model";
import { TechnologieViewModel } from "./Technologie-model";
import { PerfectionnementViewModel } from "./Perfectionnement-model";
import { LangueViewModel } from "./Langue-model";

export class CVViewModel {
  public GraphIdConseiller: string;
  public GraphIdUtilisateur: string;
  public GraphIdFonction: string;
  public GraphIdCV: string;

  public Prenom: string;
  public Nom: string;
  public Fonction: string;
  public Biographie: string;
  public DomainesDIntervention: Array<DomaineDInterventionViewModel>;
  public FormationsAcademique: Array<FormationAcademiqueViewModel>;

  Certifications: Array<CertificationViewModel>;
  Mandats: Array<MandatViewModel>;
  Technologies: Array<TechnologieViewModel>;
  Perfectionnements: Array<PerfectionnementViewModel>;
  Langues: Array<LangueViewModel>;

  constructor() {
    this.DomainesDIntervention = new Array<DomaineDInterventionViewModel>();
    this.FormationsAcademique = new Array<FormationAcademiqueViewModel>();
    this.Certifications = new Array<CertificationViewModel>();
    this.Mandats = new Array<MandatViewModel>();
    this.Technologies = new Array<TechnologieViewModel>();
    this.Perfectionnements = new Array<PerfectionnementViewModel>();
    this.Langues = new Array<LangueViewModel>();
  }
}
