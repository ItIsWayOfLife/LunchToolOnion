import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from "@angular/router";
import { NgForm } from '@angular/forms';

export class RegisterModel{
        public email : string;
        public firstname :string;
        public lastname :string;
        public patronymic :string;
        public password :string;
        public passwordConfirm :string;
}


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  statusMessage: string;
  registerModel:RegisterModel = new RegisterModel();

  ngOnInit(): void {
  }

  constructor(private http: HttpClient) { }
  register() {
    const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
    console.log(this.registerModel.email +" " +this.registerModel.password+" "+this.registerModel.patronymic);
   return this.http.post("https://localhost:44342/api/account/register", JSON.stringify(this.registerModel),
     {headers:myHeaders}).subscribe(data => {
      this.statusMessage = '?'
  });
   
}

}
