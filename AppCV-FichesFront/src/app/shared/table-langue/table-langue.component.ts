import { Component, OnInit, Input } from "@angular/core";
import { LangueViewModel } from "../../Models/Langue-model";
import { FormControl } from "@angular/forms";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";
import { NiveauLangue } from "../../Models/niveau-langue.enum";

@Component({
  selector: "app-table-langue",
  templateUrl: "./table-langue.component.html",
  styleUrls: ["./table-langue.component.css"]
})
export class TableLangueComponent implements OnInit {
  langues: Array<LangueViewModel>;
  languesAutoComplete: Array<LangueViewModel>;
  niveauLangue = new NiveauLangue;
  LangueControl = new FormControl();
  @Input() UtilisateurId: string;
  showLoadingLangue:boolean = false;
  constructor(
    private cvService: CVService,
    private errorService: ErrorService
  ) {
  this.  niveauLangue =   new NiveauLangue();
    this.languesAutoComplete = new  Array<LangueViewModel>();
    this.langues = new Array<LangueViewModel>();
  }

  ngOnInit() {
    this.showLoadingLangue =  true;
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
        x.niveauEcrit == ele.niveauEcrit &&
        x.niveauLu== ele.niveauLu &&
        x.niveauParle == ele.niveauParle &&
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
        this.UserDataLoad();

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
        for (let index = 0; index < data.length; index++) {
          this.langues.push(this.SetValue(data[index]));
        }
        this.showLoadingLangue =  false;
      });
  }
  SetValue(data: LangueViewModel): LangueViewModel {
    let returnValue = data;
    returnValue.nom;
    returnValue.LangueControl = new FormControl({
      graphId: returnValue.graphId,
      nom: returnValue.nom
    });

    return returnValue;
  }
  SetValueFromControl(formControl: any) {
    return formControl.value.nom;
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingLangue) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
