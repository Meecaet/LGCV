import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ResumeInterventionViewModel } from "../Models/CV/ResumeInterventionViewModel";
import { BioViewModel } from "../Models/Bio-Model";
import { LangueViewModel } from "../Models/Langue-model";
import { FonctionViewModel } from "../Models/Fonction-model";

@Injectable({
  providedIn: "root"
})
export class CVService {
  constructor(private _http: HttpClient) {}
  private rootPath = "https://localhost:44344";
  private doRequest<T>(
    path: string,
    method: string = "get",
    body: any = null
  ): Observable<T> {
    const url = `${this.rootPath}/${path}`;
    switch (method) {
      case "put":
        return this._http.get<T>(url);
      case "delete":
        return this._http.delete<T>(url);
      case "post":
        return this._http.post<T>(url, body);
      default:
        return this._http.get<T>(url);
    }
  }

  private EditActiontUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/Edit`;
  }

  private AddActionUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/Add`;
  }

  private DeleteActionUrl(
    cvId: string,
    controllerName: string,
    detailId: string
  ) {
    return `${controllerName}/${cvId}/Delete/${detailId}`;
  }

  private DetailActionUrl(
    controllerName: string,
    detailId: string
  ) {
    return `${controllerName}/Detail/${detailId}`;
  }

  public GetCV(id: string) {
    /*let headers = new Headers();
      headers.append('Content-Type', 'application/json');*/

    return this.doRequest(`CVPoc/Details/${id}`);
  }

  public EditBio(cv: any) {
    debugger;
    const url = this.EditActiontUrl(cv.graphIdUtilisateur, "Bio");
    return this.doRequest(url, "post", cv);
  }

  public GetCVResumeInterventions(idUtilisateur: string) {
    const url = `ResumeIntervention/${idUtilisateur}/All`;
    return this.doRequest(url);
  }

  public GetBio(idUtilisateur: string) {
    const url = this.DetailActionUrl("Bio", idUtilisateur);
    return this.doRequest(url);
  }

  public CreateBio(bio: BioViewModel) {
    const url = "Bio/Create";
    return this.doRequest<BioViewModel>(url, "post", bio);
  }
  public NotifierChangement() {
    return this.doRequest("CV/Save");
  }
  public UtilizaterLangue(idUtilisateur:string): Observable<Array<LangueViewModel>> {
    return new Observable<Array<LangueViewModel>>();
  }
  public LoadLangue(): Observable<Array<LangueViewModel>> {
    const url = this.rootPath + "/api/FrontEndLoadData/GetAllLangues";
     return this._http.get<Array<LangueViewModel>>(url);
  }
  public LoadFonction(): Observable<Array<FonctionViewModel>> {
    const url = this.rootPath + "/api/FrontEndLoadData/GetAllFonctions";
     return this._http.get<Array<FonctionViewModel>>(url);
  }
}
