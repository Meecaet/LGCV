import { COMMA, ENTER } from "@angular/cdk/keycodes";
import { Component, OnInit, Input } from "@angular/core";
import { MatChipInputEvent } from "@angular/material";
import { CVViewModel } from "../../Models/cvview-model";
import { DomaineDInterventionViewModel } from "../../Models/DomaineDIntervention-model";
import { CertificationViewModel } from "../../Models/Certification-model";
import { FormationAcademiqueViewModel } from "../../Models/FormationAcademique-model";
import { LangueViewModel } from "../../Models/Langue-model";
import { PerfectionnementViewModel } from "../../Models/Perfectionnement-model";


import {FormBuilder, FormGroup} from '@angular/forms';
import {Observable} from 'rxjs';
import {startWith, map} from 'rxjs/operators';



export interface StateGroup {
  letter: string;
  names: string[];
}
export const _filter = (opt: string[], value: string): string[] => {
  const filterValue = value.toLowerCase();

  return opt.filter(item => item.toLowerCase().indexOf(filterValue) === 0);
};



@Component({
  selector: "app-cv-create",
  templateUrl: "./cv-create.component.html",
  styleUrls: ["./cv-create.component.css"]
})
export class CvCreateComponent implements OnInit {




  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  cvModel: CVViewModel;
  certifications: Array<CertificationViewModel>;
  domains: Array<DomaineDInterventionViewModel>;
  formationAcademique: Array<FormationAcademiqueViewModel>;
  langues: Array<LangueViewModel>;
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
    this.perfectionnement = new Array<PerfectionnementViewModel>();


    this.stateGroupOptions = this.stateForm.get('stateGroup')!.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filterGroup(value))
      );
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








  /************************************************** */


  stateForm: FormGroup = this.fb.group({
    stateGroup: '',
  });

  stateGroups: StateGroup[] = [{
    letter: 'A',
    names: ['Alabama', 'Alaska', 'Arizona', 'Arkansas']
  }, {
    letter: 'C',
    names: ['California', 'Colorado', 'Connecticut']
  }, {
    letter: 'D',
    names: ['Delaware']
  }, {
    letter: 'F',
    names: ['Florida']
  }, {
    letter: 'G',
    names: ['Georgia']
  }, {
    letter: 'H',
    names: ['Hawaii']
  }, {
    letter: 'I',
    names: ['Idaho', 'Illinois', 'Indiana', 'Iowa']
  }, {
    letter: 'K',
    names: ['Kansas', 'Kentucky']
  }, {
    letter: 'L',
    names: ['Louisiana']
  }, {
    letter: 'M',
    names: ['Maine', 'Maryland', 'Massachusetts', 'Michigan',
      'Minnesota', 'Mississippi', 'Missouri', 'Montana']
  }, {
    letter: 'N',
    names: ['Nebraska', 'Nevada', 'New Hampshire', 'New Jersey',
      'New Mexico', 'New York', 'North Carolina', 'North Dakota']
  }, {
    letter: 'O',
    names: ['Ohio', 'Oklahoma', 'Oregon']
  }, {
    letter: 'P',
    names: ['Pennsylvania']
  }, {
    letter: 'R',
    names: ['Rhode Island']
  }, {
    letter: 'S',
    names: ['South Carolina', 'South Dakota']
  }, {
    letter: 'T',
    names: ['Tennessee', 'Texas']
  }, {
    letter: 'U',
    names: ['Utah']
  }, {
    letter: 'V',
    names: ['Vermont', 'Virginia']
  }, {
    letter: 'W',
    names: ['Washington', 'West Virginia', 'Wisconsin', 'Wyoming']
  }];

  stateGroupOptions: Observable<StateGroup[]>;



  private _filterGroup(value: string): StateGroup[] {
    if (value) {
      return this.stateGroups
        .map(group => ({letter: group.letter, names: _filter(group.names, value)}))
        .filter(group => group.names.length > 0);
    }

    return this.stateGroups;
  }

/****************************************************** */
}
