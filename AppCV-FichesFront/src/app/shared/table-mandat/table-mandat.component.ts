import {  Component,  OnInit,  Input,  Output,  EventEmitter} from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";
import { CVService } from "../../Services/cv.service";
import { ResumeInterventionViewModel } from "../../Models/CV/ResumeInterventionViewModel";
import { CarouselComponent } from "../carousel/carousel.component";
import { ErrorService } from "../../Services/error.service";
import { CvEditComponent } from "../../components/cv-edit/cv-edit.component";

@Component({
  selector: "app-table-mandat",
  templateUrl: "./table-mandat.component.html",
  styleUrls: ["./table-mandat.component.css"],
  providers: [CarouselComponent]
})
export class TableMandatComponent implements OnInit {
  showLoadingMandat: boolean = false;

  resume: Array<ResumeInterventionViewModel>;

  @Input("mandatCollection")  mandatCollection: Array<MandatViewModel>;
  @Input("numberPage")
  numberPage: number = 0;
  @Output("AddNewMandat")
  AddNewMandat = new EventEmitter();

  @Input("UtilisateurId")
  UtilisateurId: string;

  @Output("onChangeMandatFromTableMandat")
  onChangeMandatFromTableMandat = new EventEmitter();

  @Input("eventCaousel")
  eventCaousel: CarouselComponent;

  constructor(private serv: CVService, private errorServ: ErrorService,private parentComponent: CvEditComponent) {
    this.mandatCollection = new Array<MandatViewModel>();
    this.resume = new Array<ResumeInterventionViewModel>();
  }
  ngOnInit() {
    this.LoadData();
  }
  LoadData() {
    this.showLoadingMandat = true;
    this.serv
      .LoadResumeIntervention(this.UtilisateurId)
      .subscribe((data: Array<ResumeInterventionViewModel>) => {

        this.resume  =  data.filter((x: ResumeInterventionViewModel) => {
          if (x.editionObjecViewModels.length < 1) {
            return   this.SetData(x);
          } else if (
            !x.editionObjecViewModels.some(x => {
              return x.etat == "Modifie" && x.type == "Enlever";
            })
          ) {
            return  this.SetData(x);
          }
        });
        this.showLoadingMandat = false;
      });
  }
  private SetData(data: ResumeInterventionViewModel) :ResumeInterventionViewModel {
    if (data.editionObjecViewModels != null) {
      for (const key in data.editionObjecViewModels) {
        if (data.editionObjecViewModels[key]["etat"] == "Modifie") {
          data[data.editionObjecViewModels[key]["proprieteNom"]] = data.editionObjecViewModels[key]["proprieteValeur"];
        }
      }
    }
    return data;
    //
  }
  SelectedLine(mand: ResumeInterventionViewModel): void {
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
  removeMandat(mand: MandatViewModel) {
     this.eventCaousel.showLoadingCarousel = false;
      if (confirm("Vous voulez supprime ?")) {
        this.showLoadingMandat = true;
        this.eventCaousel.showLoadingCarousel = false;
        this.serv.DeleteMandat(this.UtilisateurId, mand.graphId)
          .subscribe(sub => {

             this.showLoadingMandat = false;
             this.eventCaousel.showLoadingCarousel = false;
             this.parentComponent.showMandat =false;
             this.LoadData();
            },(error)=> this.errorServ.ErrorHandle(error.status));
      }
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingMandat) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }

}
