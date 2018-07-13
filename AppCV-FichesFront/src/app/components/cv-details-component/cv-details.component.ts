import { Component, OnInit } from '@angular/core';
import { CVService } from '../../Services/cv.service';
import { ActivatedRoute } from '@angular/router';
/*import {MatPaginator, MatTableDataSource} from '@angular/material';
import { ResumeInterventionViewModel } from '../../Models/CV/ResumeInterventionViewModel';*/


@Component({
  selector: 'app-cv-details',
  templateUrl: './cv-details.component.html',
  styleUrls: ['./cv-details.component.css']
})
export class CvDetailsComponent implements OnInit {

  public cvDetails: any = {};

  private loading = false;
  private savingBio = false;
  private savingInterventionDomains = false;
  private savingFormationAcademique = false;
  private savingCertifications = false;
  private savingTechnologies = false;
  private savingLangues = false;
  private savingPerfectionnements = false;

  /*dataSource = new MatTableDataSource<ResumeInterventionViewModel>();
  @ViewChild(MatPaginator) paginator: MatPaginator;*/

  constructor(
    private cv: CVService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.loading = true;
    this.route.params.subscribe(params => {
      const idUtilisateur = params['id'];
      this.loadCV(idUtilisateur);
    });
  }

  loadCV(idUtilisateur: string) {
    this.cv.GetCV(idUtilisateur).subscribe(data => {
      if (data != null) {
        this.cvDetails = data;
      }

      this.loading = false;
    });
  }

  saveBio() {
    this.savingBio = true;
    const bio = {
      Nom: this.cvDetails.nom,
      Prenom: this.cvDetails.Prenom,
      Biographie: this.cvDetails.Biographie,
      Fonction: this.cvDetails.Fonction};

    this.cv.EditBio(this.cvDetails)
    .subscribe(
      data => {
        console.log(data);
        this.savingBio = false;
      });
  }
}
