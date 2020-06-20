import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {Menu} from '../menu/menu';


@Injectable()
export class MenuCompilationService{

    menuId:number;

    constructor(){
        this.menuId = 0;
    }

    appointMenuId(id:number){
        this.menuId = id;
        console.log("servMenId: "+this.menuId);
    }

    isComplition():boolean{
        if (this.menuId==0){
           return false;
        }
        else{
            return true;
        }
    }

}