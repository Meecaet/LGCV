import { Component, OnInit } from '@angular/core';
import { CVService } from '../../Services/cv.service';
import { ActivatedRoute } from '@angular/router';

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

  constructor(
    private cv: CVService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.loading = true;
    this.route.params.subscribe(params => {
      const cvId = params['id'];
      this.cv.GetCV(cvId).subscribe(data => {
        if (data != null) {
          this.cvDetails = data;
        }

        this.loading = false;
      });

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
