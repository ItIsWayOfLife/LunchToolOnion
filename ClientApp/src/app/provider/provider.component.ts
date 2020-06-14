import { Component, OnInit } from '@angular/core';

import {ProviderService} from '../service/providerService';

import {Provider} from './provider';

@Component({
  selector: 'app-provider',
  templateUrl: './provider.component.html',
  styleUrls: ['./provider.component.css'],
  providers:[ProviderService]
})
export class ProviderComponent implements OnInit {

  providers: Array<Provider>;

  constructor(private providerServ: ProviderService) {
    this.providers = new Array<Provider>();
   }

  ngOnInit(): void {
    this.loadProviders();
  }

// load users
loadProviders() {
  this.providerServ.getProviders().subscribe((data: Provider[]) => {
          this.providers = data; 
      });
}

}
