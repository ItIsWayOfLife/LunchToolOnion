import { Component, OnInit, DoCheck } from '@angular/core';

import {OrderService} from '../service/order.service';

import {Order} from './order';
import {OrderDishes} from './OrderDishes';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css'],
  providers:[OrderService]
})
export class OrderComponent implements OnInit,
DoCheck{

  orders:Array<Order>;
  orderDishes:Array<OrderDishes>;

  isViewOrders:boolean;
  isViewOrdersList:boolean;

  idOrderViewDishes:number;

  constructor(private orderServ:OrderService) {
    this.orders = new Array<Order>();
  
    this.isViewOrders  = true;
    this.isViewOrdersList = true;
    
    this.orderDishes = new Array<OrderDishes>();
   }

  ngOnInit(): void {
   this.loadOrders();
  }

  ngDoCheck() {
  }

    // load menus
    loadOrders() {
      this.orderServ.getOrder().subscribe((data: Order[]) => {
              this.orders = data; 
          });
  }

  getInfoDish(id:number){
    this.orderDishes = new Array<OrderDishes>();
  this.orderServ.getOrderDishById(id).subscribe((data: OrderDishes[]) => {
    this.orderDishes = data; 
    this.idOrderViewDishes = data[0].orderId;
});
    this.isViewOrdersList = false;
  }

  checkOrderView():boolean{
    if (this.orders.length>0){
      return true;
    }
    else{
      return false;
    }
  }

  getTotalPrice(orderD:OrderDishes){
    return (orderD.count * orderD.price).toFixed(2);
  }

  getFullPrice():number{
    return this.orders.find(p=>p.id== this.idOrderViewDishes).fullPrice;
  }

  back(){
    this.isViewOrdersList = true;
  }

}
