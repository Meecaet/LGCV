import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-cv-edit",
  templateUrl: "./cv-edit.component.html",
  styleUrls: ["./cv-edit.component.css"]
})
export class CvEditComponent implements OnInit {
  UtilisateurId: string;
  constructor(private route: ActivatedRoute) {}
  ngOnInit() {
    debugger
    this.route.params.subscribe(params => {
      this.UtilisateurId = params["id"];
    });
  }
}
