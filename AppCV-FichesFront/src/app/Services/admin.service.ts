import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { RoleViewModel } from "../Models/Admin/Role-model";
import { RoleAdministratioViewModel } from "../Models/Admin/RoleAdministration-model";
import { UserViewModel } from "../Models/Admin/User-model";


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
    return this.doRequest(`Role/GetRoles`);
  }

  public GetDetail(roleId: string) {
    const url = `Role/Details/${roleId}`;
    return this.doRequest<RoleAdministratioViewModel>(url);
  }

  public GetUsers() {
    return this.doRequest<Array<UserViewModel>>(`Role/GetAllUsers`);
  }

  public EditRol(role: RoleAdministratioViewModel) {
    const url = "Role/Edit";
    return this.doRequest<RoleAdministratioViewModel>(url, "post", role);
  }

  public AddUserRole(roleId: string, userId: string) {
    const url = `Role/AddUserRole/${roleId}/User/${userId}`;
    return this.doRequest<Array<UserViewModel>>(url);
  }

  public DeleteUserRol(roleId: string, userId: string) {
    const url = `Role/DeleteUserRole/${roleId}/User/${userId}`;
    return this.doRequest<Array<UserViewModel>>(url);
  }

  public DeleteRol(roleId: string) {
    const url = `Role/Delete/${roleId}`;
    return this.doRequest<Array<RoleViewModel>>(url);
  }
}
