import { Component, OnInit, Input } from "@angular/core";
import { FonctionViewModel } from "../../Models/Fonction-model";
import { CVService } from "../../Services/cv.service";

@Component({
  selector: "app-drop-down-fonction",
  templateUrl: "./drop-down-fonction.component.html",
  styleUrls: ["./drop-down-fonction.component.css"]
})
export class DropDownFonctionComponent implements OnInit {
  @Input() UtilisateurId: string;
  fonctionAutoComplete: Array<FonctionViewModel>;
  fonction: FonctionViewModel;
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
}
