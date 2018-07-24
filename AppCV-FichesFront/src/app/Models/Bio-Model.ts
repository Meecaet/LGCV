import { EditionObjecViewModel } from "./EditionObjec-model";
import { BioHighlight } from "./Highlight/Bio-Highlight";

export class BioViewModel {
  nom: string;
  prenom: string;
  resumeExperience: string;
  fonction: string;
  editionObjecViewModels: Array<EditionObjecViewModel>;
  bioHighlight:BioHighlight

  constructor() {
    this.editionObjecViewModels = new Array<EditionObjecViewModel>();
    this.bioHighlight =new BioHighlight();
  }
}
