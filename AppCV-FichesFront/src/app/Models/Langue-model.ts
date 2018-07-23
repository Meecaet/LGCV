import { Observable, observable } from "rxjs";
import { FormControl } from "@angular/forms";

export class LangueViewModel {
  graphid: string;
  nom: string;
  niveauparle: string;
  niveauecrit: string;
  niveaulu: string;

  LangueControl: FormControl;
 LoadLangue(langues: Array<LangueViewModel>) {
    return new Observable(observable => {
      observable.next(langues);
      observable.complete();
    });
  }

}
