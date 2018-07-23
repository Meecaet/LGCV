import { Component, OnInit, Input } from "@angular/core";
import { FormationAcademiqueViewModel } from "../../Models/FormationAcademique-model";

@Component({
  selector: "app-table-formation-academique",
  templateUrl: "./table-formation-academique.component.html",
  styleUrls: ["./table-formation-academique.component.css"]
})
export class TableFormationAcademiqueComponent implements OnInit {
  formationAcademique: Array<FormationAcademiqueViewModel>;
  constructor() {
    this.formationAcademique = new Array<FormationAcademiqueViewModel>();
  }

  ngOnInit() {}
  AddFormationAcademique(): void {
    this.formationAcademique.push(new FormationAcademiqueViewModel());
  }

  removeFormationAcademique(
    ele: any,
    button: any,
    form: FormationAcademiqueViewModel
  ) {


    if (confirm("Vous voulez supprime ?")) {
      form.highlight = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(form);
    }
  }
  deleteFromDatabase(form: FormationAcademiqueViewModel) {
    alert("to implement");
  }
  OrderBy(): void {
    this.formationAcademique = Array.from(this.formationAcademique).sort(
      (item1: any, item2: any) => {
        return this.orderByComparator(item2["annee"], item1["annee"]);
      }
    );
  }
  private orderByComparator(a: any, b: any): number {
    if (
      isNaN(parseFloat(a)) ||
      !isFinite(a) ||
      (isNaN(parseFloat(b)) || !isFinite(b))
    ) {
      if (a < b) return -1;
      if (a > b) return 1;
    } else {
      if (parseFloat(a) < parseFloat(b)) return -1;
      if (parseFloat(a) > parseFloat(b)) return 1;
    }
    return 0;
  }
}
