import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ValidatorService } from "../../validator.services";
import { LoginModel } from "../../Models/Login-Model";
import { AuthenticationService } from "../../Services/authentication.service";
import { Credential } from "../../Models/Creadation-model";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
  constructor(
    private route: Router,
    private validator: ValidatorService,
    private auth: AuthenticationService
  ) {}
  Model: LoginModel = new LoginModel();
  MsgError: string="";
  ngOnInit() {}
  IsValid(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidateEmail(value, errorRegex);
  }
  IsValidPassword(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidadePassword(value, errorRegex);
  }
  Login(): void {
    this.auth.Login(this.Model).subscribe(
      (x: Credential) => {
        if (x.authenticated) {
          this.MsgError = "";
          localStorage.setItem("token", x.token);
          localStorage.setItem("utilisateurId", x.utilisateurId);
          localStorage.setItem("userName", x.userName);
          localStorage.setItem("isAdministrateur", x.isAdministrateur.toString());
          localStorage.setItem("isApprobateur", x.isApprobateur.toString());
          localStorage.setItem("isConseiller", x.isConseiller.toString());
          this.route.navigate(['home']);
        } else {
          localStorage.removeItem("token");
          localStorage.removeItem("utilisateurId");
          this.MsgError = "User or Password is wrong!!";
        }
      },
      (error: any) => {}
    );
  }
}
