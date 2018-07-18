import { Injectable } from "@angular/core";
import { Router } from "../../../node_modules/@angular/router";

@Injectable({
  providedIn: "root"
})
export class ErrorService {
  constructor(private route: Router) {}
  ErrorHandle(errorStatus: number): void {

    switch (errorStatus) {
      case 401:
      sessionStorage.removeItem('token');
      this.route.navigate(['/account/login'])
        break;

      default:
        break;
    }
  }
}