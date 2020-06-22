import { Component, OnInit} from '@angular/core';

import {RolesService} from '../../service/roles.service';
import {UserService} from '../../service/user.service';

import {User} from './user';
import {UserChangePassword} from './userChangePassword';
import {UserChangeRoles} from './userChangeRoles';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  providers:[RolesService, UserService]
})
export class UsersComponent implements OnInit {

    isAmdim:boolean;

    editedUser: User;
    users: Array<User>;
    isView:boolean;
    isNewRecord: boolean;
    isEdit:boolean;
  
    isShowStatusMessage:boolean;
    
    statusMessage: string;

    userChangePassword:UserChangePassword;
    isChangePasword:boolean;

    isChangeRoles:boolean;
    userChangeRoles:UserChangeRoles;

    allRoles:Array<string>;

    searchSelectionString:string;
    searchStr:string;

  constructor(private rolesServ: RolesService, private usersServ: UserService) {
    this.users = new Array<User>();
    this.isAmdim = this.rolesServ.isAdminRole();
    this.isView = true;
    this.isNewRecord = false;
    this.isShowStatusMessage = false;
    this.isEdit=false;

    this.userChangePassword = new UserChangePassword();
    this.isChangePasword = false;

    this.isChangeRoles = false;
    this.userChangeRoles = new UserChangeRoles();
    this.allRoles = new Array<string>();

    this.searchSelectionString="Поиск по";
    this.searchStr="";
   }

  ngOnInit(): void {
          this.loadUsers();
  }

  // load users
  loadUsers() {
    this.usersServ.getUsers().subscribe((data: User[]) => {
            this.users = data; 
        });
}

  // add user
  addUser() {
    this.editedUser = new User();
    this.users.push(this.editedUser);
    this.isNewRecord = true;
    this.isView = false;
}

 // edit user
 editUser(user: User) {
   this.isEdit = true;
   this.isView = false;
  this.editedUser = user;
}

// save user
saveUser() {
  
    if (this.isNewRecord) {
        // добавляем пользователя
        this.usersServ.createUser(this.editedUser).subscribe(response => {
                   
            this.loadUsers();  
            this.statusMessage = 'Данные успешно добавлены';
            
            console.log(response.status);
          }           
          ,err=>{
          this.statusMessage = 'Ошибка при добавлении данных';
          this.users.pop();
          console.log(err);
        }       
        );
    } else {
        // изменяем пользователя
        this.usersServ.updateUser(this.editedUser).subscribe(response => {
          
            this.loadUsers();
            this.statusMessage = 'Данные успешно изменены';
            console.log(response.status);
        },
        err=>{
          this.statusMessage = 'Ошибка при изменении данных';
          console.log(err);
        });      
    }
    this.isShowStatusMessage=true;
}

// cancel (back)
cancel() {
    // если отмена при добавлении, удаляем последнюю запись
    if (this.isNewRecord) {
        this.users.pop();
    }
    this.isNewRecord = false;
    this.isView = true;
    this.isEdit= false;
    this.isChangePasword=false;
    this.editedUser = null;
    this.isChangeRoles = false;
}
// delete user
deleteUser(user: User) {

    this.usersServ.deleteUser(user.id).subscribe(response => {

      if (response.status==200)
      {
        this.statusMessage = 'Данные успешно удалены';   
        this.loadUsers();
      }else{
        this.statusMessage = 'Ошибка удаления';
      }
    },err=>{
console.log(err);
this.statusMessage = 'Ошибка удаления';
      }
);

this.isShowStatusMessage=true;
}

// show info
showStatusMess(){
this.isShowStatusMessage=!this.isShowStatusMessage;
this.isNewRecord = false;
this.isView = true;
this.isEdit= false;
this.isChangeRoles = false;
this.isChangePasword = false;
}

// edit pass get data
changePassUser(id:string){
  this.userChangePassword = new UserChangePassword();
  this.userChangePassword.id=id;
  this.isView = false;
 this.isChangePasword=true;
}

// edit pass
editPassUser(){
  this.usersServ.changePass(this.userChangePassword).subscribe(response=>{
    this.statusMessage = 'Пароль успешно изменен';     
    console.log(response.status);
  },
  err=>{
    this.statusMessage = 'Ошибка при изменении пароля';
    console.log(err);
  });

  this.isShowStatusMessage=true;
}

changeRolesUser(id:string){
  this.userChangeRoles = new UserChangeRoles();

  this.userChangeRoles.roles =  new Array<string>();
  this.allRoles = new Array<string>();
  this.userChangeRoles.id = id;
  
  this.isView = false;
  this.isChangeRoles = true;
  
  this.rolesServ.getUserRoles(id).subscribe((data:string[])=>{
    this.userChangeRoles.roles = data;
  });
  
  this.rolesServ.getRoles().subscribe((data: string[])=> {
    this.allRoles = data;
});
}

checkRole(role:string):boolean{
  if (this.userChangeRoles.roles.includes(role)){
    return true;
  }
  else{
    return false;
  }
}

editUserRoles(){
  console.log("id: "+this.userChangeRoles.id);
  console.log("roles: "+this.userChangeRoles.roles);

  this.rolesServ.editRoles(this.userChangeRoles).subscribe(response=>{
    this.statusMessage = 'Права доступа успешно изменены';     
    console.log(response.status);
  },
  err=>{
    this.statusMessage = 'Ошибка при изменении прав доступа';
    console.log(err);
  });

  this.isShowStatusMessage=true;
}

pressRole(role:string){
  if (!this.userChangeRoles.roles.includes(role)){
    this.userChangeRoles.roles.push(role);
  }
  else{
    const index: number = this.userChangeRoles.roles.indexOf(role);
    if (index !== -1) {
        this.userChangeRoles.roles.splice(index, 1);
    }
  }
}

getViewUsers():Array<User>{
if (this.searchSelectionString=="Id"){
  return this.users.filter(x=>x.id.toLowerCase().includes(this.searchStr.toLowerCase()));
}
else if (this.searchSelectionString=="Email"){
  return this.users.filter(x=>x.email.toLowerCase().includes(this.searchStr.toLowerCase()));
}
else if (this.searchSelectionString=="ФИО"){
  let selectUserByFLP : Array<User>   = new Array<User>();

  for (let user of this.users){
  let flp : string = user.firstname +" "+user.lastname+" "+user.patronymic;
  if (flp.toLowerCase().includes(this.searchStr.toLowerCase())){
    selectUserByFLP.push(user);
  }
}
  return selectUserByFLP;
}

return this.users;
}

refresh(){
 this.searchSelectionString="Поиск по";
 this.searchStr ="";
}

}
