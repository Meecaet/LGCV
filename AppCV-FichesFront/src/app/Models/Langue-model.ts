import { Observable, observable } from "../../../node_modules/rxjs";
import { FormControl } from "../../../node_modules/@angular/forms";

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
