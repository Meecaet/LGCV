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
  providers: [CarouselComponent]
})
export class TableMandatComponent implements OnInit {
  showLoadingMandat: boolean = false;

  resume: Array<ResumeInterventionViewModel>;

  @Input("mandatCollection") mandatCollection: Array<MandatViewModel>;
  @Input("numberPage") numberPage: number = 0;
  @Output("AddNewMandat") AddNewMandat = new EventEmitter();

  @Input("UtilisateurId") UtilisateurId: string;

  @Output("onChangeMandatFromTableMandat")
  onChangeMandatFromTableMandat = new EventEmitter();

  constructor(private serv: CVService) {
    this.mandatCollection = new Array<MandatViewModel>();
    this.resume = new Array<ResumeInterventionViewModel>();
  }
  ngOnInit() {
    this.LoadData();
  }
  LoadData() {
    this.serv
      .LoadResumeIntervention(this.UtilisateurId)
      .subscribe((data: Array<ResumeInterventionViewModel>) => {
        this.showLoadingMandat = false;
        this.resume = data;
      });
  }

  SelectedLine(mand: ResumeInterventionViewModel): void {
    debugger;
    this.SetHighLight("lineSeleted", mand);
    this.onChangeMandatFromTableMandat.emit(mand);
  }
  SetHighLight(highlight: string, mand: ResumeInterventionViewModel) {
    var cssClass = " " + highlight + " ";
    this.UnSetHighLight(cssClass);
    if (mand.highlight != undefined && mand.highlight != "") {
      mand.highlight = mand.highlight + cssClass;
    } else {
      mand.highlight = cssClass;
    }
  }
  UnSetHighLight(highlight) {
    for (let index = 0; index < this.resume.length; index++) {
      if (
        this.resume[index].highlight != undefined &&
        this.resume[index].highlight != ""
      ) {
        var newCss = this.resume[index].highlight.replace(highlight, "");
        this.resume[index].highlight = newCss;
      }
    }
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
