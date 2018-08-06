import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { FonctionViewModel } from "../../Models/Fonction-model";
import { CVService } from "../../Services/cv.service";


@Component({
  selector: "app-drop-down-fonction",
  templateUrl: "./drop-down-fonction.component.html",
  styleUrls: ["./drop-down-fonction.component.css"]
})
export class DropDownFonctionComponent implements OnInit {
  @Input() UtilisateurId: string;
  @Input() functonGraphID: string = "";
  fonctionAutoComplete: Array<FonctionViewModel>;
  fonction: FonctionViewModel;
  Teste:FonctionViewModel= new  FonctionViewModel();


  @Output("onChange") onChange = new EventEmitter();
  constructor(private cvServices: CVService) {
    this.fonction = new FonctionViewModel();
    this.fonctionAutoComplete = new Array<FonctionViewModel>();
  }

  ngOnInit() {
    this.cvServices
      .LoadFonction()
      .subscribe((data: Array<FonctionViewModel>) => {
        this.fonctionAutoComplete = data;
      });
  }
  Selected(graphId: any) {
    debugger
     this.onChange.emit(graphId);
  }
}
