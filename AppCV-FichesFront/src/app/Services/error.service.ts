import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({
  providedIn: "root"
})
export class ErrorService {
  constructor(private route: Router) { }
  ErrorHandle(errorStatus: number): void {
    debugger;
    switch (errorStatus) {
      case 401:
        sessionStorage.removeItem('token');
        this.route.navigate(['/account/login'])
        break;
      case 403:
        this.route.navigate(['/accessdenied']);
        break;
      case 404:
      sessionStorage.removeItem('token');
      this.route.navigate(['/account/login'])
        break;

      default:
        break;
    }
  }
}
