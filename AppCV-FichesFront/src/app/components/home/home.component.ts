import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { Location } from "@angular/common";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(
    private router: Router,
    private location: Location
  ) {

    if (localStorage.getItem("token") == null) {
      this.location.replaceState('/');
      this.router.navigate(['account/login']);
    }
  }

  ngOnInit() {
  }

}
