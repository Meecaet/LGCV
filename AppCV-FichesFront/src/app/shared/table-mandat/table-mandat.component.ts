import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewChild
} from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";
import { CVService } from "../../Services/cv.service";
import { ResumeInterventionViewModel } from "../../Models/CV/ResumeInterventionViewModel";
import { CarouselComponent } from "../carousel/carousel.component";

@Component({
  selector: "app-table-mandat",
  templateUrl: "./table-mandat.component.html",
  styleUrls: ["./table-mandat.component.css"],
  providers:[CarouselComponent]
})
export class TableMandatComponent implements OnInit {

  showLoadingMandat: boolean = false;
  @Input("UtilisateurId") UtilisateurId: string;

  resume: Array<ResumeInterventionViewModel>;

  @Input("mandatCollection") mandatCollection: Array<MandatViewModel>;
  @Input("numberPage") numberPage: number = 0;
  @Output("AddNewMandat") AddNewMandat = new EventEmitter();

  @Output("onChangeMandatFromTableMandat")


  onChangeMandatFromTableMandat = new EventEmitter();
  constructor(private serv: CVService) {
    this.mandatCollection = new Array<MandatViewModel>();
    this.resume = new Array<ResumeInterventionViewModel>();
  }

  SelectedLine(mand: MandatViewModel): void {
    this.onChangeMandatFromTableMandat.emit(mand)
   }

  ngOnInit() {
    this.serv
      .LoadResumeIntervention(this.UtilisateurId)
      .subscribe((data: Array<ResumeInterventionViewModel>) => {
        this.showLoadingMandat = false;
        this.resume = data;
      });
  }
  addMandat() {
    this.numberPage = this.mandatCollection.length + 1;
    this.AddNewMandat.emit({
      newMandat: new MandatViewModel(),
      numberPage: this.numberPage,
      status: "ajouter"
    });
  }
  removeMandat(ele: any, button: any, mand: MandatViewModel) {
    var eleStyle = document.getElementById(ele);
    if (confirm("Vous voulez supprime ?")) {
      mand.highlight = "highlighterror";
      document.getElementById(button).remove();
      this.deleteFromDatabase(mand);
    }
  }
  deleteFromDatabase(form: MandatViewModel) {
    alert("to implement");
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingMandat) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
