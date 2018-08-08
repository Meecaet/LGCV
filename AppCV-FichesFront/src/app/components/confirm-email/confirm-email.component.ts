import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../../Services/authentication.service';
import { ActivatedRoute } from '../../../../node_modules/@angular/router';
import { ConfirmEmailModel } from '../../Models/ConfirmEmail-model';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {

  showLoadingConfirmEmail = true;
  IsSuccess = false;
  model: ConfirmEmailModel = new ConfirmEmailModel();

  constructor( private auth: AuthenticationService, private route: ActivatedRoute)
  {
    this.route.queryParams.subscribe(params => {
      this.model.code = params.code;
      this.model.userId = params.userId;
    });
  }

  ngOnInit() {
    this.ConfirmEmail();
  }

  ConfirmEmail() {
    this.showLoadingConfirmEmail = true;
     this.auth.ConfirmEmail(this.model).subscribe(
       (data: any) => {
         this.showLoadingConfirmEmail = false;
         this.IsSuccess = true;
       },
       erro => {
         this.IsSuccess = false;
         this.showLoadingConfirmEmail = false;
       }
     );
  }

  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingConfirmEmail) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }

}
