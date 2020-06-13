import { Component, OnInit, TemplateRef, ViewChild} from '@angular/core';

import {RolesService} from '../../service/roles.service';
import {User} from './user';
import {UserService} from '../../service/user.service';
import {UserChangePassword} from './userChangePassword';
import {UserChangeRoles} from './userChangeRoles';
;

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  providers:[RolesService, UserService]
})
export class UsersComponent implements OnInit {

    myRoles:Array<string>;
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

  constructor(private rolesServ: RolesService, private usersServ: UserService) {
    this.users = new Array<User>();
    this.myRoles = new Array<string>();
    this.isView = true;
    this.isNewRecord = false;
    this.isShowStatusMessage = false;
    this.isEdit=false;

    this.userChangePassword = new UserChangePassword();
    this.isChangePasword = false;

    this.isChangeRoles = false;
    this.userChangeRoles = new UserChangeRoles();
    this.allRoles = new Array<string>();

   }

  ngOnInit(): void {
          this.getRoles();
          this.loadUsers();
  }

// get roles
 getRoles(){
  this.myRoles = this.rolesServ.myGetRelos();
  if (this.myRoles!=null){
  if (this.myRoles.includes("admin")){
    this.isAmdim=true;
        }
        else{
          this.isAmdim=false;
        }
      }
        else{
          this.isAmdim=false;
        }
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
  this.isShowStatusMessage=true;
    if (this.isNewRecord) {
        // добавляем пользователя
        this.usersServ.createUser(this.editedUser).subscribe(response => {
                   
            this.loadUsers();  
            this.statusMessage = 'Данные успешно добавлены';
            
            console.log(response.status);
          }           
          ,err=>{
          this.statusMessage = 'Ошибка при добавлении данных';
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
    // this.editedUser = null;
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
  this.isShowStatusMessage=true;
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
  this.isShowStatusMessage=true;
  this.usersServ.changePass(this.userChangePassword).subscribe(response=>{
    this.statusMessage = 'Пароль успешно изменен';     
    console.log(response.status);
  },
  err=>{
    this.statusMessage = 'Ошибка при изменении пароля';
    console.log(err);
  });
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
  console.log(this.userChangeRoles.roles);
}

}
