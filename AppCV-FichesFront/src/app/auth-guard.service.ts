import { CanActivate } from "@angular/router";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root"
})
export class AuthGuardService implements CanActivate {
  canActivate() {
     if (localStorage.getItem("token") != null) {
      return true;
    } else {
      return false;
    }
  }
}
