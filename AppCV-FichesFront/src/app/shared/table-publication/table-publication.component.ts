import { Component, OnInit, Input } from "@angular/core";
import { PublicationViewModel } from "../../Models/Publication-model";
import { CVService } from "../../Services/cv.service";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-table-publication",
  templateUrl: "./table-publication.component.html",
  styleUrls: ["./table-publication.component.css"]
})
export class TablePublicationComponent implements OnInit {
  @Input() UtilisateurId: string;
  showLoadingPublication: boolean = false;
  publications: Array<PublicationViewModel>;

  constructor(private CVServ: CVService, private errorService: ErrorService) {}

  ngOnInit() {
  this.showLoadingPublication = true;
    this.publications = new Array<PublicationViewModel>();
  }
  AddPublication(): void {
    this.publications.push(new PublicationViewModel());
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingPublication) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
