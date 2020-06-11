import { Component, OnInit } from '@angular/core';

import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Router } from "@angular/router";

import {ProfileModel} from './profileModel';
import {AccountService} from '../../service/account.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  providers: [AccountService]
})
export class ProfileComponent implements OnInit {

 profileModel:ProfileModel;

  constructor(private serv: AccountService) {
    this.profileModel = new ProfileModel();
   }

  ngOnInit(): void {
    this.serv.getProfile().subscribe((data:ProfileModel) => {
      this.profileModel = data;
    },
    err=>{
      console.log(err);
    }
    );
  }

}
