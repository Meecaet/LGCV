import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '../../../../node_modules/@angular/material';

@Component({
  selector: 'app-modal-old-information',
  templateUrl: './modal-old-information.component.html',
  styleUrls: ['./modal-old-information.component.css']
})
export class ModalOldInformationComponent implements OnInit {

   modalMessage:string;
   originalChamp:string;
  ngOnInit() {
  }
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
    this.modalMessage=data.ModalMessage;
    this.originalChamp=data.OriginalChamp;
  }


}
