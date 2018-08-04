import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { LoginModel } from "../Models/Login-Model";
import { Credential } from "../Models/Creadation-model";
import { ResetPasswordModel } from "../Models/ResetPassword-Model";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: "root"
})
export class AuthenticationService {

  apiBaseUrl: string ="";

  constructor(private _http: HttpClient) {

    this.apiBaseUrl = environment.apiUrl;
  }
  Login(model:LoginModel): Observable<Credential> {

    return this._http.post<Credential>(this.apiBaseUrl + "AccountApi/DoLogin",{Email:model.email,Password:model.password,RemeberMe:model.rememberMe})
  }

 ForgotPassword(email: string) {
    return this._http.post(this.apiBaseUrl + "AccountApi/ForgotPassword", {Email: email});
  }

 ResetPassword(model: ResetPasswordModel) {
    return this._http.post(this.apiBaseUrl + "AccountApi/ResetPassword", model);
  }
}
