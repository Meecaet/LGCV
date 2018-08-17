import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "languesFilter"
})
export class LanguesFilterPipe implements PipeTransform {
  transform(value: any, args?: any): any {

    return value.filter(x => {
   return !args.filter(f => {

        return !f.graphId == x.graphId;
      });
    });
  }
}
