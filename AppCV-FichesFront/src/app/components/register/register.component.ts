import { Component, OnInit } from "@angular/core";
import { ValidatorService } from "../../validator.services";
import { AdminService } from "../../Services/admin.service";
import { RegisterViewModel } from "../../Models/Register-model";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";
import { Router } from '@angular/router';
import { Location } from "@angular/common";

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

  constructor(
    private validator: ValidatorService,
    private adminService: AdminService,
    private router: Router,
    private location: Location
  ) {}

  model: RegisterViewModel = new RegisterViewModel();

  ngOnInit() {}

  IsValid(value, errorEmpty, errorRegex) {
    errorEmpty.isTouched = true;
    this.validator.ValidateEmpty(value, errorEmpty);
    if (errorRegex != null) {
      errorRegex.isTouched = true;
      this.validator.ValidateEmail(value, errorRegex);
    }
  }
  IsValidPassword(value, errorEmpty, errorRegex) {
    errorEmpty.isTouched = true;
    this.validator.ValidateEmpty(value, errorEmpty);
    errorRegex.isTouched = true;
    this.validator.ValidadePassword(value, errorRegex);
  }

  Registre(): void {
    this.adminService.Register(this.model).subscribe(
      (data) => {
        this.location.replaceState('/');
        this.router.navigate(['account/login']);
      },
      (error: HttpErrorResponse) => {
        // TODO

      }
    );
  }
  DisanabledButton(
    IsValidEmail,
    IsValidEmailRegex,
    IsValidPrenom,
    IsValidNom,
    IsValidPassWord,
    IsValidPassWordRegex
  ): boolean {

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
