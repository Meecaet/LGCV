import { EditionObjecViewModel } from "./EditionObjec-model";

export class TechnologieViewModel {
  graphId: string;
  description: String;
  categorie:string;
  mois: number;
  editionObjecViewModels: Array<EditionObjecViewModel>;

  /**
   *
   */
  constructor() {
    this.editionObjecViewModels = new Array<EditionObjecViewModel>();

  }


}
