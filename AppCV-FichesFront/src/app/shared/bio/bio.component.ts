import { OnInit, Input, Component } from "@angular/core";
import { BioViewModel } from "../../Models/Bio-Model";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-bio",
  templateUrl: "./bio.component.html",
  styleUrls: ["./bio.component.css"]
})
export class BioComponent implements OnInit {
  bio: BioViewModel;
  @Input() UtilisateurId: string;
  showLoadingBio:boolean=true;


  constructor(
    private cvService: CVService,
    private errorService: ErrorService,
    private route: ActivatedRoute
  ) {
    this.bio = new BioViewModel();
  }

  ngOnInit() {
    this.UserDataLoad();
  }
  saveBio(model: BioViewModel): void {

    this.cvService.EditBio(this.UtilisateurId, model).subscribe(
      (data: BioViewModel) => {
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
    this.cvService.GetBio(this.UtilisateurId).subscribe(
      (data: BioViewModel) => {
        this.SetData(data);
        this.showLoadingBio=!this.showLoadingBio
      },
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
  classValidator(cssClass :string,optionCssClass):string{
    if(this.showLoadingBio){
      return cssClass;
    }else{
      return optionCssClass;
    }
  }

  SetData(data: BioViewModel) {
    if (data.editionObjecViewModels != null) {
      for (const key in data.editionObjecViewModels) {
      if(data.editionObjecViewModels[key]["etat"] == "Modifie" )
       {
        this.bio[data.editionObjecViewModels[key]["proprieteNom"]] = data.editionObjecViewModels[key]["proprieteValeur"];
        //Set color
        this.bio.bioHighlight[data.editionObjecViewModels[key]["proprieteNom"]]=data.editionObjecViewModels[key]["type"]
       }
      }
    }
    //
 if (this.bio.resumeExperience == null) {
      this.bio.resumeExperience = data.resumeExperience;
    }
    if (this.bio.nom == null) {
      this.bio.nom = data.nom;
    }
    if (this.bio.prenom == null) {
      this.bio.prenom = data.prenom;
    }
    if (this.bio.fonction == null) {
      this.bio.fonction = data.fonction;
    }
  }
}
