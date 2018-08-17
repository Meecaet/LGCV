import { Component, OnInit, Input } from "@angular/core";
import { PublicationViewModel } from "../../Models/Publication-model";
import { CVService } from "../../Services/cv.service";
import { ErrorService } from "../../Services/error.service";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";

@Component({
  selector: "app-table-publication",
  templateUrl: "./table-publication.component.html",
  styleUrls: ["./table-publication.component.css"]
})
export class TablePublicationComponent implements OnInit {
  @Input() UtilisateurId: string;
  showLoadingPublication: boolean = false;
  publications: Array<PublicationViewModel>;

  constructor(private CVServ: CVService, private errorService: ErrorService) {}

  ngOnInit() {
   this.LoadUserData();
  }
  LoadUserData(){
    this.showLoadingPublication = true;
    this.publications = new Array<PublicationViewModel>();
    this.CVServ.UtilizateurPublication(this.UtilisateurId).subscribe(obs => {
      this.publications = obs.filter((x: PublicationViewModel) => {
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
      this.showLoadingPublication= false;
    }, (error)=> this.Error(error));
  }
  Error(error: HttpErrorResponse) {

    this.errorService.ErrorHandle(error.status);
  }
  AddPublication(): void {
    this.publications.push(new PublicationViewModel());
  }


  SavePublication(model: PublicationViewModel) {
    if (
      model.annee !== undefined &&
      model.description !== undefined &&
      model.graphId === undefined
    ) {
      this.showLoadingPublication = true;
      this.CVServ
        .AddPublication(model, this.UtilisateurId)
        .subscribe((obs: PublicationViewModel) => {
          this.LoadUserData();
        }, (error)=> this.Error(error));
    }
  }


  removePublication(
    model: PublicationViewModel
  ) {
    if (confirm("Vous voulez supprime ?")) {
      this.showLoadingPublication = true;
     this.CVServ.DeletePublication(this.UtilisateurId,model.graphId).subscribe(data=>{
       this.LoadUserData()
     })
    }
  }

  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingPublication) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
