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
  certification: CertificationViewModel;
  highlight: string;
  @Input() UtilisateurId: string;
  showLoadingCertification: boolean = true;
  constructor(private serv: CVService, private servError: ErrorService) {}
  ngOnInit() {
    this.LoadUserData();
  }
  LoadUserData() {
    this.showLoadingCertification = true;
    this.serv
      .LoadCertification(this.UtilisateurId)
      .subscribe((obs: Array<CertificationViewModel>) => {
        this.certifications = new Array<CertificationViewModel>();
        for (const data of obs) {
          this.certifications.push(this.SetData(data));
        }

        this.showLoadingCertification = false;
      }, this.Error);
  }

  AddCertifications(): void {
    this.certifications.push(new CertificationViewModel());
  }

  removeCertification(
    ele: any,
    button: any,
    certification: CertificationViewModel
  ) {
    if (confirm("Vous voulez supprime ?")) {
      certification.certificationHighlight.annee = "highlighterror";
      certification.certificationHighlight.description = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(certification);
    }
  }
  SaveCertification(model: CertificationViewModel) {

    if (
      model.annee != null &&
      model.annee.toString().trim() != "" &&
      model.description != null &&
      model.description.toString().trim() != ""
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

  private SetData(data: CertificationViewModel) {
    this.certification = new CertificationViewModel();

    if (data.editionObjecViewModels != null) {
      for (const key in data.editionObjecViewModels) {
        if (data.editionObjecViewModels[key]["etat"] == "Modifie") {
          this.certification[data.editionObjecViewModels[key]["proprieteNom"]] =
            data.editionObjecViewModels[key]["proprieteValeur"];
          //Set color
          this.certification.certificationHighlight[
            data.editionObjecViewModels[key]["proprieteNom"]
          ] =
            data.editionObjecViewModels[key]["type"];
        }
      }
    }
    if (this.certification.annee == null) {
      this.certification.annee = data.annee;
    }
    if (this.certification.description == null) {
      this.certification.description = data.description;
    }
    this.certification.graphId = data.graphId;
    this.certification.graphIdGenre = data.graphIdGenre;

    if (
      this.certification.graphId == null &&
      this.certification.graphIdGenre == null
    ) {
      this.certification = new CertificationViewModel();
    }

    return this.certification;
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingCertification) {
      return cssClass;
    } else {
      return optionCssClass;
    }

  }
}
