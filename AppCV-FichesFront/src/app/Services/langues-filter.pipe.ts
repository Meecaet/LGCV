import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
  name: "languesFilter"
})
export class LanguesFilterPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    debugger;
    return value.filter(x => { debugger;
                            return !args.filter(f => { debugger;
                              return !f.graphId == x.graphId;
                               });
                        });
  }
}
