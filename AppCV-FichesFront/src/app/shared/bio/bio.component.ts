import { Component, OnInit, Input } from "@angular/core";
import { BioViewModel } from "../../Models/Bio-Model";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";
import { ErrorService } from "../../Services/error.service";
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: "app-bio",
  templateUrl: "./bio.component.html",
  styleUrls: ["./bio.component.css"]
})
export class BioComponent implements OnInit {
  bio: BioViewModel;
  UtilisateurId: string;

  constructor(
    private cvService: CVService,
    private errorService: ErrorService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.UtilisateurId = params['id'];
      this.UserDataLoad();
    });
  }

  UserDataLoad() {
    this.cvService
      .GetBio(this.UtilisateurId)
      .subscribe(
        (data: BioViewModel) => {
          this.bio = data;
          console.log(this.bio);
        },
        (error: HttpErrorResponse) => {
          this.errorService.ErrorHandle(error.status);
        }
      );
  }
}
