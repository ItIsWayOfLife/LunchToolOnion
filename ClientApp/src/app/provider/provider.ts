import { Data } from '@angular/router';

export class Provider{
    public id:number;
    public name:string;
    public email:string;
    public timeWorkWith:Data;
    public timeWorkTo:Data; 
    public isActive:boolean;
    public path:File;
    public workingDays:string;   
    public isFavorite:boolean;
    public info:string;
}