import { Component, OnInit } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';

import {MenuService} from '../service/menu.service';
import {RolesService} from '../service/roles.service';
import {ProviderService} from '../service/provider.service';

import {Provider} from '../provider/provider';
import {Menu} from './menu';


@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
  providers:[RolesService,
    MenuService,
    ProviderService]
})
export class MenuComponent implements OnInit {

  providerId: number;

  isAdminMyRole:boolean;

  editedMenu: Menu;
  menus: Array<Menu>;

  isShowStatusMessage:boolean;
  statusMessage: string;

  nameProvider:string;

  searchSelectionString:string;
  searchStr:string;

  editedDate:string;

  isEdit:boolean;
  isView:boolean;
  isNewRecord: boolean;

  constructor(private activateRoute: ActivatedRoute,
     private menuServ :MenuService,
     private rolesServ:RolesService,
     private providerServ:ProviderService,
     private _location: Location) {

    this.providerId = activateRoute.snapshot.params['providerId'];

    this.menus = new Array<Menu>();
    this.editedMenu = new Menu();

    this.isShowStatusMessage = false;

    this.searchSelectionString="Поиск по";
    this.searchStr="";

    this.isView=true;
    this.isEdit = false;
    this.isNewRecord = false;

    this.editedDate = "";
   }

   backClicked() {
    this._location.back();
}

getEditDate(){
  let eDateMenu:string = this.editedMenu.date;
  
  let eDateDay = eDateMenu.substr(0,2);
  let eDateMonth = eDateMenu.substr(3,2);
  let eDateYear = eDateMenu.substr(6,4);
  
  console.log("day: "+eDateDay);
  console.log("month: "+eDateMonth);
  console.log("year: "+eDateYear);

  console.log(eDateYear+"-"+eDateMonth+"-"+eDateDay);
this.editedMenu.date = eDateYear+"-"+eDateMonth+"-"+eDateDay;
}

  ngOnInit(): void {
    this.loadMenus();
    this.getNameProvider();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
  }

  getNameProvider(){
    this.providerServ.getPrivoder(this.providerId).subscribe((data:Provider)=>
    {
      this.nameProvider=data.name;
    });
  }

   // load menus
   loadMenus() {
    this.menuServ.getMenuByProviderId(this.providerId).subscribe((data: Menu[]) => {
            this.menus = data; 
        });
}

// delete menu
deleteMenu(id: number) {
  this.isShowStatusMessage=true;
    this.menuServ.deleteMenu(id).subscribe(response => {

      if (response.status==200)
      {
        this.statusMessage = 'Данные успешно удалены';   
        this.loadMenus();
      }else{
        this.statusMessage = 'Ошибка удаления';
      }
    },err=>{
console.log(err);
this.statusMessage = 'Ошибка удаления';
      }
);
}

// show info
showStatusMess(){
  this.isShowStatusMessage=!this.isShowStatusMessage;
  this.isNewRecord = false;
  this.isView = true;
  this.isEdit= false;
  }

  addMenu(){
    this.editedMenu = new Menu();
    this.menus.unshift(this.editedMenu);
    this.isView = false;
    this.isNewRecord = true;
  }

// edit menu
editMenu(menu: Menu) {
  this.isEdit = true;
  this.isView = false;
 this.editedMenu = menu;
 this.getEditDate();
}

  refresh(){
    this.searchSelectionString="Поиск по";
    this.searchStr ="";
  }

  getMenus():Array<Menu>{
    if (this.searchSelectionString=="Id"){
      return this.menus.filter(x=>x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Дате"){
      return this.menus.filter(x=>x.date.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
    }
    else if (this.searchSelectionString=="Информации"){
      return this.menus.filter(x=>x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
    }
  
  return this.menus;
  }

  saveMenu(){
    this.isShowStatusMessage=true;
    this.editedMenu.providerId =Number(this.providerId);
   
    if (this.isNewRecord) {
    this.menuServ.createMenu(this.editedMenu).subscribe(response => {
                   
      this.loadMenus();  
      this.statusMessage = 'Данные успешно добавлены';
      
      console.log(response.status);
    }           
    ,err=>{
    this.statusMessage = 'Ошибка при добавлении данных. '+err.error;
    this.menus.pop();
    console.log(err);
  }       
  );
} else {
   
 // edit catalog
  this.menuServ.updateMenu(this.editedMenu).subscribe(response => {
    
      this.loadMenus();
      this.statusMessage = 'Данные успешно изменены';
      console.log(response.status);
  },
  err=>{
    this.statusMessage = 'Ошибка при изменении данных';
    console.log(err);
  });  
}
  }

  cancel(){
    // если отмена при добавлении, удаляем последнюю запись
    if (this.isNewRecord) {
      this.menus.pop();
  }
  this.isNewRecord = false;
  this.isView = true;
  this.editedMenu = null;
   this.isEdit= false;
  }

}
