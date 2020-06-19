import { Component, OnInit } from '@angular/core';

import {OrderService} from '../service/order.service';

import {Order} from './order';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css'],
  providers:[OrderService]
})
export class OrderComponent implements OnInit {

  orders:Array<Order>;
ischeckOrderView:boolean;

  constructor(private orderServ:OrderService) {
    this.orders = new Array<Order>();
   }

  ngOnInit(): void {
   this.loadOrders();
  }

    // load menus
    loadOrders() {
      this.orderServ.getOrder().subscribe((data: Order[]) => {
              this.orders = data; 
          });
  }

  checkOrderView():boolean{
    if (this.orders.length>0){
      return true;
    }
    else{
      return false;
    }
  }

}
