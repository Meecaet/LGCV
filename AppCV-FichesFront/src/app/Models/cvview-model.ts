import { DomaineDInterventionViewModel } from "./DomaineDIntervention-model";
import { FormationAcademiqueViewModel } from "./FormationAcademique-model";
import { CertificationViewModel } from "./Certification-model";
import { MandatViewModel } from "./Mandat-model";
import { TechnologieViewModel } from "./Technologie-model";
import { PerfectionnementViewModel } from "./Perfectionnement-model";
import { LangueViewModel } from "./Langue-model";

export class CVViewModel {
  public graphIdConseiller: string;
  public graphIdUtilisateur: string;
  public graphIdFonction: string;
  public graphIdCV: string;

  public prenom: string;
  public nom: string;
  public fonction: string;
  public biographie: string;
  public domainesdintervention: Array<DomaineDInterventionViewModel>;
  public formationsacademique: Array<FormationAcademiqueViewModel>;

  certifications: Array<CertificationViewModel>;
  mandats: Array<MandatViewModel>;
  technologies: Array<TechnologieViewModel>;
  perfectionnements: Array<PerfectionnementViewModel>;
  langues: Array<LangueViewModel>;

  constructor() {
    this.domainesdintervention = new Array<DomaineDInterventionViewModel>();
    this.formationsacademique = new Array<FormationAcademiqueViewModel>();
    this.certifications = new Array<CertificationViewModel>();
    this.mandats = new Array<MandatViewModel>();
    this.technologies = new Array<TechnologieViewModel>();
    this.perfectionnements = new Array<PerfectionnementViewModel>();
    this.langues = new Array<LangueViewModel>();
  }
}
