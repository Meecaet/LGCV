import { Component, OnInit, Input } from "@angular/core";
import { FormationAcademiqueViewModel } from "../../Models/FormationAcademique-model";

@Component({
  selector: "app-table-formation-academique",
  templateUrl: "./table-formation-academique.component.html",
  styleUrls: ["./table-formation-academique.component.css"]
})
export class TableFormationAcademiqueComponent implements OnInit {
  @Input() formationAcademique: Array<FormationAcademiqueViewModel>;
  constructor() {}

  ngOnInit() {}
  AddFormationAcademique(): void {
    this.formationAcademique.push(new FormationAcademiqueViewModel());
  }

  removeFormationAcademique(ele: FormationAcademiqueViewModel) {
    const index = this.formationAcademique.findIndex(
      x =>
        x.diplome == ele.diplome &&
        x.annee == ele.annee &&
        x.etablissement == ele.etablissement
    );
    if (index >= 0) {
      this.formationAcademique.splice(index, 1);
    }
  }
}
