import { Component, OnInit, Input } from "@angular/core";
import { PerfectionnementViewModel } from "../../Models/Perfectionnement-model";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-table-perfectionnements",
  templateUrl: "./table-perfectionnements.component.html",
  styleUrls: ["./table-perfectionnements.component.css"]
})
export class TablePerfectionnementsComponent implements OnInit {
  perfectionnement: Array<PerfectionnementViewModel>;
  @Input() UtilisateurId: string;

  showLoadingPerfectionnement: boolean = true;
  constructor(private cvServ: CVService,private servError : ErrorService) {
    this.perfectionnement = new Array<PerfectionnementViewModel>();
  }

  ngOnInit() {
  this.  LoadUserData();
  }
  LoadUserData():void{
    this.showLoadingPerfectionnement = true;
    this.cvServ
      .UtilizateurPerfectionement(this.UtilisateurId)
      .subscribe(obs => {
  this.perfectionnement = obs.filter((x: PerfectionnementViewModel) => {
          if (x.editionObjecViewModels.length < 1) {
            return x;
          } else if (
            !x.editionObjecViewModels.some(x => {
              return x.etat == "Modifie" && x.type == "Enlever";
            })
          ) {
            return x;
          }
        });
        this.showLoadingPerfectionnement = false;
      }, (error)=> this.Error(error));
  }
  Error(error: HttpErrorResponse) {

    this.servError.ErrorHandle(error.status);
  }
  AddPerfectionnement(): void {
    this.perfectionnement.push(new PerfectionnementViewModel());
  }
  removePerfectionnement(perc: PerfectionnementViewModel) {
       if (confirm("Vous voulez supprime ?")) {
      this.showLoadingPerfectionnement = true;
      this.cvServ
        .DeletePerfectionnement(this.UtilisateurId, perc.graphId)
        .subscribe(sub => {
          this.LoadUserData();
        });
    }
  }

  SavePerfectionement(model: PerfectionnementViewModel) {
    if (
      model.annee !== undefined &&
      model.description !== undefined &&
      model.graphId === undefined
    ) {
      this.showLoadingPerfectionnement = true;
      this.cvServ
        .AddPerfectionnement(model, this.UtilisateurId)
        .subscribe((obs: PerfectionnementViewModel) => {
          this.LoadUserData();
        }, (error)=> this.Error(error));
    }
  }

  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingPerfectionnement) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
