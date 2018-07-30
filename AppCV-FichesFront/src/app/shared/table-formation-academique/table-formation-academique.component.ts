import { Component, OnInit, Input } from "@angular/core";
import { FormationAcademiqueViewModel } from "../../Models/FormationAcademique-model";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";
import { NiveauAcademique } from "../../Models/niveau-academique.enum";

@Component({
  selector: "app-table-formation-academique",
  templateUrl: "./table-formation-academique.component.html",
  styleUrls: ["./table-formation-academique.component.css"]
})
export class TableFormationAcademiqueComponent implements OnInit {
  formationAcademique: Array<FormationAcademiqueViewModel>;
  niveauAcademique  : NiveauAcademique= new  NiveauAcademique();
  @Input() UtilisateurId: string;
  showLoadingFormationAcademique: boolean = true;
  constructor(private serv: CVService, private servError: ErrorService) {

    this.formationAcademique = new Array<FormationAcademiqueViewModel>();
  }

  ngOnInit() {
   this.LoadUserData();
  }
  AddFormationAcademique(): void {
    this.formationAcademique.push(new FormationAcademiqueViewModel());
  }
  LoadUserData() {
    this.showLoadingFormationAcademique = true;
    this.serv
      .LoadFormationAcademique(this.UtilisateurId)
      .subscribe((obs: Array<FormationAcademiqueViewModel>) => {
        this.formationAcademique = obs;
        this.showLoadingFormationAcademique = false;
      }, this.Error);
  }
  Error(error: HttpErrorResponse) {
    debugger;
    this.servError.ErrorHandle(error.status);
  }
  removeFormationAcademique(
    ele: any,
    button: any,
    form: FormationAcademiqueViewModel
  ) {
    if (confirm("Vous voulez supprime ?")) {
      form.highlight = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(form);
    }
  }
  deleteFromDatabase(form: FormationAcademiqueViewModel) {
    alert("to implement");
  }

  SaveFormation(model: FormationAcademiqueViewModel) {
    if (
      model.annee !== undefined &&
      model.diplome !== undefined &&
      model.etablissement !== undefined &&
      model.niveau !== undefined
    ) {
      this.showLoadingFormationAcademique = true;
      this.serv
        .FormationAcademique(model, this.UtilisateurId)
        .subscribe((obs: FormationAcademiqueViewModel) => {
          this.LoadUserData();
        }, this.Error);
    }
  }

  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingFormationAcademique) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
