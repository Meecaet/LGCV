import { Component, OnInit, Input } from "@angular/core";
import { CVViewModel } from "../../Models/cvview-model";
import { DomaineDInterventionViewModel } from "../../Models/DomaineDIntervention-model";
import { CertificationViewModel } from "../../Models/Certification-model";
import { FormationAcademiqueViewModel } from "../../Models/FormationAcademique-model";
import { LangueViewModel } from "../../Models/Langue-model";
import { PerfectionnementViewModel } from "../../Models/Perfectionnement-model";
import { LangueService } from "../../Services/langue.service";
import { TechnologieViewModel } from "../../Models/Technologie-model";
import { MandatViewModel } from "../../Models/Mandat-model";
import { CVService } from "../../Services/cv.service";
import { BioViewModel } from "../../Models/Bio-Model";
import { ErrorService } from "../../Services/error.service";
import { HttpErrorResponse } from "../../../../node_modules/@angular/common/http";

@Component({
  selector: "app-cv-create",
  templateUrl: "./cv-create.component.html",
  styleUrls: ["./cv-create.component.css"],
  providers: [LangueService]
})
export class CvCreateComponent implements OnInit {
  hidden: boolean = true;
  bio: BioViewModel;
  cvModel: CVViewModel;
  certifications: Array<CertificationViewModel>;
  domains: Array<DomaineDInterventionViewModel>;
  formationAcademique: Array<FormationAcademiqueViewModel>;
  langues: Array<LangueViewModel>;
  languesAutoComplete: Array<LangueViewModel>;
  perfectionnement: Array<PerfectionnementViewModel>;
  technologies: Array<TechnologieViewModel>;
  mandats: Array<MandatViewModel>;
  constructor(
    private cvService: CVService,
    private errorService: ErrorService
  ) {
    this.bio = new BioViewModel();
    this.cvModel = new CVViewModel();
    this.certifications = new Array<CertificationViewModel>();
    this.domains = new Array<DomaineDInterventionViewModel>();
    this.formationAcademique = new Array<FormationAcademiqueViewModel>();
    this.langues = new Array<LangueViewModel>();
    this.perfectionnement = new Array<PerfectionnementViewModel>();
    this.technologies = new Array<TechnologieViewModel>();
    this.mandats = new Array<MandatViewModel>();
  }
  saveBio(model: BioViewModel): void {
    switch ("") {
      case this.IsDataValid(model.nom):
        return;
      case this.IsDataValid(model.prenom):
        return;
      case this.IsDataValid(model.biographie):
        return;
      case this.IsDataValid(model.GraphIdUtilisateur):
        this.cvService.CreateBio(model).subscribe(
          (data: BioViewModel) => {
            this.hidden = false;
          },
          (error: HttpErrorResponse) => {
            this.errorService.ErrorHandle(error.status);
          }
        );
        return;
      default:
        this.EditBio();
    }
  }
  IsDataValid(value: any): any {
    if (value == null) {
      return "";
    } else {
      return value.trim();
    }
  }

  EditBio() {}
  ngOnInit(): void {
    this.LoadLangue();
  }
  LoadLangue(): void {
    this.cvService.LoadLangue().subscribe(
      (data: Array<LangueViewModel>) => {
        this.languesAutoComplete = data;
      },
      (error: HttpErrorResponse) => {
        this.errorService.ErrorHandle(error.status);
      }
    );
  }
}
