import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";

import {RegisterModel} from './registerModel';
import {AccountService} from '../../service/account.service';
import {LoginModel} from '../login/LoginModel';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  providers: [AccountService]
})
export class RegisterComponent implements OnInit {

  statusCode:number;
  statusOk:boolean ;
  statusNotOk:boolean ;

  invalidLogin: boolean;
  registerModel:RegisterModel;
  newLogModel: LoginModel;
  ngOnInit(): void {
  }

  constructor(private router: Router, private serv: AccountService) {
    this.registerModel= new RegisterModel();
    this.statusOk=false;
    this.statusNotOk = false;
   }

  register() {   
  this.serv.register(this.registerModel)
      .subscribe(response => {

        this.statusCode = response.status;

       if (response.status== 200){
        this.statusOk = true;

        this.newLogModel = new LoginModel();
        this.newLogModel.userName = this.registerModel.email;
        this.newLogModel.password = this.registerModel.password;
        this.serv.login(this.newLogModel).subscribe(response => {
          let token = (<any>response).token;
          localStorage.setItem("jwt", token);
          this.invalidLogin = false;
        }, err => {
          this.invalidLogin = true;
          console.log(err.message);
        });;
       }
       else{
        this.statusOk =  false;
       }
       
        // Or any other header:
        console.log("Code: "+response.status);
      },
      err => {
        this.statusNotOk = true;
        console.log("Code: "+err.status);
        console.log(err.message);
       } ); 
    }

    Ok(){
      this.router.navigate(["/"]);
    }
}