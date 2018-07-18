import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ResumeInterventionViewModel } from "../Models/CV/ResumeInterventionViewModel";
import { BioViewModel } from "../Models/Bio-Model";
import { LangueViewModel } from "../Models/Langue-model";

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
    cvId: string,
    controllerName: string,
    detailId: string
  ) {
    return `${controllerName}/${cvId}/Detail/${detailId}`;
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

    // .pipe<ResumeInterventionViewModel[]>(
    //   map((data: any[]) => {
    //     let resumes: ResumeInterventionViewModel[] = [];
    //     debugger;
    //     for (let d of data) {
    //       resumes.push({
    //         nombre: d.nombre,
    //         entrerise: d.entrerise,
    //         client: d.client,
    //         projet: d.projet,
    //         envergure: d.envergure,
    //         fonction: d.fonction,
    //         annee: d.annee,
    //         efforts: d.efforts,
    //         debutMandat: d.debutMandat
    //       });
    //     }

    //     return resumes;
    //   })
    // );
  }

  public CreateBio(bio: BioViewModel) {
    debugger;
    const url = "Bio/Create";
    return this.doRequest<BioViewModel>(url, "post", bio);
  }
  public NotifierChangement() {
    return this.doRequest("CV/Save");
  }
  public UtilizaterLangue(): Observable<Array<LangueViewModel>> {


    return;
  }
  public LoadLangue(): Observable<Array<LangueViewModel>> {
    const url = this.rootPath + "/api/FrontEndLoadData/GetAllLangues";
    debugger;
    return this._http.get<Array<LangueViewModel>>(url);
  }
}
