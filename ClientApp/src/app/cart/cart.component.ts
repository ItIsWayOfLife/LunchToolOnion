import { Component, OnInit } from '@angular/core';
import {Location} from '@angular/common';
import { Router } from "@angular/router";

import {CartService} from '../service/cart.service';
import {OrderService} from '../service/order.service';

import {CartDishes} from './cartDishes';
import {CartDishesUpdate} from './cartDishesUpdate';


@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
  providers:[CartService,
    OrderService]
})
export class CartComponent implements OnInit {

  isShowStatusMessage:boolean;
  statusMessage: string;

  cartDishes:Array<CartDishes>;
  updateCart:Array<CartDishesUpdate>;

  constructor(private cartServ:CartService,
    private _location: Location,
    private orderServ:OrderService,
    private router: Router,) {
   this.cartDishes = new Array<CartDishes>();
  this.updateCart =new Array<CartDishesUpdate>();
  this.isShowStatusMessage = false;
   }

   backClicked() {
    this._location.back();
}

  ngOnInit(): void {
    this.loadCart();
    this.getFullPrice();
  }

  // load dishes
  loadCart() {
    this.cartServ.getDishInCart().subscribe((data: CartDishes[]) => {
            this.cartDishes = data; 
        });
}

getInfoForView():boolean{
  if (this.cartDishes.length>0){
    return true;
  }
  else{
    return false;
  }
}

updateData(){
  this.updateCart =new Array<CartDishesUpdate>();
  for (let c of this.cartDishes){
    let cdu:CartDishesUpdate = new CartDishesUpdate();
    cdu.id =c.id;
    cdu.count = c.count;
    this.updateCart.push(cdu);
  }

  this.cartServ.updateCart(this.updateCart).subscribe(response => {
                     
    this.loadCart();  
    this.statusMessage = 'Корзина успешно обновлена';
    
    console.log(response.status);
  }           
  ,err=>{
    console.log(err);
  this.statusMessage = 'Ошибка при обновлении корзины';
}       
);
this.isShowStatusMessage = true; 

this.getFullPrice();
}

getTotalPrice(cartD:CartDishes){
  return (cartD.count * cartD.price).toFixed(2);
}

getFullPrice(){
 let fullPrice: number =0;

 for (let c of this.cartDishes){
 fullPrice += Number(this.getTotalPrice(c));
 }

 return fullPrice.toFixed(2);
}

// show info
showStatusMess(){
  this.isShowStatusMessage=!this.isShowStatusMessage;
  }

  deleteCartDish(id:number){  
    this.cartServ.deleteDishInCart(id).subscribe(response => {

      if (response.status==200)
      {
        this.statusMessage = 'Данные успешно удалены';   
        this.loadCart();
      }else{
        this.statusMessage = 'Ошибка удаления';
      }
    },err=>{
console.log(err);
this.statusMessage = 'Ошибка удаления';
      }
);

this.isShowStatusMessage = true; 
}

deleteAllDishesInCart(){
  this.cartServ.deleteAllDishesInCart().subscribe(response => {

    if (response.status==200)
    {
      this.statusMessage = 'Корчина успешно очищена';   
      this.loadCart();
    }else{
      this.statusMessage = 'Ошибка при очистке корзины';
    }
  },err=>{
console.log(err);
this.statusMessage = 'Ошибка при очистке корзины';
    }
  );

  this.isShowStatusMessage = true; 
}

createOrder(){
this.orderServ.createOrder().subscribe(response=>
  {
    this.router.navigate(["/order"]);
  },err=>
  {
    console.log(err);

    this.statusMessage = 'Ошибка при создании заказа';
    this.isShowStatusMessage = true; 
  });
}

}
