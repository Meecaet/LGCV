import { FormControl } from "@angular/forms";
import { Observable } from "../../../node_modules/rxjs";

export class FonctionViewModel{
  graphId: string;
  nom: string;
  FonctionControl: FormControl = new FormControl();
  LoadFonction(fonctions: Array<FonctionViewModel>) {
     return new Observable(observable => {
       observable.next(fonctions);
       observable.complete();
     });
   }
}
