import { Component, OnInit, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
import { TechnologieViewModel } from "../../Models/Technologie-model";
import { CVService } from "../../Services/cv.service";
import { ErrorService } from "../../Services/error.service";

@Component({
  selector: "app-modal-technologie",
  templateUrl: "./modal-technologie.component.html",
  styleUrls: ["./modal-technologie.component.css"]
})
export class ModalTechnologieComponent implements OnInit {
  tecnologie: Array<TechnologieViewModel>;
  tecnologieToAdd: Array<TechnologieViewModel>;
  categorie: string;
  UtilisateurId: string;
  constructor(
    public dialogRef: MatDialogRef<ModalTechnologieComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private cvServ: CVService,
    private err: ErrorService
  ) {
    this.tecnologie = data.TechnologieViewModel;
    this.UtilisateurId = data.UtilisateurId;
    this.tecnologieToAdd = Array<TechnologieViewModel>();
    this.addTechnologie();
  }

  ngOnInit() {}
  addTechnologie(): void {
    this.tecnologieToAdd.push(new TechnologieViewModel());
  }
  SelecteValue(model: TechnologieViewModel, selected: string): void {
    model.categorie = this.tecnologie.filter((x: TechnologieViewModel) => {
     return x.graphId === selected;
    })[0].categorie;
  }
  SaveData(modelToSave: Array<TechnologieViewModel>): void {
    this.cvServ.AddTechnologies(modelToSave, this.UtilisateurId).subscribe(
      x => {
        this.dialogRef.close();
      },
      (error: any) => {
        this.err.ErrorHandle(error.status);
      }
    );
  }
}
