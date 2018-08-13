import { TechnologieViewModel } from "./Technologie-model";

export class TechnologieByCategorieViewModel {
  categorie: string;
  dataByCategorie: Array<TechnologieViewModel>;
  hidden: any = true;
  /**
   *
   */
  constructor() {
    this.dataByCategorie = new Array<TechnologieViewModel>();
  }


}
