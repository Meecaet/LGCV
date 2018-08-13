import { EditionObjecViewModel } from "./EditionObjec-model";

export class FormationAcademiqueViewModel {
  graphId: string;
  graphIdEtablissement: string;
  diplome: string;
  annee: Number;
  etablissement: string;
  niveau: number;
  highlight: string;
  editionObjecViewModels: Array<EditionObjecViewModel>;
 /**
  *
  */
 constructor() {
   this.editionObjecViewModels = new Array<EditionObjecViewModel>();

 }
}
