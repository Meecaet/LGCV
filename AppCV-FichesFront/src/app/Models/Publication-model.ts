import { EditionObjecViewModel } from "./EditionObjec-model";

export class PublicationViewModel {
  graphIdGenre: string;
  description: string;
  annee: Number;
  graphId: string;
  editionObjecViewModels: Array<EditionObjecViewModel>;

  constructor() {
    this.editionObjecViewModels = new Array<EditionObjecViewModel>();


  }
}
