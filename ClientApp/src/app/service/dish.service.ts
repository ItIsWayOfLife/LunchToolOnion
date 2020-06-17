import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {Dish} from '../dish/dish';


@Injectable()
export class DishService{

    private url = "https://localhost:44342/api/dish";

    constructor(private http: HttpClient) {}

    getDishByCatalogId(catalogId:number){
        return this.http.get(this.url + '/catalog/' + catalogId);
    }

    deleteDish(id: number){
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.delete(this.url + '/' + id, {headers:myHeaders, observe: 'response'});
    }

    createMenu(dish: Dish){
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.post(this.url, JSON.stringify(dish), {headers: myHeaders, observe: 'response'}); 
    }

    updateMenu(dish: Dish) {
        const myHeaders = new HttpHeaders().set("Content-Type", "application/json");
        return this.http.put(this.url, JSON.stringify(dish), {headers:myHeaders, observe: 'response'});
    }
}