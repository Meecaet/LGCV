import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Location } from "@angular/common";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"]
})
export class HomeComponent implements OnInit {

  isAdministrateur: boolean = false;
  isApprobateur: boolean = false;
  isConseiller: boolean = false;

  utilisateurId:string=localStorage.getItem("utilisateurId");
  constructor(
    private router: Router,
    private location: Location,
    private serv: CVService,
    private errorServ: ErrorService
  ) {
    this.serv.IsTokenValid().subscribe(
      (isValid: boolean) => {
        if (!isValid) {
          localStorage.removeItem("token");
          this.router.navigate(["account/login"]);
        }
      },
      error => {
        this.Error(error);
      }
    );
  }

  Error(error: HttpErrorResponse) {
    this.errorServ.ErrorHandle(error.status);
  }
  ngOnInit() {
    if (localStorage.getItem("isAdministrateur")== "true") {this.isAdministrateur=true;}
    if (localStorage.getItem("isApprobateur")== "true") {this.isApprobateur=true;}
    if (localStorage.getItem("isConseiller")== "true") {this.isConseiller=true;}
  }
}
