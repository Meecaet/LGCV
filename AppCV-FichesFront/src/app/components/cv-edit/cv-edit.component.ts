import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { MandatViewModel } from "../../Models/Mandat-model";
import { ENTER, COMMA } from "@angular/cdk/keycodes";
@Component({
  selector: "app-cv-edit",
  templateUrl: "./cv-edit.component.html",
  styleUrls: ["./cv-edit.component.css"]
})
export class CvEditComponent implements OnInit {
  UtilisateurId: string;
  mandatCollection: Array<MandatViewModel>;
  mandatSeleted = new MandatViewModel();
  numberPage: number;
  lastPage: number;
  showMandat: boolean = false;
  hiddenButton: string;
  constructor(private route: ActivatedRoute) {
    this.mandatCollection = new Array<MandatViewModel>();
  }
  ngOnInit() {
    this.route.params.subscribe(params => {
      this.UtilisateurId = params["id"];
    });
  }
  setToMandat(event: MandatViewModel): void {
    document.getElementById("anchor").scrollIntoView();
    this.showMandat = false;
    this.mandatSeleted = new MandatViewModel();

    this.mandatCollection.push(event);
    this.lastPage = this.mandatCollection.length;
  }
  addNewMandatFromList(arg: any): void {
    // this.InputMandatCarousel = arg.newMandat;
    this.numberPage = arg.numberPage;
    this.showMandat = true;
    this.lastPage = this.mandatCollection.length;
    this.mandatSeleted = new MandatViewModel();
    debugger
    this.hiddenButton =  this.mandatSeleted.mandatStatus;
  }
  onChangePage(changeTo: number): void {
    const newValue = this.mandatCollection[changeTo];
    if (newValue != null) {
      this.mandatSeleted = newValue;
      this.numberPage = changeTo + 1;
    }
  }
  onChangeMandatFromTableMandat(arg: any): void {
    this.mandatSeleted = <MandatViewModel>arg.mandat;
    this.numberPage = arg.indexMandat;
    this.showMandat = true;
    debugger
    this.hiddenButton = this.mandatSeleted.mandatStatus;
  }
}
