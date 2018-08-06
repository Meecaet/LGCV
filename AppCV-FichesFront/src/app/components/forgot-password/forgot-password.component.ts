import { Component, OnInit } from "@angular/core";
import { ValidatorService } from "../../validator.services";
import { AuthenticationService } from "../../Services/authentication.service";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-forgot-password",
  templateUrl: "./forgot-password.component.html",
  styleUrls: ["./forgot-password.component.css"]
})
export class ForgotPasswordComponent implements OnInit {
  Email: string;
  IsSuccess?: boolean = null;
  showLoadingForgotPassword: boolean = false;

  constructor(
    private validator: ValidatorService,
    private auth: AuthenticationService,
    private err: ErrorService
  ) {}

  ngOnInit() {}

  IsValid(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidateEmail(value, errorRegex);
  }

  ForgotPassword() {
    this.showLoadingForgotPassword = true;
     this.auth.ForgotPassword(this.Email).subscribe(
       (data: any) => {
         this.showLoadingForgotPassword = false;
         this.IsSuccess = true;
         this.Email = "";
       },
       erro => {
         this.IsSuccess = false;
         this.showLoadingForgotPassword = false;
       }
     );
  }

  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingForgotPassword) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
