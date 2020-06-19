import { Component, OnInit } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';

import {DishService} from '../service/dish.service';
import {RolesService} from '../service/roles.service';
import {CatalogService} from '../service/catalog.service';

import {Dish} from './dish';
import { Catalog } from '../catalog/catalog';

@Component({
  selector: 'app-dish',
  templateUrl: './dish.component.html',
  styleUrls: ['./dish.component.css'],
  providers:[RolesService,
    DishService,
    CatalogService]
})
export class DishComponent implements OnInit {

  catalogId: number;

  isAdminMyRole:boolean;

  editedDish: Dish;
  dishes: Array<Dish>;

  isShowStatusMessage:boolean;
  statusMessage: string;

  nameCatalog:string;

  searchSelectionString:string;
  searchStr:string;

  isEdit:boolean;
  isView:boolean;
  isNewRecord: boolean;

  fileName:string;

  constructor(private activateRoute: ActivatedRoute,
    private dishServ :DishService,
    private rolesServ:RolesService,
    private catalogServ:CatalogService,
    private _location: Location) { 

      
    this.fileName ="";
    this.catalogId = activateRoute.snapshot.params['catalogId'];

    this.dishes = new Array<Dish>();
    this.editedDish = new Dish();

    this.isShowStatusMessage = false;

    this.searchSelectionString="Поиск по";
    this.searchStr="";

    this.isView=true;
    this.isEdit = false;
    this.isNewRecord = false;
  }

  backClicked() {
    this._location.back();
}

  ngOnInit(): void {
    this.loadDishes();
    this.getNameCatalog();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
  }

  getNameCatalog(){
    this.catalogServ.getCatalog(this.catalogId).subscribe((data:Catalog)=>
    {
      this.nameCatalog=data.name;
    });
  }

    // load dishes
    loadDishes() {
      this.dishServ.getDishByCatalogId(this.catalogId).subscribe((data: Dish[]) => {
              this.dishes = data; 
          });
  }

  // delete dish
deleteDish(id: number) {
  this.isShowStatusMessage=true;
    this.dishServ.deleteDish(id).subscribe(response => {

      if (response.status==200)
      {
        this.statusMessage = 'Данные успешно удалены';   
        this.loadDishes();
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

  addDish(){
    this.editedDish = new Dish();
    this.dishes.push(this.editedDish);
    this.isView = false;
    this.isNewRecord = true;
  }

  // edit menu
editDish(dish: Dish) {
  this.isEdit = true;
  this.isView = false;
 this.editedDish = dish;
}

refresh(){
  this.searchSelectionString="Поиск по";
  this.searchStr ="";
}

getDishes():Array<Dish>{
  if (this.searchSelectionString=="Id"){
    return this.dishes.filter(x=>x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
  }
  else if (this.searchSelectionString=="Названию"){
    return this.dishes.filter(x=>x.name.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
  }
  else if (this.searchSelectionString=="Информации"){
    return this.dishes.filter(x=>x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
  }
  else if (this.searchSelectionString=="Весу"){
    return this.dishes.filter(x=>x.weight.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
  }
  else if (this.searchSelectionString=="Цене"){
    return this.dishes.filter(x=>x.price.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
  }

return this.dishes;
}


saveDish(){
  this.isShowStatusMessage=true;
  this.editedDish.catalogId =Number(this.catalogId);
 
  if (this.isNewRecord) {
    this.editedDish.path =this.fileName;
  this.dishServ.createMenu(this.editedDish).subscribe(response => {
                 
    this.loadDishes();  
    this.statusMessage = 'Данные успешно добавлены';
    
    console.log(response.status);
  }           
  ,err=>{
  this.statusMessage = 'Ошибка при добавлении данных';
  this.dishes.pop();
  console.log(err);
}       
);
} else {

  if (this.fileName!=""){
    this.editedDish.path =this.fileName;
  }
 
// edit catalog
this.dishServ.updateMenu(this.editedDish).subscribe(response => {
  
    this.loadDishes();
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
    this.dishes.pop();
}
this.isNewRecord = false;
this.isView = true;
this.editedDish = null;
 this.isEdit= false;
}

onNotify(message:string):void {
  console.log(message);
 
  this.fileName= message;
}

}


