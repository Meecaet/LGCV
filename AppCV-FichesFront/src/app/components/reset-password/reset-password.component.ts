import { OnInit, Component } from "../../../../node_modules/@angular/core";
import { Router } from "../../../../node_modules/@angular/router";
import { ValidatorService } from "../../validator.services";
import { AuthenticationService } from "../../Services/authentication.service";
import { ResetPasswordModel } from "../../Models/ResetPassword-Model";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-reset-password",
  templateUrl: "./reset-password.component.html",
  styleUrls: ["./reset-password.component.css"]
})
export class ResetPasswordComponent implements OnInit {
  showLoadingResetPassword: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private validator: ValidatorService,
    private auth: AuthenticationService
  ) {
    debugger;

    this.route.params.subscribe(params => {
      debugger;

      this.model.code = params["code"];
    });
  }

  model: ResetPasswordModel = new ResetPasswordModel();
  MsgError = "";
  IsSuccess?: boolean = null;

  ngOnInit() {}

  IsValid(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidateEmail(value, errorRegex);
  }
  IsValidPassword(value, errorEmpty, errorRegex) {
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidadePassword(value, errorRegex);
  }

  ResetPassword() {
    this.showLoadingResetPassword = true;
    this.auth.ResetPassword(this.model).subscribe(
      (data: any) => {
        this.showLoadingResetPassword = false;
        this.IsSuccess = true;
        this.model.code = "";
        this.model.confirmPassword = "";
        this.model.password = "";
        this.model.email = "";
      },
      erro => {
        this.showLoadingResetPassword = false;
        this.IsSuccess = false;
      }
    );
  }
}
