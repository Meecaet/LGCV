import { Component, OnInit, Input } from "@angular/core";
import { CertificationViewModel } from "../../Models/Certification-model";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-table-certifications",
  templateUrl: "./table-certifications.component.html",
  styleUrls: ["./table-certifications.component.css"]
})
export class TableCertificationsComponent implements OnInit {
  certifications: Array<CertificationViewModel>;

  highlight: string;
  @Input() UtilisateurId: string;
  showLoadingCertification: boolean = true;
  constructor(private serv: CVService, private servError: ErrorService) {}
  ngOnInit() {
    this.LoadUserData();
  }
  LoadUserData() {
    this.certifications = new Array<CertificationViewModel>();
    this.showLoadingCertification = true;
    this.serv
      .LoadCertification(this.UtilisateurId)
      .subscribe((obs: Array<CertificationViewModel>) => {

        this.certifications = obs.filter((x: CertificationViewModel) => {
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
        this.showLoadingCertification = false;
      }, (error)=> this.Error(error));
  }

  AddCertifications(): void {
    this.certifications.push(new CertificationViewModel());
  }
  NoSave():void{

  }
  removeCertification(certification: CertificationViewModel) {
    if (confirm("Vous voulez supprime ?")) {
      this.showLoadingCertification = true;
      this.serv
        .DeleteCertification(this.UtilisateurId, certification.graphId)
        .subscribe(sub => {
          this.LoadUserData();
        });
    }
  }
  SaveCertification(model: CertificationViewModel) {
    if (
      model.annee != null &&
      model.annee.toString().trim() != "" &&
      model.description != null &&
      model.description.toString().trim() != "" &&
      model.graphId === undefined
    )
      this.serv
        .AddCertifications(model, this.UtilisateurId)
        .subscribe((obs: CertificationViewModel) => {
          this.LoadUserData();
        }, this.Error);
  }
  Error(error: HttpErrorResponse) {
    this.servError.ErrorHandle(error.status);
  }
  deleteFromDatabase(certification: CertificationViewModel) {
    alert("to implement");
  }

  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingCertification) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
