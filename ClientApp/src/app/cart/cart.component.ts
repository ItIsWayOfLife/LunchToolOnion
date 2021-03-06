import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from "@angular/router";
import { Title } from '@angular/platform-browser';

import { CartService } from '../service/cart.service';
import { OrderService } from '../service/order.service';

import { CartDishes } from './cartDishes';
import { CartDishesUpdate } from './cartDishesUpdate';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
  providers: [CartService,
    OrderService]
})
export class CartComponent implements OnInit {

  isShowStatusMessage: boolean;
  statusMessage: string;
  isMessInfo: boolean;

  cartDishes: Array<CartDishes>;
  updateCart: Array<CartDishesUpdate>;

  constructor(private cartServ: CartService,
    private _location: Location,
    private orderServ: OrderService,
    private router: Router,
    private titleService: Title) {

    this.titleService.setTitle('Корзина');

    this.cartDishes = new Array<CartDishes>();
    this.updateCart = new Array<CartDishesUpdate>();
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

  updateData() {
    this.updateCart = new Array<CartDishesUpdate>();
    for (let c of this.cartDishes) {
      let cdu: CartDishesUpdate = new CartDishesUpdate();
      cdu.id = c.id;
      cdu.count = c.count;
      this.updateCart.push(cdu);
    }
    this.cartServ.updateCart(this.updateCart).subscribe(response => {
      this.loadCart();
      this.statusMessage = 'Корзина успешно обновлена';
      this.isMessInfo = true;
      this.isShowStatusMessage = true;
    }, err => {    
        this.statusMessage = 'Ошибка при обновлении корзины';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
   
    this.getFullPrice();
  }

  getTotalPrice(cartD: CartDishes) {
    return (cartD.count * cartD.price).toFixed(2);
  }

  getFullPrice() {
    let fullPrice: number = 0;

    for (let c of this.cartDishes) {
      fullPrice += Number(this.getTotalPrice(c));
    }

    return fullPrice.toFixed(2);
  }

  // show info
  showStatusMess() {
    this.isShowStatusMessage = !this.isShowStatusMessage;
  }

  deleteCartDish(id: number) {
    this.cartServ.deleteDishInCart(id).subscribe(response => {
      this.loadCart();
      this.statusMessage = 'Данные успешно удалены';
      this.isShowStatusMessage = true;
      this.isMessInfo = true;
    }, err => {
        this.statusMessage = 'Ошибка удаления';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
  }

  deleteAllDishesInCart() {
    this.cartServ.deleteAllDishesInCart().subscribe(response => {
      this.loadCart();
      this.statusMessage = 'Корзина успешно очищена';
      this.isMessInfo = true;
      this.isShowStatusMessage = true;
    }, err => {   
        this.statusMessage = 'Ошибка при очистке корзины';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
  }

  createOrder() {
    this.orderServ.createOrder().subscribe(response => {
      this.router.navigate(["/order"]);
    }, err => {
        this.statusMessage = 'Ошибка при создании заказа';
        if (typeof err.error == 'string') {
          this.statusMessage += '. ' + err.error;
        }
        this.isMessInfo = false;
        this.isShowStatusMessage = true;
        console.log(err);
      });
  }

}
