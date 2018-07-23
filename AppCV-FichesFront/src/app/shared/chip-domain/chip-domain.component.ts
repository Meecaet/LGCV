import { Component, OnInit, Input } from "@angular/core";
import { MatChipInputEvent } from "@angular/material";
import { DomaineDInterventionViewModel } from "../../Models/DomaineDIntervention-model";
import { ENTER, COMMA } from "@angular/cdk/keycodes";

@Component({
  selector: "app-chip-domain",
  templateUrl: "./chip-domain.component.html",
  styleUrls: ["./chip-domain.component.css"]
})
export class ChipDomainComponent implements OnInit {
  domains: Array<DomaineDInterventionViewModel>;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  constructor() {
    this.domains = new Array<DomaineDInterventionViewModel>();
  }

  ngOnInit() {}
  addDomain(event: MatChipInputEvent): void {

    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || "").trim()) {
      let d = new DomaineDInterventionViewModel();
      d.description = value.trim();
      this.domains.push(d);
    }

    // Reset the input value
    if (input) {
      input.value = "";
    }
  }
  removeDomain(domain: DomaineDInterventionViewModel): void {

    const index = this.domains.findIndex(
      x => x.description == domain.description
    );
    if (index >= 0) {
      this.domains.splice(index, 1);
    }
  }
}
