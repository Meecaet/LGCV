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
import { environment } from "../../environments/environment";
import { TechnologieViewModel } from "../Models/Technologie-model";
import { TechnologieByCategorieViewModel } from "../Models/TechlogieByCategorie-model";
import { PerfectionnementViewModel } from "../Models/Perfectionnement-model";
import { ConferenceViewModel } from "../Models/Conference-model";
import { PublicationViewModel } from "../Models/Publication-model";
import { TacheViewModel } from "../Models/Tache-model";

@Injectable({
  providedIn: "root"
})
export class CVService {
  constructor(private _http: HttpClient) {
    this.rootPath = environment.apiUrl;
  }
  private rootPath = "";
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

  IsTokenValid(): Observable<boolean> {
    const url = this.rootPath + "/AccountApi/IsTokenValid";
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

  public DownloadCV(id: string) {
    return this._http.get(`${this.rootPath}/FichierWord/${id}`, {
      observe: "response",
      responseType: "arraybuffer"
    });
  }

  public EditBio(utilisateurId: any, model: any) {
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

  /**
   * UtilizateurPublication
   */
  public UtilizateurPublication(
    idUtilisateur: string
  ): Observable<Array<PublicationViewModel>> {
    const url = this.rootPath + "/Publication/" + idUtilisateur + "/All";
    return this._http.get<Array<PublicationViewModel>>(url);
  }
  public AddPublication(
    modelToSave: ConferenceViewModel,
    idUtilisateur: string
  ): Observable<ConferenceViewModel> {
    const url = this.rootPath + "/Publication/" + idUtilisateur + "/add";
    return this._http.post<ConferenceViewModel>(url, modelToSave);
  }
  DeletePublication(
    idUtilisateur: string,
    graphId: string
  ): Observable<PublicationViewModel> {
    const url = "Publication/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<PublicationViewModel>(url, "post");
  }

  /**
   * UtilizateurConference
   */
  public UtilizateurConference(
    idUtilisateur: string
  ): Observable<Array<ConferenceViewModel>> {
    const url = this.rootPath + "/Conference/" + idUtilisateur + "/All";
    return this._http.get<Array<ConferenceViewModel>>(url);
  }
  public AddConference(
    modelToSave: ConferenceViewModel,
    idUtilisateur: string
  ): Observable<ConferenceViewModel> {
    const url = this.rootPath + "/Conference/" + idUtilisateur + "/add";
    return this._http.post<ConferenceViewModel>(url, modelToSave);
  }
  DeleteConference(
    idUtilisateur: string,
    graphId: string
  ): Observable<ConferenceViewModel> {
    const url = "Conference/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<ConferenceViewModel>(url, "post");
  }

  /**
   *   Technologie
   */
  public UtilizateurTechnologie(
    idUtilisateur: string
  ): Observable<Array<TechnologieByCategorieViewModel>> {
    const url = this.rootPath + "/Technologies/" + idUtilisateur + "/All";
    return this._http.get<Array<TechnologieByCategorieViewModel>>(url);
  }

  public LoadTechnologie(): Observable<Array<TechnologieViewModel>> {
    const url = this.rootPath + "/FrontEndLoadData/GetAllTechnologies";
    return this._http.get<Array<TechnologieViewModel>>(url);
  }
  AddTechnologies(    modelToSave: Array<TechnologieViewModel>,    idUtilisateur: string  ) {
    const url = this.rootPath + "/Technologies/" + idUtilisateur + "/add";
    return this._http.post(url, modelToSave);
  }

  DeleteTechnologie(    idUtilisateur: string,    graphId: string  ): Observable<ConferenceViewModel> {
    const url = "Technologies/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<ConferenceViewModel>(url, "post");
  }

  /**
   *  Perfectionement
   */
  public UtilizateurPerfectionement(
    idUtilisateur: string
  ): Observable<Array<PerfectionnementViewModel>> {
    const url = this.rootPath + "/Perfectionnement/" + idUtilisateur + "/All";
    return this._http.get<Array<PerfectionnementViewModel>>(url);
  }

  DeletePerfectionnement(
    idUtilisateur: string,
    graphId: string
  ): Observable<PerfectionnementViewModel> {
    const url = "Perfectionnement/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<PerfectionnementViewModel>(url, "post");
  }

  AddPerfectionnement(
    modelToSave: PerfectionnementViewModel,
    idUtilisateur: string
  ) {
    const url = this.rootPath + "/Perfectionnement/" + idUtilisateur + "/add";
    return this._http.post(url, modelToSave);
  }

  ///
  // Fonction
  //
  public LoadFonction(): Observable<Array<FonctionViewModel>> {
    const url = this.rootPath + "/FrontEndLoadData/GetAllFonctions";
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

  DeleteFormationAcademique(
    idUtilisateur: string,
    graphId: string
  ): Observable<FormationAcademiqueViewModel> {
    const url = "FormationAcademique/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<FormationAcademiqueViewModel>(url, "post");
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

  DeleteDomain(
    idUtilisateur: string,
    graphId: string
  ): Observable<DomaineDInterventionViewModel> {
    const url = "CVDomainIntervention/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<DomaineDInterventionViewModel>(url, "post");
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

  DeleteCertification(
    idUtilisateur: string,
    graphId: string
  ): Observable<CertificationViewModel> {
    const url = "Certification/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<CertificationViewModel>(url, "post");
  }
  ///
  // Mandat
  ///
  LoadMandat(idUtilisateur: string, idMandat: string) {
    const url = this.LoadMandatActionUrl(idUtilisateur, idMandat);
    return this.doRequest<MandatViewModel>(url, "get");
  }
  AddMandat(idUtilisateur: string, modelToSave: MandatViewModel) {
    const url = this.rootPath + "/Mandat/" + idUtilisateur + "/add";
    return this._http.post(url, modelToSave);
  }
  EditMandat(idUtilisateur: string, modelToSave: MandatViewModel) {
    const url = this.rootPath + "/Mandat/" + idUtilisateur + "/Edit";
    return this._http.post(url, modelToSave);
  }
  DeleteMandat(idUtilisateur, graphId) {
    const url = "Mandat/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<MandatViewModel>(url, "post");
  }

  /// Langue
  public UtilizaterLangue(
    idUtilisateur: string
  ): Observable<Array<LangueViewModel>> {
    const url = this.rootPath + "/Langue/" + idUtilisateur + "/All";
    return this._http.get<Array<LangueViewModel>>(url);
  }

  public LoadLangue(): Observable<Array<LangueViewModel>> {
    const url = this.rootPath + "/FrontEndLoadData/GetAllLangues";
    return this._http.get<Array<LangueViewModel>>(url);
  }
  DeleteLangue(idUtilisateur, graphId): Observable<LangueViewModel> {
    const url = "Langue/" + idUtilisateur + "/Delete/" + graphId;
    return this.doRequest<LangueViewModel>(url, "post");
  }
  ///Tache
  DeleteTache(idUtilisateur,mandatId, model): Observable<TacheViewModel> {
    const url =   this.rootPath+    "/Tache/" + idUtilisateur + "/Mandat/" + mandatId+"/DeleteTache";
    return this._http.post<TacheViewModel>(url,model)
  }
  AddTache(idUtilisateur,mandatId, model): Observable<TacheViewModel> {
    const url =   this.rootPath+  "/Tache/" + idUtilisateur + "/Mandat/" + mandatId+"/AddTache";
    return this._http.post<TacheViewModel>(url,model)
  }
  /**
   * MandatTechnologie
   */
  AddMandatTechnologie(idUtilisateur,mandatId, technologie:TechnologieViewModel): Observable<TechnologieViewModel> {
    const url =     "MandatTechnologie/" + idUtilisateur + "/Mandat/" + mandatId+"/AddTechnologie";
    return this.doRequest<TechnologieViewModel>(url, "post",technologie);
  }
  DeleteMandatTechnologie(idUtilisateur,mandatId, technologie:TechnologieViewModel): Observable<TechnologieViewModel> {
    const url =     "MandatTechnologie/" + idUtilisateur + "/Mandat/" + mandatId+"/DeleteTechnologie";
    return this.doRequest<TechnologieViewModel>(url, "post",technologie);
  }
}
