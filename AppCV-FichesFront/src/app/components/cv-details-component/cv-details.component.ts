import { Component, OnInit } from '@angular/core';
import { CVService } from '../../Services/cv.service';

@Component({
  selector: 'app-cv-details',
  templateUrl: './cv-details.component.html',
  styleUrls: ['./cv-details.component.css']
})
export class CvDetailsComponent implements OnInit {
  public cvDetails: any = {};

  constructor(private cv: CVService) {
  }

  ngOnInit() {
        this.cv.GetCV().subscribe(data => {
      this.cvDetails = data;
    });
  }

}
