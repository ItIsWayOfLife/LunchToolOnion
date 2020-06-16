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

}
