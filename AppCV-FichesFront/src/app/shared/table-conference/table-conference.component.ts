import { Component, OnInit, Input } from "@angular/core";
import { ConferenceViewModel } from "../../Models/Conference-model";

@Component({
  selector: "app-table-conference",
  templateUrl: "./table-conference.component.html",
  styleUrls: ["./table-conference.component.css"]
})
export class TableConferenceComponent implements OnInit {
  @Input() UtilisateurId: string;
  showLoadingConference: boolean = false;
  conferenceses: Array<ConferenceViewModel>;
  constructor() {}

  ngOnInit() {
    this.conferenceses = new Array<ConferenceViewModel>();
    this.showLoadingConference = true;
  }
  AddConference(): void {}
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingConference) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
