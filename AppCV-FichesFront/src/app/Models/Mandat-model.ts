import { TacheViewModel } from "./Tache-model";
import { TechnologieViewModel } from "./Technologie-model";

export class MandatViewModel {
  GraphId: string;
  GraphIdProjet: string;
  GraphIdClient: string;
  GraphIdFonction: string;
  GraphIdSocieteDeConseil: string;

  NomClient: string;
  NumeroMandat: Number;
  NomEntreprise: string;
  TitreProjet: string;
  TitreMandat: string;
  Envergure: Number;
  Efforts: Number;
  Fonction: string;
  ContexteProjet: string;
  PorteeDesTravaux: string;
  Taches: Array<TacheViewModel>;
  Technologies: Array<TechnologieViewModel>;

  DebutProjet: Date;
  FinProjet: Date;
  DebutMandat: Date;
  FinMandat: Date;

  NomReference: string;
  FonctionReference: string;
  TelephoneReference: string;
  CellulaireReference: string;
  CourrielReference: string;

  constructor() {
    this.Taches = new Array<TacheViewModel>();
    this.Technologies = new Array<TechnologieViewModel>();
  }
}
