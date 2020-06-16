import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';



@Injectable()
export class CatalogService{

    private url = "https://localhost:44342/api/catalog";

    constructor(private http: HttpClient) {}

    getCatalogsByProviderId(providerId:number){
        return this.http.get(this.url + '/provider/' + providerId);
    }

    deleteCatalog(id: number){
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, {headers:myHeaders, observe: 'response'});
    }
}