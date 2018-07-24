import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";

@Component({
  selector: "app-table-mandat",
  templateUrl: "./table-mandat.component.html",
  styleUrls: ["./table-mandat.component.css"]
})
export class TableMandatComponent implements OnInit {
  showMandat: boolean = false;
  @Input("mandatCollection") mandatCollection: Array<MandatViewModel>;
  @Input("numberPage") numberPage: number = 0;
  @Output("AddNewMandat") AddNewMandat = new EventEmitter();

  @Output("onChangeMandatFromTableMandat")
  onChangeMandatFromTableMandat = new EventEmitter();
  constructor() {
    this.mandatCollection = new Array<MandatViewModel>();
  }

  ngOnInit() {}
  addMandat() {
    this.numberPage = this.mandatCollection.length + 1;
    this.AddNewMandat.emit({
      newMandat: new MandatViewModel(),
      numberPage: this.numberPage,
      status:'ajouter'
    });
  }
  removeMandat(ele: any, button: any, mand: MandatViewModel) {
    var eleStyle = document.getElementById(ele);
    if (confirm("Vous voulez supprime ?")) {
      mand.highlight = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(mand);
    }
  }
  deleteFromDatabase(form: MandatViewModel) {
    alert("to implement");
  }
  selectMandat(indexMandat: number, mandat: MandatViewModel) {
    this.onChangeMandatFromTableMandat.emit({
      indexMandat: indexMandat + 1,
      mandat: mandat,
      mandatStatus:mandat.mandatStatus
    });
  }
}
