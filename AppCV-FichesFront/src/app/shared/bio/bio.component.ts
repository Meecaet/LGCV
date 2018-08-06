import { OnInit, Input, Component } from "@angular/core";
import { BioViewModel } from "../../Models/Bio-Model";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";
import { ActivatedRoute } from "@angular/router";
import { MatDialog } from "../../../../node_modules/@angular/material";
import { ModalOldInformationComponent } from "../modal-old-information/modal-old-information.component";


@Component({
  selector: "app-bio",
  templateUrl: "./bio.component.html",
  styleUrls: ["./bio.component.css"]
})
export class BioComponent implements OnInit {
  bio: BioViewModel;
  originalBio: BioViewModel;
  @Input() UtilisateurId: string;
  showLoadingBio: boolean = true;

  constructor(
    private cvService: CVService,
    private errorService: ErrorService,
    private route: ActivatedRoute,
    public dialog: MatDialog
  ) {
    this.bio = new BioViewModel();
    this.originalBio = new BioViewModel();
  }

  ngOnInit() {
    this.UserDataLoad();
  }
  saveBio(model: BioViewModel): void {
    this.cvService.EditBio(this.UtilisateurId, model).subscribe(
      (data: BioViewModel) => {
        this.UserDataLoad();
      },
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
  IsDataValid(value: any): any {
    if (value == null) {
      return "";
    } else {
      return value.trim();
    }
  }
  UserDataLoad() {
    this.showLoadingBio = true;
    this.cvService.GetBio(this.UtilisateurId).subscribe(
      (data: BioViewModel) => {

        this.originalBio = data;

        this.SetData(data);
        this.showLoadingBio = !this.showLoadingBio;
      },
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingBio) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
  IsShowInformations(fieldName): boolean {
    if (this.originalBio.editionObjecViewModels.length > 0) {

      var teste = this.originalBio.editionObjecViewModels.some(
        x => x.proprieteNom === fieldName
      );
      return teste;
    } else {
      return false;
    }
  }
  onChange(data:any){
     this.bio.fonction = data;
     this.saveBio(this.bio)
  }
  SetData(data: BioViewModel) {
    if (data.editionObjecViewModels != null) {
      debugger
      for (const key in data.editionObjecViewModels) {
        if (data.editionObjecViewModels[key]["etat"] == "Modifie") {

          if(data.editionObjecViewModels[key]["type"]=="ChangementRelation"){
            this.bio[data.editionObjecViewModels[key]["proprieteNom"]] =
            data.editionObjecViewModels[key]["editionId"];
          //Set color
          this.bio.bioHighlight[data.editionObjecViewModels[key]["proprieteNom"]          ] =
            data.editionObjecViewModels[key]["type"];

          }else{
          this.bio[data.editionObjecViewModels[key]["proprieteNom"]] =
            data.editionObjecViewModels[key]["proprieteValeur"];
          //Set color
          this.bio.bioHighlight[data.editionObjecViewModels[key]["proprieteNom"]          ] =
            data.editionObjecViewModels[key]["type"];
          }
        }
      }
    }
    //
    if (
      this.bio.resumeExperience == null ||
      this.bio.resumeExperience == undefined
    ) {
      this.bio.resumeExperience = data.resumeExperience;
    }
    if (this.bio.nom == null || this.bio.nom == undefined) {
      this.bio.nom = data.nom;
    }
    if (this.bio.prenom == null || this.bio.prenom == undefined) {
      this.bio.prenom = data.prenom;
    }
    if (this.bio.fonction == null || this.bio.fonction == undefined) {
      this.bio.fonction = data.fonction;
    }
    this.bio.editionObjecViewModels = data.editionObjecViewModels;
  }

  /**********************************************************************************
   * *MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL*MODAL**
   * *********************************************************************************/

  openDialog(ModalMessage, OriginalChamp) {

    this.dialog.open(ModalOldInformationComponent, {
      data: {
        ModalMessage: ModalMessage,
        OriginalChamp: OriginalChamp
      }
    });
  }
}
