import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

import {RolesService} from '../../service/roles.service'

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  providers:[RolesService]
})
export class HeaderComponent implements OnInit {

  myRoles:Array<string>;
  isAmdim:boolean;

  constructor(private jwtHelper: JwtHelperService, private router: Router, private rolesServ: RolesService) {
    this.myRoles = rolesServ.myGetRelos();
    if (this.myRoles!=null){
    if (this.myRoles.includes("admin")){
      this.isAmdim=true;
          }
          else{
            this.isAmdim=false;
          }
        }
          else{
            this.isAmdim=false;
          }
  }

  logOut() {
    localStorage.removeItem("jwt");
 }

  isUserAuthenticated() {
    let token: string = localStorage.getItem("jwt");
    
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      return true;
    }
    else {
      return false;
    }
  }

  ngOnInit(): void {

  }

}
