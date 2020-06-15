import {Injectable, OnInit} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {UserChangeRoles} from '../admin/users/userChangeRoles';

@Injectable()
export class RolesService implements OnInit{
   private url = "https://localhost:44342/api/roles";

    constructor(private http: HttpClient) {
    }

    ngOnInit(): void {
    
    }

     isAdminRole():boolean{
      if (this.myGetRelos()!=null){
         if (this.myGetRelos().includes("admin")){
          return true;
               }
               else{
                  return false;
               }
             }
               else{
                  return false;
               }
     }

     isEmployeeRole():boolean{
      if (this.myGetRelos()!=null){
         if (this.myGetRelos().includes("employee")){
          return true;
               }
               else{
                  return false;
               }
             }
               else{
                  return false;
               }
     }

     isAdminOrEmployeeRole():boolean{
        if (this.isAdminRole() || this.isEmployeeRole()){
           return true;
        }
        else{
           return false;
        }
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

     getRoles(){
      return this.http.get(this.url);
     }

     getUserRoles(id:string){
      return this.http.get(this.url + '/' + id);
     }

     editRoles(userChangeRoles:UserChangeRoles){
      const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
      return this.http.put(this.url, JSON.stringify(userChangeRoles), {headers:myHeaders, observe: 'response'});
     }
}