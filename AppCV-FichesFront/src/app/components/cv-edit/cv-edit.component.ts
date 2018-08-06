import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { MandatViewModel } from "../../Models/Mandat-model";
import { ENTER, COMMA } from "@angular/cdk/keycodes";
import { CVService } from "../../Services/cv.service";
import { Observable } from "../../../../node_modules/rxjs";
import { saveAs } from 'file-saver/FileSaver';
@Component({
  selector: "app-cv-edit",
  templateUrl: "./cv-edit.component.html",
  styleUrls: ["./cv-edit.component.css"]
})
export class CvEditComponent implements OnInit {
  UtilisateurId: string;
  mandatCollection: Array<MandatViewModel>;
  mandatSeleted = new MandatViewModel();
  numberPage: number;
  lastPage: number;
  showMandat: boolean = false;
  hiddenButton: string;
  showLoadingCarousel: boolean = true;
  showDownloadFile: boolean = false;
  IsSuccess: boolean = true;
  constructor(private route: ActivatedRoute, private CVserv: CVService) {
    this.mandatCollection = new Array<MandatViewModel>();
    this.route.params.subscribe(params => {
      this.UtilisateurId = params["id"];
    });
  }
  ngOnInit() {}
  setToMandat(event: MandatViewModel): void {
    document.getElementById("anchor").scrollIntoView();
    this.showMandat = false;
    this.mandatSeleted = new MandatViewModel();

    this.mandatCollection.push(event);
    this.lastPage = this.mandatCollection.length;
  }
  addNewMandatFromList(arg: any): void {
    // this.InputMandatCarousel = arg.newMandat;
    this.numberPage = arg.numberPage;
    this.showMandat = true;
    this.lastPage = this.mandatCollection.length;
    this.mandatSeleted = new MandatViewModel();

    this.hiddenButton = this.mandatSeleted.mandatStatus;
  }
  onChangePage(changeTo: number): void {
    const newValue = this.mandatCollection[changeTo];
    if (newValue != null) {
      this.mandatSeleted = newValue;
      this.numberPage = changeTo + 1;
    }
  }
  onChangeMandatFromTableMandat(arg: any): void {
    this.showLoadingCarousel = true;
    this.CVserv.LoadMandat(this.UtilisateurId, arg.graphId).subscribe(
      (valeu: MandatViewModel) => {
        this.mandatSeleted = valeu;

        this.calcMonth(valeu.debutMandat, valeu.finMandat, "moisMandat");
        this.calcMonth(valeu.debutProjet, valeu.finProjet, "moisProjet");

        this.showMandat = true;
        this.hiddenButton = "modifier";
        this.numberPage = arg.nombre;
        this.showLoadingCarousel = false;
      }
    );
  }

  private calcMonth(init: Date, fin: Date, eleHtml: string) {
    if (init != null && fin != null) {
      var date1: any = new Date(init);
      var date2: any = new Date(fin);
      var diffDays = Math.round((date2 - date1) / (1000 * 60 * 60 * 24 * 30));
      if ("moisMandat" == eleHtml) {
        this.mandatSeleted.moisMandat = diffDays;
      } else {
        this.mandatSeleted.moisProjet = diffDays;
      }
    }
  }

  private DownloadFile() {
    this.showDownloadFile = true;
    this.IsSuccess = true;
    this.CVserv.DownloadCV(this.UtilisateurId)
    .subscribe(response => {
      const fileName = response.headers.get('Content-Disposition').split(';')[1].split('filename')[1].split('=')[1].trim().replace(/"/g, '');;
      const b: any = new Blob([response.body], { type: 'application/vnd.ms-word' });
      saveAs(b, fileName);
      this.showDownloadFile = false;
      this.IsSuccess = true;
       },
       erro => {
         this.IsSuccess = false;
         this.showDownloadFile = false;
       }
  );
  }

classValidator(cssClass: string, optionCssClass): string {
    if (this.showDownloadFile) {
      return cssClass;
    } else {
      return optionCssClass;
    }
  }
}
