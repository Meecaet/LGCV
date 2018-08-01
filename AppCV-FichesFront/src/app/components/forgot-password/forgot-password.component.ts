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
    this.auth.ForgotPassword(this.Email).subscribe(
      (data: any) => {
        this.IsSuccess = true;
        this.Email = "";
      },
      erro => {
        this.IsSuccess = false;
      }
    );
  }
}
