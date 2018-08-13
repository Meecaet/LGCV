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
  opt: LangueViewModel;
  niveauLangue = new NiveauLangue();
  LangueControl = new FormControl();
  @Input()
  UtilisateurId: string;
  showLoadingLangue: boolean = false;
  constructor(
    private cvService: CVService,
    private errorService: ErrorService
  ) {
    this.niveauLangue = new NiveauLangue();
    this.languesAutoComplete = new Array<LangueViewModel>();
    this.langues = new Array<LangueViewModel>();

    this.opt = new LangueViewModel();
    this.opt.LangueControl = new FormControl();
  }

  ngOnInit() {
    this.showLoadingLangue = true;
    this.DataLoad();
  }
  AddLangue(): void {
    let lan = new LangueViewModel();
    lan.LangueControl = new FormControl();
    this.langues.push(lan);
  }
  removeLange(ele: LangueViewModel) {
    this.cvService.DeleteLangue(this.UtilisateurId, ele.graphId).subscribe(
      obs => {
        this.DataLoad();
      },
      error => this.Error(error)
    );
  }
  Error(error: HttpErrorResponse) {
    this.errorService.ErrorHandle(error.status);
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
    this.showLoadingLangue = true;
    this.cvService.UtilizaterLangue(this.UtilisateurId).subscribe(
      (data: Array<LangueViewModel>) => {
        var temData = data.filter((x: LangueViewModel) => {
          if (x.editionObjecViewModels.length < 1) {
            return x;
          } else if (
            !x.editionObjecViewModels.some(x => {
              return x.etat == "Modifie" && x.type == "Enlever";
            })
          ) {
            return x;
          }
        });
        for (let index = 0; index < temData.length; index++) {
          this.langues.push(this.SetValue(temData[index]));
        }
        this.showLoadingLangue = false;
      },
      error => this.Error(error)
    );
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
  SaveFormation(value, whoCalls, index: number, args?: any) {
    debugger;
    switch (whoCalls) {
      case "niveauLu":
        this.langues[index].niveauLu = value;
        break;
      case "niveauEcrit":
        this.langues[index].niveauEcrit = value;
        break;
      case "niveauParle":
        this.langues[index].niveauParle = value;
        break;
      case "graphId":
        if (
          !this.langues.filter(x => {
            return x.graphId == value;
          })
        ) {
          this.langues[index].graphId = value;
        } else {
          this.langues[index].graphId =   null;
          value = null;
          args = null;
        }
        break;
    }
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingLangue) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
