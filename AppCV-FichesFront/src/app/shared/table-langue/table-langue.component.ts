import { Component, OnInit, Input } from "@angular/core";
import { LangueViewModel } from "../../Models/Langue-model";
import { FormControl } from "../../../../node_modules/@angular/forms";

@Component({
  selector: "app-table-langue",
  templateUrl: "./table-langue.component.html",
  styleUrls: ["./table-langue.component.css"]
})
export class TableLangueComponent implements OnInit {
  @Input() langues: Array<LangueViewModel>;
  @Input() languesAutoComplete: Array<LangueViewModel>;

  AddLangue(): void {
    let lan = new LangueViewModel();
    lan.LangueControl = new FormControl();
    this.langues.push(lan);
  }
  removeLange(ele: LangueViewModel) {
    const index = this.langues.findIndex(
      x =>
        x.niveauecrit == ele.niveauecrit &&
        x.niveaulu == ele.niveaulu &&
        x.niveauparle == ele.niveauparle &&
        x.nom == ele.nom
    );
    if (index >= 0) {
      this.langues.splice(index, 1);
    }
  }
  constructor() {}

  ngOnInit() {}
}
