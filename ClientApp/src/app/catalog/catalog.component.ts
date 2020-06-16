import { Component, OnInit } from '@angular/core';
import { ActivatedRoute} from '@angular/router';

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

  isAmdim:boolean;

  editedCatalog: Catalog;
  catalogs: Array<Catalog>;

  isShowStatusMessage:boolean;
  statusMessage: string;

  nameProvider:string;

  constructor(private activateRoute: ActivatedRoute,
     private catalogServ :CatalogService,
     private rolesServ:RolesService,
     private providerServ:ProviderService){
      this.providerId = activateRoute.snapshot.params['providerId'];

      this.catalogs = new Array<Catalog>();
      this.editedCatalog = new Catalog();
  }

  ngOnInit(): void {
    this.loadCatalog();
    this.getNameProvider();
  }

  getNameProvider(){
    this.providerServ.getPrivoder(this.providerId).subscribe((data:Provider)=>
    {
      this.nameProvider=data.name;
    });
  }

  // load users
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
  // this.isNewRecord = false;
  // this.isView = true;
  // this.isEdit= false;
  }

}
