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
    switch ("") {
      case this.IsDataValid(model.nom):
        return;
      case this.IsDataValid(model.prenom):
        return;
      case this.IsDataValid(model.resumeExperience):
        return;
      case this.IsDataValid(model.graphIdConseiller):
        this.cvService.CreateBio(model).subscribe(
          (data: BioViewModel) => {},
          (error: HttpErrorResponse) => {
            this.errorService.ErrorHandle(error.status);
          }
        );
      default:
    }
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
        this.bio = data;
      },
      (error: HttpErrorResponse) => {
        debugger;
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
}
