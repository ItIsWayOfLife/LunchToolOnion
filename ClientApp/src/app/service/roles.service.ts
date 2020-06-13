import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Router } from "@angular/router";

import {UserChangeRoles} from '../admin/users/userChangeRoles';

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

     editRoles(userChangeRoles:UserChangeRoles){
      console.log("id1: "+userChangeRoles.id);
      console.log("roles2: "+userChangeRoles.roles);
      const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
      return this.http.put(this.url, JSON.stringify(userChangeRoles), {headers:myHeaders, observe: 'response'});
     }
}