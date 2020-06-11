import { Component, OnInit } from '@angular/core';

import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Router } from "@angular/router";

import {ProfileModel} from './profileModel';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

 profileModel:ProfileModel;

 token:string = localStorage.getItem("jwt");

  constructor(private http: HttpClient) {
    this.profileModel = new ProfileModel();
    this.profileModel.firstname = "ww";
    this.profileModel.email = "fff";

   }

  ngOnInit(): void {
    this.http.get("https://localhost:44342/api/account/profile").subscribe((data:ProfileModel) => {
      this.profileModel = data;
    },
    err=>{
      console.log(err);
    }
    );
  }

}
