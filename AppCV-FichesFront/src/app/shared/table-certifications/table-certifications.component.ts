import { Component, OnInit, Input } from '@angular/core';
import { CertificationViewModel } from '../../Models/Certification-model';

@Component({
  selector: 'app-table-certifications',
  templateUrl: './table-certifications.component.html',
  styleUrls: ['./table-certifications.component.css']
})
export class TableCertificationsComponent implements OnInit {
@Input()  certifications: Array<CertificationViewModel>;
  constructor() { }
  ngOnInit() {
  }
  AddCertifications(): void {
    this.certifications.push(new CertificationViewModel());
  }
  removeCertification(ele: CertificationViewModel) {
    const index = this.certifications.findIndex(
      x => x.description == ele.description
    );
    if (index >= 0) {
      this.certifications.splice(index, 1);
    }
  }
}
