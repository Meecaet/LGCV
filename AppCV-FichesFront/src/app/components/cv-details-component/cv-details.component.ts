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

  constructor(
    private cv: CVService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.loading = true;
    this.route.params.subscribe(params => {
      const cvId = params['id'];

      console.log(`Parametre ID: ${cvId}`);

      this.cv.GetCV(cvId).subscribe(data => {
        if (data != null) {
          this.cvDetails = data;
        }

        this.loading = false;
      });

    });
  }

}
