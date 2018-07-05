import { Component, OnInit } from '@angular/core';
import { ValidatorService } from '../../validator.services';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {
  Email:string=null;
  constructor(private validator: ValidatorService) {}

  ngOnInit() {
  }
  IsValid(value, errorEmpty, errorRegex) {
    debugger;
    this.validator.ValidateEmpty(value, errorEmpty);
    this.validator.ValidateEmail(value, errorRegex);
  }
}
