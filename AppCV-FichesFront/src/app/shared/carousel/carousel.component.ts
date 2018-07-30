import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { MandatViewModel } from "../../Models/Mandat-model";
import { TacheViewModel } from "../../Models/Tache-model";
import { ENTER, COMMA } from "@angular/cdk/keycodes";
import { MatChipInputEvent } from "@angular/material";
import { Router } from "@angular/router";

@Component({
  selector: "app-carousel",
  templateUrl: "./carousel.component.html",
  styleUrls: ["./carousel.component.css"]
})
export class CarouselComponent implements OnInit {
  @Input("ngModelMandat") mandat: MandatViewModel;
  @Input("lastPage") lastPage: number;
  @Output() OutPutMandatCarousel = new EventEmitter();
  @Input("numberPage") numberPage: number;
  @Output("onChangePage") onChangePage = new EventEmitter();

  @Input() private uploadSuccess: EventEmitter<boolean>;

  @Input("hiddenButton") hiddenButton: string;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;

  showLoadingCarousel:boolean=false;
  ngOnInit() {}
  constructor(private route: Router) {
    this.mandat = new MandatViewModel();
  }
  private calcMonth(init: Date, fin: Date, eleHtml: HTMLSpanElement) {
    if (init != null && fin != null) {
      var date1: any = new Date(init);
      var date2: any = new Date(fin);
      var diffDays = Math.round((date2 - date1) / (1000 * 60 * 60 * 24 * 30));
      eleHtml.innerHTML = "(" + diffDays + " mois )";
    }
  }

  LoadMandat(utilizateurId:string,mandat:MandatViewModel){
    this.showLoadingCarousel=true;
  }
  addTache(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || "").trim()) {
      let d = new TacheViewModel();
      d.description = value.trim();
      this.mandat.taches.push(d);
    }

    // Reset the input value
    if (input) {
      input.value = "";
    }
  }
  removeTache(domain: TacheViewModel): void {
    const index = this.mandat.taches.findIndex(
      x => x.description == domain.description
    );
    if (index >= 0) {
      this.mandat.taches.splice(index, 1);
    }
  }
  previous(currentPage: number) {
    debugger;

    const newpage = currentPage - 2;
    this.onChangePage.emit(newpage);
  }
  next(currentPage: number) {
    const newpage = currentPage + 1;
    this.onChangePage.emit(newpage);
  }

  SendMandatCarousel(mandat: MandatViewModel): void {
    this.OutPutMandatCarousel.emit(mandat);
  }
  ModifierMandatCarousel(mandat: MandatViewModel): void {
    alert("to implement");
  }
  classValidator(cssClass :string,optionCssClass):string{
    if(this.showLoadingCarousel){
      return cssClass;
    }else{
      return optionCssClass;
    }
  }
}
