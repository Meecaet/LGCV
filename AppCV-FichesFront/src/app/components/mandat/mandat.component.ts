import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-mandat',
  templateUrl: './mandat.component.html',
  styleUrls: ['./mandat.component.css']
})
export class MandatComponent implements OnInit {
  @Input() public mandat: any;

  constructor() { }

  ngOnInit() {
  }

}
