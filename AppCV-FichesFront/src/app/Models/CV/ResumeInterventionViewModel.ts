import { EditionObjecViewModel } from "../EditionObjec-model";

export class ResumeInterventionViewModel {
  nombre: number;
  entrerise: string;
  client: string;
  projet: string;
  envergure: string;
  fonction: string;
  annee: number;
  effors: number;
  debutMandat: Date;
  editionObjecViewModels: Array<EditionObjecViewModel>;
  highlight: string;
  constructor() {
    this.editionObjecViewModels = new Array<EditionObjecViewModel>();
  }
}
