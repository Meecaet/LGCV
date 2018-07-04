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

  public GetCV() {
    /*let headers = new Headers();
      headers.append('Content-Type', 'application/json');*/

    return this._http.get('https://localhost:44344/CVPoc/Details/1');
  }
}
