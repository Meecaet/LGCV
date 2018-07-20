import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { RoleViewModel } from "../Models/Admin/Role-model";


@Injectable({
  providedIn: "root"
})
export class AdminService {
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

  public CreateRole(role: RoleViewModel) {
    const url = "Role/Create";
    return this.doRequest<RoleViewModel>(url, "post", role);
  }

  public GetRole() {
    return this.doRequest(`Role/GetAll`);
  }

}
