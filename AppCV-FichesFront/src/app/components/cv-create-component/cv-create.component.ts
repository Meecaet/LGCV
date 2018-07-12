import { COMMA, ENTER } from "@angular/cdk/keycodes";
import { Component, OnInit, Input } from "@angular/core";
import { MatChipInputEvent } from "@angular/material";
import { CVViewModel } from "../../Models/cvview-model";
import { DomaineDInterventionViewModel } from "../../Models/DomaineDIntervention-model";
import { CertificationViewModel } from "../../Models/Certification-model";
import { FormationAcademiqueViewModel } from "../../Models/FormationAcademique-model";
import { LangueViewModel } from "../../Models/Langue-model";
import { PerfectionnementViewModel } from "../../Models/Perfectionnement-model";


import {FormBuilder, FormControl} from '@angular/forms';



@Component({
  selector: "app-cv-create",
  templateUrl: "./cv-create.component.html",
  styleUrls: ["./cv-create.component.css"]
})
export class CvCreateComponent implements OnInit {

  myControl = new FormControl();
  options: string[] = ['One', 'Two', 'Three'];


  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  cvModel: CVViewModel;
  certifications: Array<CertificationViewModel>;
  domains: Array<DomaineDInterventionViewModel>;
  formationAcademique: Array<FormationAcademiqueViewModel>;
  langues: Array<LangueViewModel>;
  languesAutoComplete: Array<LangueViewModel>;
  perfectionnement: Array<PerfectionnementViewModel>;

  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;

  constructor(private fb: FormBuilder) {
    this.cvModel = new CVViewModel();
    this.certifications = new Array<CertificationViewModel>();
    this.domains = new Array<DomaineDInterventionViewModel>();
    this.formationAcademique = new Array<FormationAcademiqueViewModel>();
    this.langues = new Array<LangueViewModel>();
    this.languesAutoComplete = new Array<LangueViewModel>()
    this.languesAutoComplete = <Array<LangueViewModel>>[{Nom:"teste"}];
    this.perfectionnement = new Array<PerfectionnementViewModel>();

  }
  cert: Array<any> = null;
  AddCertifications(): void {
    this.certifications.push(new CertificationViewModel());
  }
  AddPerfectionnement(): void {
    this.perfectionnement.push(new PerfectionnementViewModel());
  }
  AddFormationAcademique(): void {
    this.formationAcademique.push(new FormationAcademiqueViewModel());
  }
  AddLangue(): void {
    this.langues.push(new LangueViewModel());
  }
  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || "").trim()) {
      let d = new DomaineDInterventionViewModel();
      d.Description = value.trim();
      this.domains.push(d);
    }

    // Reset the input value
    if (input) {
      input.value = "";
    }
  }
  remove(domain: DomaineDInterventionViewModel): void {
    const index = this.domains.findIndex(
      x => x.Description == domain.Description
    );
    if (index >= 0) {
      this.domains.splice(index, 1);
    }
  }
  removeCertification(ele: CertificationViewModel) {
    const index = this.certifications.findIndex(
      x => x.Description == ele.Description
    );
    if (index >= 0) {
      this.certifications.splice(index, 1);
    }
  }
  removeFormationAcademique(ele: FormationAcademiqueViewModel) {
    const index = this.formationAcademique.findIndex(
      x =>
        x.Diplome == ele.Diplome &&
        x.Annee == ele.Annee &&
        x.Etablissement == ele.Etablissement
    );
    if (index >= 0) {
      this.formationAcademique.splice(index, 1);
    }
  }
  removePerfectionnement(ele: PerfectionnementViewModel) {
    const index = this.perfectionnement.findIndex(
      x => x.Annee == ele.Annee && x.Description == ele.Description
    );
    if (index >= 0) {
      this.perfectionnement.splice(index, 1);
    }
  }
  removeLange(ele: LangueViewModel) {
    const index = this.langues.findIndex(
      x =>
        x.NiveauEcrit == ele.NiveauEcrit &&
        x.NiveauLu == ele.NiveauLu &&
        x.NiveauParle == ele.NiveauParle &&
        x.Nom == ele.Nom
    );
    if (index >= 0) {
      this.langues.splice(index, 1);
    }
  }
  ngOnInit(): void {

  }








}
