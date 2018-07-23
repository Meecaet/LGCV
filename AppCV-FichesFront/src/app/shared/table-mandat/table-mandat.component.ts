import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";

@Component({
  selector: "app-table-mandat",
  templateUrl: "./table-mandat.component.html",
  styleUrls: ["./table-mandat.component.css"]
})
export class TableMandatComponent implements OnInit {
  @Input("mandatCollection") mandatCollection: Array<MandatViewModel>;
  @Input("numberPage") numberPage: number = 0;
  @Output("AddNewMandat") AddNewMandat = new EventEmitter();
  @Output("onChangeMandatFromTableMandat") onChangeMandatFromTableMandat = new EventEmitter();
  constructor() {
    this.mandatCollection = new Array<MandatViewModel>();
  }


  ngOnInit() {}
  addMandat() {
    this.numberPage = this.mandatCollection.length+1;
    this.AddNewMandat.emit({newMandat:new MandatViewModel(),numberPage:this.numberPage});
  }
  removeMandat(ele: MandatViewModel) {
    const index = this.mandatCollection.findIndex(
      x =>
        x.graphId == ele.graphId &&
        x.graphIdProjet == ele.graphIdProjet &&
        x.graphIdClient == ele.graphIdClient &&
        x.graphIdFonction == ele.graphIdFonction &&
        x.graphIdSocieteDeConseil == ele.graphIdSocieteDeConseil &&
        x.nomClient == ele.nomClient &&
        x.numeroMandat == ele.numeroMandat &&
        x.nomEntreprise == ele.nomEntreprise &&
        x.titreProjet == ele.titreProjet &&
        x.titreMandat == ele.titreMandat &&
        x.envergure == ele.envergure &&
        x.efforts == ele.efforts &&
        x.fonction == ele.fonction &&
        x.contexteProjet == ele.contexteProjet &&
        x.porteeDesTravaux == ele.porteeDesTravaux &&
        x.debutProjet == ele.debutProjet &&
        x.finProjet == ele.finProjet &&
        x.debutMandat == ele.debutMandat &&
        x.finMandat == ele.finMandat &&
        x.nomReference == ele.nomReference &&
        x.fonctionReference == ele.fonctionReference &&
        x.telephoneReference == ele.telephoneReference &&
        x.cellulaireReference == ele.cellulaireReference &&
        x.courrielReference == ele.courrielReference
    );
    if (index >= 0) {
      this.mandatCollection.splice(index, 1);
    }
  }
  selectMandat(indexMandat: number, mandat: MandatViewModel) {
    this.onChangeMandatFromTableMandat.emit({indexMandat:indexMandat+1,mandat:mandat});
  }
}
