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

  isView:boolean;

  editedProvider: Provider;
  isNewRecord: boolean;

  isAdminMyRole:boolean;

  searchSelectionString:string;
  searchStr:string;

  isShowStatusMessage:boolean;
  statusMessage: string;


  fileName:string;
 

  constructor(private providerServ: ProviderService, private rolesServ: RolesService) {

    this.fileName ="";

    this.providers = new Array<Provider>();

    this.isNewRecord = false;
    this.editedProvider = new Provider();

    this.isView = true;

    this.titleProviders = "Поставщики";
    this.titleFavoriteProviders  = "Популярные поставщики"
    this.isFavoriteProviders = false;

    this.searchSelectionString="Поиск по";
    this.searchStr="";

    this.isShowStatusMessage = false;
   }

  ngOnInit(): void {
    this.loadProviders();
    this.isAdminMyRole = this.rolesServ.isAdminOrEmployeeRole();
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
  this.editedProvider = new Provider();
  this.providers.push(this.editedProvider);
  this.isView = false;
  this.isNewRecord = true;
  
}

deleteProvider(id:number){
  this.isShowStatusMessage = true;
 this.providerServ.deleteProvider(id).subscribe(response=>{
  if (response.status==200)
  {
    this.statusMessage = 'Данные успешно удалены';   
    this.loadProviders();
  }else{
    this.statusMessage = 'Ошибка удаления';
  }
},err=>{
console.log(err);
this.statusMessage = 'Ошибка удаления';
  });
}

// show info
showStatusMess(){
  this.isShowStatusMessage=!this.isShowStatusMessage;
  }

  saveProvider(){
    this.isShowStatusMessage=true;
    this.editedProvider.path =this.fileName;
    if (this.isNewRecord) {
    this.providerServ.createProvider(this.editedProvider).subscribe(response => {
                   
      this.loadProviders();  
      this.statusMessage = 'Данные успешно добавлены';
      
      console.log(response.status);
    }           
    ,err=>{
    this.statusMessage = 'Ошибка при добавлении данных';
    console.log(err);
  }       
  );
} else {
  // изменяем пользователя
  // this.providerServ(this.editedProvider).subscribe(response => {
    
  //     this.loadProviders();
  //     this.statusMessage = 'Данные успешно изменены';
  //     console.log(response.status);
  // },
  // err=>{
  //   this.statusMessage = 'Ошибка при изменении данных';
  //   console.log(err);
  // });      
}
  }

  cancel(){
    // если отмена при добавлении, удаляем последнюю запись
    if (this.isNewRecord) {
      this.providers.pop();
  }
  this.isNewRecord = false;
  this.isView = true;
  this.editedProvider = null;
    this.fileName ="";
  // this.isEdit= false;
  }

  onNotify(message:string):void {
    console.log(message);
   
    this.fileName= message;
  }

 

}