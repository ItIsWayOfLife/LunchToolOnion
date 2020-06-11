import { Component, OnInit } from '@angular/core';
import { Input} from '@angular/core'; 

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
 editProfileModel:ProfileModel;

 isEditProfile:boolean;
 isStatusEditGood:boolean;
 isStatusEditBad:boolean;

  constructor(private serv: AccountService) {
    this.profileModel = new ProfileModel();
    this.editProfileModel = new ProfileModel();
    this.isEditProfile = false;
    this.isStatusEditBad = false;
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
  }

  editProfile(){ 
    this.serv.editProfile(this.editProfileModel).subscribe(response => {
     
      if (response.status==200){
        this.isStatusEditGood  = true;
        this.profileModel.email=this.editProfileModel.email;
        this.profileModel.firstname=this.editProfileModel.firstname;
        this.profileModel.lastname=this.editProfileModel.lastname;
        this.profileModel.patronymic=this.editProfileModel.patronymic;
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

}
