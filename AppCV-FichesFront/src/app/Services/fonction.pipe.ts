import { Pipe, PipeTransform } from '@angular/core';
import { FonctionViewModel } from '../Models/Fonction-model';

@Pipe({
  name: 'fonctionFilter',
  pure:false
})
export class FonctionPipe implements PipeTransform {

  transform(value: Array<FonctionViewModel>, args?: any): Array<FonctionViewModel> {
    if (args === "" || args === null) {
      return value;
    } else {
      return value.filter((x: FonctionViewModel) => {
        return x.nom.toLowerCase().startsWith(args.toLowerCase());
      });
    }
  }

}
