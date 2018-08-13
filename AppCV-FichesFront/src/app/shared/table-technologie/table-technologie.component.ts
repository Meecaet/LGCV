import { Component, OnInit, Input } from "@angular/core";
import { TechnologieViewModel } from "../../Models/Technologie-model";
import { CVService } from "../../Services/cv.service";
import { TechnologieByCategorieViewModel } from "../../Models/TechlogieByCategorie-model";
import { MatDialog } from "../../../../node_modules/@angular/material";
import { ModalTechnologieComponent } from "../modal-technologie/modal-technologie.component";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-table-technologie",
  templateUrl: "./table-technologie.component.html",
  styleUrls: ["./table-technologie.component.css"]
})
export class TableTechnologieComponent implements OnInit {
  techByCategorie: Array<TechnologieByCategorieViewModel>;
  technologies: Array<TechnologieViewModel>;
  @Input("UtilisateurId") UtilisateurId: string;
  showLoadingTecnologie: boolean = true;
  constructor(
    private cvServ: CVService,
    private dialog: MatDialog,
    private servError: ErrorService
  ) {
    this.techByCategorie = new Array<TechnologieByCategorieViewModel>();
    this.technologies = new Array<TechnologieViewModel>();
  }

  ngOnInit() {
    this.LoadData();
    this.LoadAllTechnologie();
  }
  LoadData() {
    this.showLoadingTecnologie = true;
    this.cvServ.UtilizateurTechnologie(this.UtilisateurId).subscribe(
      obs => {
        this.techByCategorie = obs;
        this.showLoadingTecnologie = false;
      },
      error => this.Error(error)
    );
  }
  LoadAllTechnologie() {
    this.cvServ.LoadTechnologie().subscribe(obs => {
      this.technologies = obs;
    });
  }
  Error(error: HttpErrorResponse) {
    debugger;
    this.servError.ErrorHandle(error.status);
  }
  addTechnologie(): void {
    this.dialog
      .open(ModalTechnologieComponent, {
        data: {
          UtilisateurId: this.UtilisateurId,
          TechnologieViewModel: this.technologies
        }
      })
      .beforeClose()
      .subscribe(obs => {
        this.LoadData();
      });
    this.techByCategorie.push(new TechnologieByCategorieViewModel());
  }
  removeTechonolie(ele: TechnologieViewModel): void {
    if (confirm("Vous voulez supprime ?")) {
      this.showLoadingTecnologie = true;
      this.cvServ.DeleteTechnologie(this.UtilisateurId, ele.graphId).subscribe(
        data => {
          this.LoadData();
        },
        error => this.Error(error)
      );
    }
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingTecnologie) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
  ShowDetails(
    isShow: TechnologieByCategorieViewModel,
    model: Array<TechnologieByCategorieViewModel>
  ) {
    if (isShow.hidden == undefined) {
      isShow.hidden = false;
    } else {
      isShow.hidden = !isShow.hidden;
    }
  }
  public GetTechnologies(tech:TechnologieByCategorieViewModel):Array<TechnologieViewModel>{

    return tech.dataByCategorie.filter(x=>{
      return   x.editionObjecViewModels.length < 1 || !x.editionObjecViewModels.some(x => {return x.etat == "Modifie" && x.type == "Enlever";})
     })
    }
}
