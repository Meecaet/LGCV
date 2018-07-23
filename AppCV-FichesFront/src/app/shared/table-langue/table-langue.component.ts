import { Component, OnInit, Input } from "@angular/core";
import { LangueViewModel } from "../../Models/Langue-model";
import { FormControl } from "@angular/forms";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-table-langue",
  templateUrl: "./table-langue.component.html",
  styleUrls: ["./table-langue.component.css"]
})
export class TableLangueComponent implements OnInit {
  langues: Array<LangueViewModel>;
  languesAutoComplete: Array<LangueViewModel>;
  @Input() UtilisateurId: string;

  constructor(
    private cvService: CVService,
    private errorService: ErrorService
  ) {
    this.langues = new  Array<LangueViewModel>();

  }

  ngOnInit() {

    this.UserDataLoad();

    this.DataLoad();
  }
  AddLangue(): void {
    let lan = new LangueViewModel();
    lan.LangueControl = new FormControl();
    this.langues.push(lan);
  }
  removeLange(ele: LangueViewModel) {
    const index = this.langues.findIndex(
      x =>
        x.niveauecrit == ele.niveauecrit &&
        x.niveaulu == ele.niveaulu &&
        x.niveauparle == ele.niveauparle &&
        x.nom == ele.nom
    );
    if (index >= 0) {
      this.langues.splice(index, 1);
    }
  }

  DataLoad() {
    this.cvService.LoadLangue().subscribe(
      (data: Array<LangueViewModel>) => {
        this.languesAutoComplete = data;
      },
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
  UserDataLoad() {
     this.cvService
      .UtilizaterLangue(this.UtilisateurId)
      .subscribe((data: Array<LangueViewModel>) => {
        this.langues = data;
      });
  }
}
