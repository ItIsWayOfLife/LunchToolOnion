import { Component, OnInit } from '@angular/core';
import { Input} from '@angular/core'; 

import {ProfileModel} from './profileModel';
import {ChangePasswordModel} from './changePasswordModel';
import {AccountService} from '../../service/account.service';
import { ThrowStmt } from '@angular/compiler';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  providers: [AccountService]
})
export class ProfileComponent implements OnInit {

 profileModel:ProfileModel;
 editProfileModel:ProfileModel;

 isEditProfile:boolean;
 isStatusEditGood:boolean;
 isStatusEditBad:boolean;

 changePasswordModel:ChangePasswordModel;

 isEditPass:boolean;
 isStatusEditPassGood:boolean;
 isStatusEditPassBad:boolean;

  constructor(private serv: AccountService) {
    this.profileModel = new ProfileModel();
    this.editProfileModel = new ProfileModel();

    this.isEditProfile = false;
    this.isStatusEditGood=false;
    this.isStatusEditBad = false;

    this.changePasswordModel = new ChangePasswordModel();

    this.isEditPass =false;
    this.isStatusEditPassGood= false;
    this.isStatusEditPassBad = false;
   }

  ngOnInit(): void {


    this.serv.getProfile().subscribe((data:ProfileModel) => {
      this.profileModel = data;
      this.editProfileModel =data;
    },
    err=>{
      console.log(err);
    }
    );
  }

  editProfileShow(){
    this.isEditProfile =!this.isEditProfile;
    if (this.isEditPass){
      this.isEditPass = false;
    }
  }

  editProfile(){ 
    this.serv.editProfile(this.editProfileModel).subscribe(response => {
     
      if (response.status==200){
        this.isStatusEditGood  = true;
        this.profileModel=this.editProfileModel;
        this.isStatusEditBad = false;
      }
      else{
        this.isStatusEditBad = true;
      }
    },  err => {
      console.log(err);
      this.isStatusEditBad = true;
    }
    );
  }

  OkEdit(){
    this.isStatusEditGood = !this.isStatusEditGood;
  }

  editPassShow(){
this.isEditPass=!this.isEditPass;
if (this.isEditProfile){
  this.isEditProfile = false;
}
}
OkEditPass(){
  this.isStatusEditPassGood = !this.isStatusEditPassGood;
}

editPass(){

  console.log("ww "+this.changePasswordModel.newPassword +" "+ this.changePasswordModel.oldPassword);

  this.serv.editPass(this.changePasswordModel).subscribe(response=>{
    if (response.status==200){
      this.isStatusEditPassGood  = true;
      this.isStatusEditPassBad = false;
    }
    else{
      this.isStatusEditPassBad = true;
    }
  },err => {
    console.log(err);
    this.isStatusEditPassBad = true;
  }
  );
}
}
