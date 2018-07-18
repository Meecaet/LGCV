import { Component, OnInit, Input } from "@angular/core";
import { PerfectionnementViewModel } from "../../Models/Perfectionnement-model";

@Component({
  selector: "app-table-perfectionnements",
  templateUrl: "./table-perfectionnements.component.html",
  styleUrls: ["./table-perfectionnements.component.css"]
})
export class TablePerfectionnementsComponent implements OnInit {
 @Input() perfectionnement: Array<PerfectionnementViewModel>;

  constructor() {}

  ngOnInit() {}

  AddPerfectionnement(): void {
    this.perfectionnement.push(new PerfectionnementViewModel());
  }
  removePerfectionnement(ele: PerfectionnementViewModel) {
    const index = this.perfectionnement.findIndex(
      x => x.Annee == ele.Annee && x.Description == ele.Description
    );
    if (index >= 0) {
      this.perfectionnement.splice(index, 1);
    }
  }
}
