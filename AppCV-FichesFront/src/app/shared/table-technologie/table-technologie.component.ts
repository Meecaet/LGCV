import { Component, OnInit, Input } from "@angular/core";
import { TechnologieViewModel } from "../../Models/Technologie-model";

@Component({
  selector: "app-table-technologie",
  templateUrl: "./table-technologie.component.html",
  styleUrls: ["./table-technologie.component.css"]
})
export class TableTechnologieComponent implements OnInit {
  technologies: Array<TechnologieViewModel>;
  constructor() {
    this.technologies = new Array<TechnologieViewModel>();
  }

  ngOnInit() {}
  addTechnologie(): void {
    this.technologies.push(new TechnologieViewModel());
  }
  removeTechonolie(ele: TechnologieViewModel): void {
    const index = this.technologies.findIndex(
      x => x.description == ele.description && x.mois == ele.mois
    );
    if (index >= 0) {
      this.technologies.splice(index, 1);
    }
  }
}
