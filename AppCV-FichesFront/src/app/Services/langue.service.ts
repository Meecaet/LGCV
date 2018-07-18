import { Pipe, PipeTransform } from "@angular/core";
import { LangueViewModel } from "../Models/Langue-model";


@Pipe({
  name: "langueService",
  pure: false
})
export class LangueService implements PipeTransform {
  constructor() {}
  transform(value: Array<LangueViewModel>, args?: any): Array<LangueViewModel> {
    if (args === "" || args === null) {
      return value;
    } else {
      return value.filter((x: LangueViewModel) => {
        return x.nom.toLowerCase().startsWith(args.toLowerCase());
      });
    }
  }

}
