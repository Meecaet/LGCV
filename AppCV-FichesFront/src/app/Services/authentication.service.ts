import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { LoginModel } from "../Models/Login-Model";
import { Credential } from "../Models/Creadation-model";
import { ResetPasswordModel } from "../Models/ResetPassword-Model";

@Injectable({
  providedIn: "root"
})
export class AuthenticationService {
  constructor(private _http: HttpClient) {}
  Login(model:LoginModel): Observable<Credential> {

    return this._http.post<Credential>("https://localhost:44344/api/AccountApi/DoLogin",{Email:model.email,Password:model.password,RemeberMe:model.rememberMe})
  }

 ForgotPassword(email: string) {
    return this._http.post("https://localhost:44344/api/AccountApi/ForgotPassword", {Email: email});
  }

 ResetPassword(model: ResetPasswordModel) {
    return this._http.post("https://localhost:44344/api/AccountApi/ResetPassword", model);
  }
}
