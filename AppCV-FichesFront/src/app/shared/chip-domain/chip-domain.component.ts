import { Component, OnInit, Input } from "@angular/core";
import { MatChipInputEvent } from "@angular/material";
import { DomaineDInterventionViewModel } from "../../Models/DomaineDIntervention-model";
import { ENTER, COMMA } from "@angular/cdk/keycodes";
import { CVService } from "../../Services/cv.service";
import { HttpErrorResponse } from "@angular/common/http";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-chip-domain",
  templateUrl: "./chip-domain.component.html",
  styleUrls: ["./chip-domain.component.css"]
})
export class ChipDomainComponent implements OnInit {
  @Input("UtilisateurId") UtilisateurId: string;
  domains: Array<DomaineDInterventionViewModel>;
  showLoadingDomain: boolean = true;

  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  constructor(private CVserv: CVService,private   servError :ErrorService) {
    this.domains = new Array<DomaineDInterventionViewModel>();
  }

  ngOnInit() {
    this.LoadUserData();
  }
  private LoadUserData() {
    this.CVserv.LoadDomain(this.UtilisateurId).subscribe(
      (data: Array<DomaineDInterventionViewModel>) => {
        this.domains = data;
        this.showLoadingDomain = false;
      },
       this.Error
    );
  }
  addDomain(event: MatChipInputEvent): void {

    const input = event.input;
    const value = event.value;
    // Add our fruit
    if ((value || "").trim()) {
      let domain = new DomaineDInterventionViewModel();
      domain.description = value.trim();
      this.showLoadingDomain = true;
      this.CVserv.AddDomain(this.UtilisateurId, domain).subscribe(x => {
        this.LoadUserData();
      }, this.Error);
    }

    // Reset the input value
    if (input) {
      input.value = "";
    }
  }
  Error(error: HttpErrorResponse){
    this.servError.ErrorHandle(error.status);
  }
  removeDomain(domain: DomaineDInterventionViewModel): void {

    const index = this.domains.findIndex(
      x => x.description == domain.description
    );
    if (index >= 0) {
      this.domains.splice(index, 1);
    }
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingDomain) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
