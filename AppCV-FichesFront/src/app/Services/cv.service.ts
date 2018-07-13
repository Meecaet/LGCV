import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs';
import { map, take, catchError } from 'rxjs/operators';
import { ResumeInterventionViewModel } from '../Models/CV/ResumeInterventionViewModel';
import { ResumeInterventionsComponent } from '../components/CV/resume-interventions/resume-interventions.component';


@Injectable({
  providedIn: 'root'
})
export class CVService {

  constructor(private _http: HttpClient) { }

  private doRequest(path: string, method: string= 'get', body: any = null) {
    const rootPath = 'https://localhost:44344/';
    const url = `${rootPath}/${path}`;
    switch (method) {
      case 'put':
        return this._http.get(url);
      case 'delete':
        return this._http.delete(url);
      case 'post':
      return this._http.post(url, body);
      default:
        return this._http.get(url);
    }
  }

  private EditActiontUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/Edit`;
  }

  private AddActionUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/Add`;
  }

  private DeleteActionUrl(cvId: string, controllerName: string, detailId: string) {
    return `${controllerName}/${cvId}/Delete/${detailId}`;
  }

  private DetailActionUrl(cvId: string, controllerName: string, detailId: string) {
    return `${controllerName}/${cvId}/Detail/${detailId}`;
  }

  public GetCV(id: string) {
    /*let headers = new Headers();
      headers.append('Content-Type', 'application/json');*/

    return this.doRequest(`CVPoc/Details/${id}`);
  }

  public EditBio(cv: any) {
    const url = this.EditActiontUrl(cv.graphIdUtilisateur, 'Bio');
    return this.doRequest(url, 'post', cv);
  }

  public GetCVResumeInterventions(idUtilisateur: string)  {
    const url = `ResumeIntervention/${idUtilisateur}/All`;
    return this.doRequest(url).pipe<ResumeInterventionViewModel[]>(
      map((data: any[]) => {
        let resumes: ResumeInterventionViewModel[] = [];
        debugger;
        for (let d of data)
        {
          resumes.push({
            nombre: d.nombre,
            entrerise: d.entrerise,
            client: d.client,
            projet: d.projet,
            envergure: d.envergure,
            fonction: d.fonction,
            annee: d.annee,
            efforts: d.efforts,
            debutMandat: d.debutMandat});
        }

        return resumes;
      }));
  }

  public NotifierChangement() {
    return this.doRequest('CV/Save');
  }
}
