import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';



@Injectable()
export class CatalogService{

    private url = "https://localhost:44342/api/catalog";

    constructor(private http: HttpClient) {}

    getCatalogsByProviderId(providerId:number){
        return this.http.get(this.url + '/provider/' + providerId);
    }
}