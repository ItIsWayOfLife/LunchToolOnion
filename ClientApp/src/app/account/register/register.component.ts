import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HttpClientModule } from '@angular/common/http';

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

  statusCode:number;
  statusOk:boolean = false;
  statusNotOk:boolean = false;
  registerModel:RegisterModel = new RegisterModel();

  ngOnInit(): void {
  }

  constructor(private http: HttpClient) { }
  register() {
    const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
   
   return this.http.post("https://localhost:44342/api/account/register", JSON.stringify(this.registerModel),
     {headers:myHeaders, observe: 'response' })
      .subscribe(response => {

        this.statusCode = response.status;

       if (response.status== 200){
        this.statusOk = true;
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
}
