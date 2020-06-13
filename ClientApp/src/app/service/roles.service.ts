import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Router } from "@angular/router";

@Injectable()
export class RolesService{
   private url = "https://localhost:44342/api/roles";


    constructor(private router: Router, private http: HttpClient) {}

     getRoles(){
      return this.http.get(this.url);
     }

     myGetRelos():Array<string>{
        let token: string = localStorage.getItem("jwt");

        if (token==null)
        return null;

        let jwtData = token.split('.')[1]
        let decodedJwtJsonData = window.atob(jwtData)     
        var splitted = decodedJwtJsonData.substring(decodedJwtJsonData.indexOf('['),decodedJwtJsonData.indexOf(']')).slice(1); 
        splitted = splitted.replace(/"/g,'');
        var roles = splitted.split(',');
       
        return roles;
     }

     getUserRoles(id:string){
      return this.http.get(this.url + '/' + id);
     }
}