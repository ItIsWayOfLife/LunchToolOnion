import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Router } from "@angular/router";

import {LoginModel} from '../account/login/LoginModel';
import {RegisterModel} from '../account/register/registerModel';


@Injectable()
export class AccountService{

    constructor(private router: Router, private http: HttpClient) { }

    login(model: LoginModel) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        let credentials = JSON.stringify(model);

      return  this.http.post("https://localhost:44342/api/account/login", credentials, {headers: myHeaders});
    }

    register(model:RegisterModel){
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");  
           let credentials = JSON.stringify(model);
           
           return this.http.post("https://localhost:44342/api/account/register", credentials,
           {headers:myHeaders, observe: 'response' });
    }
    
    getProfile(){
    return this.http.get("https://localhost:44342/api/account/profile");
  }
}