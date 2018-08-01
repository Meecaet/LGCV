import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ResumeInterventionViewModel } from "../Models/CV/ResumeInterventionViewModel";
import { BioViewModel } from "../Models/Bio-Model";
import { LangueViewModel } from "../Models/Langue-model";
import { FonctionViewModel } from "../Models/Fonction-model";
import { CertificationViewModel } from "../Models/Certification-model";
import { DomaineDInterventionViewModel } from "../Models/DomaineDIntervention-model";
import { FormationAcademiqueViewModel } from "../Models/FormationAcademique-model";
import { MandatViewModel } from "../Models/Mandat-model";

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

  IsTokenValid() : Observable<boolean> {
    const url = this.rootPath + "/api/AccountApi/IsTokenValid";
    return this._http.get<boolean>(url);
  }

  private EditActiontUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/Edit`;
  }

  private AddActionUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/Add`;
  }
  private AllActionUrl(cvId: string, controllerName: string) {
    return `${controllerName}/${cvId}/All`;
  }
  private LoadMandatActionUrl(utilisateurId: string, mandatId: string) {
    return `Mandat/${utilisateurId}/Detail/${mandatId}`;
  }

  private DeleteActionUrl(
    cvId: string,
    controllerName: string,
    detailId: string
  ) {
    return `${controllerName}/${cvId}/Delete/${detailId}`;
  }

  private DetailActionUrl(controllerName: string, detailId: string) {
    return `${controllerName}/Detail/${detailId}`;
  }

  public GetCV(id: string) {
    /*let headers = new Headers();
      headers.append('Content-Type', 'application/json');*/

    return this.doRequest(`CVPoc/Details/${id}`);
  }

  public EditBio(utilisateurId: any, model: any) {
    debugger;
    const url = this.EditActiontUrl(utilisateurId, "Bio");
    return this.doRequest(url, "post", model);
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

  public LoadFonction(): Observable<Array<FonctionViewModel>> {
    const url = this.rootPath + "/api/FrontEndLoadData/GetAllFonctions";
    return this._http.get<Array<FonctionViewModel>>(url);
  }

  ///
  // FormationAcademique
  //
  LoadFormationAcademique(idUtilisateur: string) {
    const url = this.AllActionUrl(idUtilisateur, "FormationAcademique");
    return this.doRequest<Array<FormationAcademiqueViewModel>>(url, "post");
  }
  FormationAcademique(
    domain: FormationAcademiqueViewModel,
    idUtilisateur: string
  ) {
    const url = this.AddActionUrl(idUtilisateur, "FormationAcademique");
    return this.doRequest<FormationAcademiqueViewModel>(url, "post", domain);
  }

  ///
  //ResumeIntervention
  ///

  LoadResumeIntervention(idUtilisateur: string) {

    const url = this.AllActionUrl(idUtilisateur, "ResumeIntervention");
    return this.doRequest<Array<ResumeInterventionViewModel>>(url, "post");
  }

  AddResumeIntervention(
    idUtilisateur: string,
    domain: ResumeInterventionViewModel
  ) {
    const url = this.AddActionUrl(idUtilisateur, "ResumeIntervention");
    return this.doRequest<ResumeInterventionViewModel>(url, "post", domain);
  }

  ///
  // Domain
  ///
  LoadDomain(idUtilisateur: string) {
    const url = this.AllActionUrl(idUtilisateur, "CVDomainIntervention");
    return this.doRequest<Array<DomaineDInterventionViewModel>>(url, "post");
  }

  AddDomain(idUtilisateur: string, domain: DomaineDInterventionViewModel) {
    const url = this.AddActionUrl(idUtilisateur, "CVDomainIntervention");
    return this.doRequest<DomaineDInterventionViewModel>(url, "post", domain);
  }
  ///
  // Certifications
  ///
  LoadCertification(idUtilisateur: string) {
    const url = this.AllActionUrl(idUtilisateur, "Certification");
    return this.doRequest<Array<CertificationViewModel>>(url, "post");
  }
  public AddCertifications(
    certification: CertificationViewModel,
    idUtilisateur: string
  ) {
    const url = this.AddActionUrl(idUtilisateur, "Certification");
    return this.doRequest<CertificationViewModel>(url, "post", certification);
  }
  ///
  // Mandat
  ///
  LoadMandat(idUtilisateur: string,idMandat :string) {

    const url = this.LoadMandatActionUrl(idUtilisateur,idMandat);
    return this.doRequest<MandatViewModel>(url, "get");
  }
  /// Langue
  public UtilizaterLangue(idUtilisateur: string  ): Observable<Array<LangueViewModel>> {
    const url = this.rootPath + "/Langue/"+idUtilisateur+"/All";
    return this._http.get<Array<LangueViewModel>>(url);
  }

  public LoadLangue(): Observable<Array<LangueViewModel>> {
    const url = this.rootPath + "/api/FrontEndLoadData/GetAllLangues";
    return this._http.get<Array<LangueViewModel>>(url);
  }
}
