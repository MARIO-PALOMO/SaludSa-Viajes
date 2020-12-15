import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { retry } from "rxjs/operators";
import { environment } from '../../../environments/environment';

@Injectable()
export class ApiService {

  url = environment.urlService;
  constructor(private http: HttpClient) { 
  }

  post(endpoint: string, body: any): Observable<any> {

    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post(this.url + "" + endpoint, body, httpOptions).pipe(retry(3));
  }

  get(endpoint: string, params?: any, reqOpts?: any): Observable<any> {

    if (!reqOpts) {
      reqOpts = {
        params: new HttpParams()
      };
    }

    if (params) {
      reqOpts.params = new HttpParams();
      for (let k in params) {
        reqOpts.params.set(k, params[k]);
      }
    }

    reqOpts = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };

    return this.http.get(this.url + "" + endpoint, reqOpts).pipe(retry(3));
  }

}
