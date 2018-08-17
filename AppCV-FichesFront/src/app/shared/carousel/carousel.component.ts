import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";
import { TacheViewModel } from "../../Models/Tache-model";
import { ENTER, COMMA } from "@angular/cdk/keycodes";
import { MatChipInputEvent } from "@angular/material";
import { Router } from "@angular/router";
import { Observable } from "../../../../node_modules/rxjs";
import { CVService } from "../../Services/cv.service";
import { TechnologieViewModel } from "../../Models/Technologie-model";
import { FormControl } from "../../../../node_modules/@angular/forms";
import { TableMandatComponent } from "../table-mandat/table-mandat.component";

@Component({
  selector: "app-carousel",
  templateUrl: "./carousel.component.html",
  styleUrls: ["./carousel.component.css"]
})
export class CarouselComponent implements OnInit {
  //Inputs
  @Input("ngModelMandat")  mandat: MandatViewModel;
  @Input("lastPage")
  lastPage: number;
  @Input("numberPage")
  numberPage: number;
  @Input("hiddenButton")
  hiddenButton: string;
  @Input("showLoadingCarousel")
  showLoadingCarousel: boolean = true;
  //Outputs
  @Output("OutPutMandatCarousel")
  OutPutMandatCarousel = new EventEmitter();
  @Output("OutPutMandatCarouselEdit")
  OutPutMandatCarouselEdit = new EventEmitter();
  @Output("onChangePage")
  onChangePage = new EventEmitter();

  @Input("eventMandat")
  eventMandat: TableMandatComponent;
  @Input("utilizateurId")
  utilizateurId: string;

  myControl: FormControl = new FormControl();
  techs: Observable<Array<TechnologieViewModel>>;

  public watchTest;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  ngOnInit() {}
  constructor(private route: Router, private cvServ: CVService) {
    this.mandat = new MandatViewModel();
    this.techs = this.cvServ.LoadTechnologie();
    this.techs.subscribe(sub => {
      return sub;
    });
  }
  calcMonth(init: Date, fin: Date, eleHtml: string) {
    if (init != null && fin != null) {
      var date1: any = new Date(init);
      var date2: any = new Date(fin);
      var diffDays = Math.round((date2 - date1) / (1000 * 60 * 60 * 24 * 30));

      if ("moisMandat" == eleHtml) {
        this.mandat.moisMandat = diffDays;
      } else {
        this.mandat.moisProjet = diffDays;
      }
    }
  }
  LoadMandat(utilizateurId: string, mandat: MandatViewModel) {
    this.showLoadingCarousel = true;
  }

  //#region Tache
  addTache(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;
    if ((value || "").trim()) {
      let d = new TacheViewModel();
      d.description = value.trim();
      this.mandat.taches.push(d);
      if (this.hiddenButton !== "ajouter") {
        debugger
         this.cvServ.AddTache(this.eventMandat.UtilisateurId,   this.mandat.graphId,d).subscribe(()=>{},
         (erro)=>{
debugger
         })
      }

    }
    if (input) {
      input.value = "";
    }
  }
  removeTache(tache: TacheViewModel): void {
    if (tache.graphId !== undefined) {
      this.cvServ.DeleteTache(this.eventMandat.UtilisateurId,  this.mandat.graphId, tache.graphId);
    }

    const index = this.mandat.taches.findIndex(
      x => x.description == tache.description
    );
    if (index >= 0) {
      this.mandat.taches.splice(index, 1);
    }
  }
  //#endregion

  previous(currentPage: number) {
    const newpage = currentPage - 2;
    this.onChangePage.emit(newpage);
  }
  next(currentPage: number) {
    const newpage = currentPage + 1;
    this.onChangePage.emit(newpage);
  }
  SendMandatCarousel(mandat: MandatViewModel): void {
    this.OutPutMandatCarousel.emit(mandat);
    this.cvServ
      .AddMandat(this.eventMandat.UtilisateurId, mandat)
      .subscribe((sub: MandatViewModel) => {
        this.eventMandat.LoadData();
      });
  }

  ModifierMandatCarousel(mandat: MandatViewModel): void {
    this.OutPutMandatCarouselEdit.emit(mandat);
    this.cvServ
      .EditMandat(this.eventMandat.UtilisateurId, mandat)
      .subscribe((sub: MandatViewModel) => {
        this.eventMandat.LoadData();
      });
  }

  //#region Technologie
  AddTechno(selected: FormControl): void {
    this.techs.subscribe((tec: Array<TechnologieViewModel>) => {
      let newValue = tec.filter(f => {
        return f.description == selected.value;
      })[0];
      if (this.hiddenButton !== "ajouter") {
        this.cvServ.AddMandatTechnologie(
          this.eventMandat.UtilisateurId,
          this.mandat.graphId,
          newValue
        );
      }
      this.mandat.technologies.push(newValue);
    });
  }
  RemoveTech(item: TechnologieViewModel): void {
    if (this.hiddenButton !== "ajouter") {
      this.cvServ
        .DeleteMandatTechnologie(
          this.eventMandat.UtilisateurId,
          this.mandat.graphId,
          item
        )
        .subscribe(s => {});
    }

    let index = this.mandat.technologies.findIndex(
      x => x.graphId == item.graphId
    );
    this.mandat.technologies.splice(index, 1);
  }
  //#endregion

  SelectedFonction(graphIdFonction: string) {
    this.mandat.graphIdFonction = graphIdFonction;
  }
  classValidator(cssClass: string, optionCssClass): string {
    if (this.showLoadingCarousel) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }

}
