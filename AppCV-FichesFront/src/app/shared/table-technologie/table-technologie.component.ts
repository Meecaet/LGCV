import { Component, OnInit, Input } from '@angular/core';
import { TechnologieViewModel } from '../../Models/Technologie-model';

@Component({
  selector: 'app-table-technologie',
  templateUrl: './table-technologie.component.html',
  styleUrls: ['./table-technologie.component.css']
})
export class TableTechnologieComponent implements OnInit {
@Input()technologies : Array<TechnologieViewModel>
  constructor() { }

  ngOnInit() {
  }

}
