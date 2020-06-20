import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ReportService{

    private url = "https://localhost:44342/api/report";

    constructor(private http: HttpClient) {}

    
    getReportProvider(providerid:number): any {
        return this.http.get(this.url + '/provider/' + providerid, {
          responseType: 'blob'
        })
        .pipe(
          map((res: any) => {
            return new Blob([res.body], { type: 'application/pdf' });
          })
        );
      }
  
  
}