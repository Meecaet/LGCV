import { Observable, observable } from "rxjs";
import { FormControl } from "@angular/forms";
import { EditionObjecViewModel } from "./EditionObjec-model";

export class LangueViewModel {
  graphId: string;
  nom: string;
  niveauParle: string;
  niveauEcrit: string;
  niveauLu: string;
  LangueControl: FormControl = new FormControl();
  editionObjecViewModels: Array<EditionObjecViewModel>;


}
