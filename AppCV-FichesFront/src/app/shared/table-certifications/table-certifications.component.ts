import { Component, OnInit, Input } from "@angular/core";
import { CertificationViewModel } from "../../Models/Certification-model";

@Component({
  selector: "app-table-certifications",
  templateUrl: "./table-certifications.component.html",
  styleUrls: ["./table-certifications.component.css"]
})
export class TableCertificationsComponent implements OnInit {
  certifications: Array<CertificationViewModel>;
  highlight: string;
  constructor() {
    this.certifications = new Array<CertificationViewModel>();
  }
  ngOnInit() {}
  AddCertifications(): void {
    this.certifications.push(new CertificationViewModel());
  }
  removeCertification(
    ele: any,
    button: any,
    certification: CertificationViewModel
  ) {
    if (confirm("Vous voulez supprime ?")) {
      certification.highlight = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(certification);
    }
  }

  deleteFromDatabase(certification: CertificationViewModel) {
    alert("to implement");
  }
  OrderBy(): void {
    this.certifications = Array.from(this.certifications).sort(
      (item1: any, item2: any) => {
        return this.orderByComparator(item2["annee"], item1["annee"]);
      }
    );
  }
  private orderByComparator(a: any, b: any): number {
    if (
      isNaN(parseFloat(a)) ||
      !isFinite(a) ||
      (isNaN(parseFloat(b)) || !isFinite(b))
    ) {
      if (a < b) return -1;
      if (a > b) return 1;
    } else {
      if (parseFloat(a) < parseFloat(b)) return -1;
      if (parseFloat(a) > parseFloat(b)) return 1;
    }
    return 0;
  }
}
