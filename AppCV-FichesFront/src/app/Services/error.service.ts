import { Injectable } from "@angular/core";
import { Router } from "@angular/router";

@Injectable({
  providedIn: "root"
})
export class ErrorService {
  constructor(private route: Router) { }
  ErrorHandle(errorStatus: number): void {
        switch (errorStatus) {
      case 401:
        sessionStorage.removeItem('token');

        this.route.navigate(['/account/login'])
        break;
      case 403:
        sessionStorage.removeItem('token');
        this.route.navigate(['/accessdenied']);
        break;
      case 404:
      sessionStorage.removeItem('token');
      this.route.navigate(['/account/login'])
        break;

      default:
        sessionStorage.removeItem('token');
        break;
    }
  }
}
