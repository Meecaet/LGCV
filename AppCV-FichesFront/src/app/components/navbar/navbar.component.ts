import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { Location } from "@angular/common";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  userName:string;

  constructor(
    private router: Router,
    private location: Location
  ) {}

  ngOnInit() {}

  isAuthenticated(): boolean {
    if (localStorage.getItem("token") == null) {
      this.userName = "";
      return false;
    } else {
      this.userName = localStorage.getItem("userName");
      return true;
    }
  }

  logOut(): void {
    localStorage.removeItem("token");
    localStorage.removeItem("utilisateurId");
    localStorage.removeItem("userName");
    this.userName = "";
    this.location.replaceState('/');
    this.router.navigate(['account/login']);
  }

}
