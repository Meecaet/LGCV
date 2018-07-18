import { Component, OnInit, Input } from '@angular/core';
import { ResumeInterventionViewModel } from '../../../Models/CV/ResumeInterventionViewModel';
import { CVService } from '../../../Services/cv.service';


@Component({
  selector: 'app-resume-interventions',
  templateUrl: './resume-interventions.component.html',

})
export class ResumeInterventionsComponent implements OnInit {

  // private _utilisateurId: string;
  // @Input() Interventions: Array<ResumeInterventionViewModel>;


  //   set UtilisateurId(id: string) {
  //     this._utilisateurId = id;
  //     this.cvService.GetCVResumeInterventions(this._utilisateurId)
  //       .subscribe((data: Array<ResumeInterventionViewModel>)  => {
  //       this.Interventions = data;
  //       });
  //   }

  // get name(): string { return this._utilisateurId; }

  constructor(private cvService: CVService) {



  }

  ngOnInit() {
  }
}
