import { Component, OnInit, Input } from "@angular/core";
import { PerfectionnementViewModel } from "../../Models/Perfectionnement-model";

@Component({
  selector: "app-table-perfectionnements",
  templateUrl: "./table-perfectionnements.component.html",
  styleUrls: ["./table-perfectionnements.component.css"]
})
export class TablePerfectionnementsComponent implements OnInit {
  perfectionnement: Array<PerfectionnementViewModel>;

  constructor() {
    this.perfectionnement = new Array<PerfectionnementViewModel>();
  }

  ngOnInit() {}

  AddPerfectionnement(): void {
    this.perfectionnement.push(new PerfectionnementViewModel());
  }
  removePerfectionnement(
    ele: any,
    button: any,
    perc: PerfectionnementViewModel
  ) {
    var eleStyle = document.getElementById(ele);
    if (confirm("Vous voulez supprime ?")) {
      perc.highlight = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(perc);
    }
  }
  deleteFromDatabase(form: PerfectionnementViewModel) {
    alert("to implement");
  }
  OrderBy(): void {
    this.perfectionnement = Array.from(this.perfectionnement).sort(
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
