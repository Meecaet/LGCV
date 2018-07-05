import { Component, OnInit } from "@angular/core";
import { ValidatorService } from "../../validator.services";

@Component({
  selector: "app-register",
  templateUrl: "./register.component.html",
  styleUrls: ["./register.component.css"]
})
export class RegisterComponent implements OnInit {
  Email: string = null;
  PassWord: string = null;
  ConfirmPassword: string = null;
  Prenom: string = null;
  Nom: string = null;
  constructor(private validator: ValidatorService) {}

  ngOnInit() {}
  IsValid(value, errorEmpty, errorRegex) {
    debugger;
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidateEmail(value, errorRegex);
  }
  IsValidPassword(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidadePassword(value, errorRegex);
  }

  DisanabledButton(
    IsValidEmail,
    IsValidEmailRegex,
    IsValidPrenom,
    IsValidNom,
    IsValidPassWord,
    IsValidPassWordRegex
  ): boolean {
    debugger
    if (
      IsValidEmail.hidden == true &&
      IsValidEmailRegex.hidden == true &&
      IsValidPrenom.hidden == true &&
      IsValidNom.hidden == true &&
      IsValidPassWord.hidden == true &&
      IsValidPassWordRegex.hidden == true &&
      this.PassWord == this.ConfirmPassword
    ) {
      return false
    } else {
      return true
    }
  }
}
