import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';


import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: []
})
export class HomeComponent {


  url="../../assets/images/providers/777.jpeg";

  constructor(private jwtHelper: JwtHelperService, private router: Router, private http: HttpClient) {}

  onselectFile(e){
if (e.targer.files)
var reader = new FileReader();
reader.readAsDataURL(e.target.files[0]);  
reader.onload=(event:any)=>{
  this.url = event.target.result;
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

  myRedirect(){
    
  }

  test(){
    let token: string = localStorage.getItem("jwt");

    let jwtData = token.split('.')[1]
    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)
    
    var splitted = decodedJwtJsonData.substring(decodedJwtJsonData.indexOf('['),decodedJwtJsonData.indexOf(']')).slice(1); 

    var roles = splitted.split(','); 
    console.log("wwws: "+roles[0])

    console.log(splitted)

    let isAdmin = decodedJwtData.admin
    
    console.log('jwtData: ' + jwtData)
    console.log('decodedJwtJsonData: ' + decodedJwtJsonData)
    console.log('decodedJwtData: ' + decodedJwtData)
    console.log('Is admin: ' + isAdmin)

  }

}
