export class User{
    public id : string;
    public firstname : string;
    public lastname : string;
    public patronymic : string;
    public email : string;
    public password:string;

 public getFLP():string{
        return this.firstname +" "+this.lastname+" "+this.patronymic;
    }
}