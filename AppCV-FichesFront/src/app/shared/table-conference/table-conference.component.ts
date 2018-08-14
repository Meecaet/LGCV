import { Component, OnInit, Input } from "@angular/core";
import { ConferenceViewModel } from "../../Models/Conference-model";
import { CVService } from "../../Services/cv.service";
import { ErrorService } from "../../Services/error.service";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";

@Component({
  selector: "app-table-conference",
  templateUrl: "./table-conference.component.html",
  styleUrls: ["./table-conference.component.css"]
})
export class TableConferenceComponent implements OnInit {
  @Input() UtilisateurId: string;
  showLoadingConference: boolean = false;
  conferenceses: Array<ConferenceViewModel>;
  constructor(private cvServ: CVService,private err: ErrorService,private servError : ErrorService) {}

  ngOnInit() {
    this.conferenceses = new Array<ConferenceViewModel>();
     this. LoadUserData();
  }
  AddConference(): void {
    this.conferenceses.push(new ConferenceViewModel());
  }
  SaveConferences(model: ConferenceViewModel) {
    if (
      model.annee !== undefined &&
      model.description  !== undefined &&
      model.graphId     === undefined
    ) {
      this.showLoadingConference = true;
      this.cvServ
        .AddConference(model, this.UtilisateurId)
        .subscribe((obs: ConferenceViewModel) => {
          this.LoadUserData();
        }, (error:any)=> this.err.ErrorHandle(error.status));
    }
  }
  LoadUserData(){
    this.showLoadingConference = true;
    this.cvServ.UtilizateurConference(this.UtilisateurId).subscribe(obs => {
      this.conferenceses = obs.filter((x: ConferenceViewModel) => {
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
      this.showLoadingConference = false;
    }, (error)=> this.Error(error));

  }




  Error(error: HttpErrorResponse) {
    debugger;
    this.servError.ErrorHandle(error.status);
  }

  removeConferences(
    model: ConferenceViewModel
  ) {
    if (confirm("Vous voulez supprime ?")) {
      this.showLoadingConference = true;
     this.cvServ.DeleteConference(this.UtilisateurId,model.graphId).subscribe(data=>{
       this.LoadUserData()
     })
    }
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingConference) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
