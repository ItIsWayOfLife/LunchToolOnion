import { Component, OnInit } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';

import {CatalogService} from '../service/catalog.service';
import {RolesService} from '../service/roles.service';
import {ProviderService} from '../service/providerService';

import {Provider} from '../provider/provider';
import {Catalog} from './catalog';

@Component({
  selector: 'app-catalog',
  templateUrl: './catalog.component.html',
  styleUrls: ['./catalog.component.css'],
  providers:[RolesService,
     CatalogService,
     ProviderService]
})
export class CatalogComponent implements OnInit {

  providerId: number;

  isAdminMyRole:boolean;

  editedCatalog: Catalog;
  catalogs: Array<Catalog>;

  isShowStatusMessage:boolean;
  statusMessage: string;

  nameProvider:string;

  searchSelectionString:string;
  searchStr:string;

  isEdit:boolean;
  isView:boolean;
  isNewRecord: boolean;

  constructor(private activateRoute: ActivatedRoute,
     private catalogServ :CatalogService,
     private rolesServ:RolesService,
     private providerServ:ProviderService,
     private _location: Location){
      this.providerId = activateRoute.snapshot.params['providerId'];

      this.catalogs = new Array<Catalog>();
      this.editedCatalog = new Catalog();

      this.isShowStatusMessage = false;

      this.searchSelectionString="Поиск по";
      this.searchStr="";

      this.isView=true;
      this.isEdit = false;
      this.isNewRecord = false;
  }

  ngOnInit(): void {
    this.loadCatalog();
    this.getNameProvider();
    this.isAdminMyRole = this.rolesServ.isAdminRole();
  }

  backClicked() {
    this._location.back();
}

  getNameProvider(){
    this.providerServ.getPrivoder(this.providerId).subscribe((data:Provider)=>
    {
      this.nameProvider=data.name;
    });
  }

  // load catalog
  loadCatalog() {
    this.catalogServ.getCatalogsByProviderId(this.providerId).subscribe((data: Catalog[]) => {
            this.catalogs = data; 
        });
}

// delete catalog
deleteCatalog(id: number) {
  this.isShowStatusMessage=true;
    this.catalogServ.deleteCatalog(id).subscribe(response => {

      if (response.status==200)
      {
        this.statusMessage = 'Данные успешно удалены';   
        this.loadCatalog();
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
  addCatalog(){
    this.editedCatalog = new Catalog();
    this.catalogs.push(this.editedCatalog);
    this.isView = false;
    this.isNewRecord = true;
  }

// edit catalog
editCatalog(catalog: Catalog) {
  this.isEdit = true;
  this.isView = false;
 this.editedCatalog = catalog;
}

  refresh(){
    this.searchSelectionString="Поиск по";
    this.searchStr ="";
  }

  getCatalogs():Array<Catalog>{
      if (this.searchSelectionString=="Id"){
        return this.catalogs.filter(x=>x.id.toString().toLowerCase().includes(this.searchStr.toLowerCase()));
      }
      else if (this.searchSelectionString=="Названию"){
        return this.catalogs.filter(x=>x.name.toLowerCase().includes(this.searchStr.toLowerCase()));
      }
      else if (this.searchSelectionString=="Информации"){
        return this.catalogs.filter(x=>x.info.toLowerCase().includes(this.searchStr.toLowerCase()));
      }
    
    return this.catalogs;
    }

    
    saveCatalog(){
      this.isShowStatusMessage=true;
      this.editedCatalog.providerId =Number(this.providerId);
     
      if (this.isNewRecord) {
      this.catalogServ.createCatalog(this.editedCatalog).subscribe(response => {
                     
        this.loadCatalog();  
        this.statusMessage = 'Данные успешно добавлены';
        
        console.log(response.status);
      }           
      ,err=>{
      this.statusMessage = 'Ошибка при добавлении данных';
      this.catalogs.pop();
      console.log(err);
    }       
    );
  } else {
     
   // edit catalog
    this.catalogServ.updateCatalog(this.editedCatalog).subscribe(response => {
      
        this.loadCatalog();
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
        this.catalogs.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.editedCatalog = null;
     this.isEdit= false;
    }

}
