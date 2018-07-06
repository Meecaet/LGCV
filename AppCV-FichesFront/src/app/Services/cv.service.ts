import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs';
import { map, take, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CVService {

  private cv: any;
  constructor(private _http: HttpClient) { }

  private doRequest(path: string, method: string= 'get', body: any = null) {
    const rootPath = 'https://localhost:44344';
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

  public GetCV(id: string) {
    /*let headers = new Headers();
      headers.append('Content-Type', 'application/json');*/

    return this.doRequest(`CVPoc/Details/${id}`);
  }

  public SaveCV() {
    return this.doRequest('CV/Save');
  }
}
