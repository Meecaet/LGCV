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
    debugger;
    this.cvService.EditBio(this.UtilisateurId, model).subscribe(
      (data: BioViewModel) => {
        debugger;
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
      },
      (error: HttpErrorResponse) => {
        debugger;
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
  SetData(data: BioViewModel) {
    if (data.editionObjecViewModels != null) {
      for (const key in data.editionObjecViewModels) {
        debugger;
      this.bio[data.editionObjecViewModels[key]["proprieteNom"]] = data.editionObjecViewModels[key]["proprieteValeur"];
      //Set color
      this.bio.bioHighlight[data.editionObjecViewModels[key]["proprieteNom"]]=data.editionObjecViewModels[key]["type"]
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
