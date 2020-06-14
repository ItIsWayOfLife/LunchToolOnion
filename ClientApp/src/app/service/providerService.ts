import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {Provider} from '../provider/provider';



@Injectable()
export class ProviderService{
   private url = "https://localhost:44342/api/provider";

    constructor(private http: HttpClient) {}

    getProviders(){
        return this.http.get(this.url);
       }
}