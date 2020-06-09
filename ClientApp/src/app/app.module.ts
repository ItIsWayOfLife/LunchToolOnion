import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule }   from '@angular/forms';
import {RouterModule, Routes} from '@angular/router';

import { HttpClientModule } from '@angular/common/http';

import { JwtModule } from "@auth0/angular-jwt";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './account/login/login.component';
import { RegisterComponent } from './account/register/register.component';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';

import { AuthGuard } from './account/guards/auth-guard.service';
import { HomeComponent } from './home/home.component';
import { CustomersComponent } from './customers/customers.component';

export function tokenGetter() {
  return localStorage.getItem("jwt");
}

const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  {   path: 'login', component: LoginComponent},
  {   path: 'register', component: RegisterComponent},
  {   path: 'customers', component: CustomersComponent },
]

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HeaderComponent,
    FooterComponent,
    HomeComponent,
    CustomersComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(appRoutes),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["localhost:44350"],
        blacklistedRoutes: []
      }
    })
],
  providers: [AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }