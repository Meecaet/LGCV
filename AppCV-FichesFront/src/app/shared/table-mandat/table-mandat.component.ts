import { Component, OnInit, Input } from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";

@Component({
  selector: "app-table-mandat",
  templateUrl: "./table-mandat.component.html",
  styleUrls: ["./table-mandat.component.css"]
})
export class TableMandatComponent implements OnInit {
  @Input() mandats: Array<MandatViewModel>;
  constructor() {}

  ngOnInit() {}
  addMandat() {
    this.mandats.push(new MandatViewModel());
  }
  removeMandat(ele: MandatViewModel) {
    const index = this.mandats.findIndex(
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
      this.mandats.splice(index, 1);
    }
  }
}
