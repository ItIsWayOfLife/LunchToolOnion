import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Router } from "@angular/router";

import {LoginModel} from '../account/login/LoginModel';
import {RegisterModel} from '../account/register/registerModel';
import {ProfileModel} from '../account/profile/profileModel';
import {ChangePasswordModel} from '../account/profile/changePasswordModel';
import { chainedInstruction } from '@angular/compiler/src/render3/view/util';

@Injectable()
export class RolesService{

    constructor(private router: Router, private http: HttpClient) {

     }

     MyGetRelos():Array<string>{
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
}