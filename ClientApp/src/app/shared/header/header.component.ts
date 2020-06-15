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

 
  isAmdim:boolean;

  constructor(private jwtHelper: JwtHelperService, private router: Router, private rolesServ: RolesService) {
    this.isAmdim = this.rolesServ.isAdminRole();
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
