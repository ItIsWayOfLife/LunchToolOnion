import { Component, OnInit } from '@angular/core';

import {ProviderService} from '../service/providerService';
import {RolesService} from '../service/roles.service';

import {Provider} from './provider';

@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css'],
  providers:[ProviderService, RolesService]
})
export class ProviderComponent implements OnInit {

  providers: Array<Provider>;

  titleProviders:string;
  titleFavoriteProviders:string;;
  isFavoriteProviders:boolean;

  isAdminOrEmplooyeMyRole:boolean;

  searchSelectionString:string;
  searchStr:string;

  constructor(private providerServ: ProviderService, private rolesServ: RolesService) {
    this.providers = new Array<Provider>();

    this.titleProviders = "Поставщики";
    this.titleFavoriteProviders  = "Популярные поставщики"
    this.isFavoriteProviders = false;

    this.searchSelectionString="Поиск по";
    this.searchStr="";
   }

  ngOnInit(): void {
    this.loadProviders();
    this.isAdminOrEmplooyeMyRole = this.rolesServ.isAdminOrEmployeeRole();
  }


// load users
loadProviders() {
  this.providerServ.getProviders().subscribe((data: Provider[]) => {
          this.providers = data; 
      });
}

getMenu(providerId:number){

}

getCatalogDishes(providerId:number){

}

getFavoriteProviders(){
  this.isFavoriteProviders = true;
}

getAllProviders(){
  this.isFavoriteProviders = false;
}

getProviders():Array<Provider>{
  if ( this.isFavoriteProviders){
return this.providers.filter(p=>p.isFavorite);
  }
  else{
    if (this.searchSelectionString=="Id"){
      return this.providers.filter(x=>x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Названию"){
      return this.providers.filter(x=>x.name.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Email"){
      return this.providers.filter(x=>x.email.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Рабочим дням"){
      return this.providers.filter(x=>x.workingDays.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Времени работы с"){
      return this.providers.filter(x=>x.timeWorkWith.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Времени работы до"){
      return this.providers.filter(x=>x.timeWorkTo.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Статусу"){
      let activeStr = "Работает";
      let notActiveStr = "Не работает";
      if (activeStr.toLowerCase().startsWith(this.searchStr.toLowerCase()) && (this.searchStr!="")){
        return this.providers.filter(x=>x.isActive);
      }
      else if(notActiveStr.toLowerCase().startsWith(this.searchStr.toLowerCase())  && (this.searchStr!="")){
        return this.providers.filter(x=>x.isActive==false);
      }
    }
    else if (this.searchSelectionString=="Информации"){
      return this.providers.filter(x=>x.email.toLowerCase().includes(this.searchStr.toLowerCase()));
    }

  return this.providers;
  }
}

refresh(){
  this.searchSelectionString="Поиск по";
  this.searchStr ="";
}

addProvider(){

}

}
