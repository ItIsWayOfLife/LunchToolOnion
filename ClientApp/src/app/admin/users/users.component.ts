import { Component, OnInit, TemplateRef, ViewChild} from '@angular/core';

import {RolesService} from '../../service/roles.service';
import {User} from './user';
import { Router } from "@angular/router";
import {UserService} from '../../service/user.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  providers:[RolesService, UserService]
})
export class UsersComponent implements OnInit {

   //типы шаблонов
   @ViewChild('readOnlyTemplate', {static: false}) readOnlyTemplate: TemplateRef<any>;
   @ViewChild('editTemplate', {static: false}) editTemplate: TemplateRef<any>;

  myRoles:Array<string>;
  isAmdim:boolean;

    editedUser: User;
    users: Array<User>;
    isView:boolean;
    isNewRecord: boolean;
    isShowStatusMessage:boolean;
    statusMessage: string;

  constructor(private rolesServ: RolesService, private usersServ: UserService) {
    this.users = new Array<User>();
    this.myRoles = new Array<string>();
    this.isView = true;
    this.isNewRecord = false;
    this.isShowStatusMessage = false;

   }

  ngOnInit(): void {
          this.getRoles();
          this.loadUsers();
  }

// получение ролей
private getRoles(){
  this.myRoles = this.rolesServ.MyGetRelos();
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

  //загрузка пользователей
  private loadUsers() {
    this.usersServ.getUsers().subscribe((data: User[]) => {
            this.users = data; 
        });
}

  // добавление пользователя
  addUser() {
    this.editedUser = new User();
    this.users.push(this.editedUser);
    this.isNewRecord = true;
    this.isView = false;
}

 // редактирование пользователя
 editUser(user: User) {
  this.editedUser = new User();
}
// загружаем один из двух шаблонов
loadTemplate(user: User) {
  if (this.editedUser && this.editedUser.id === user.id) {
      return this.editTemplate;
  } else {
      return this.readOnlyTemplate;
  }
}

// сохраняем пользователя
saveUser() {
  this.isShowStatusMessage=true;
    if (this.isNewRecord) {
        // добавляем пользователя
        this.usersServ.createUser(this.editedUser).subscribe(response => {
                   
            this.loadUsers();  
            this.statusMessage = 'Данные успешно добавлены',    
            this.editedUser = null;  
            console.log(response.status);
          }           
          ,err=>{
          this.statusMessage = 'Ошибка при добавлении данных',
          console.log(err);
        }       
        );
       
    } else {
        // изменяем пользователя
        this.usersServ.updateUser(this.editedUser).subscribe(data => {
            this.statusMessage = 'Данные успешно обновлены',
            this.loadUsers();
        });
        this.editedUser = null;
    }
}
// отмена редактирования
cancel() {
    // если отмена при добавлении, удаляем последнюю запись
    if (this.isNewRecord) {
        this.users.pop();
        this.isNewRecord = false;
        this.isView = true;
    }
    this.editedUser = null;
}
// удаление пользователя
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

showStatusMess(){
this.isShowStatusMessage=!this.isShowStatusMessage;
this.isNewRecord = false;
this.isView = true;
}
}
