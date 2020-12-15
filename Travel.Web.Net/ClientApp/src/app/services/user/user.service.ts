import { Injectable } from '@angular/core';

@Injectable()
export class UserService {


  keyUser = "k:user";

  constructor() { }

  public deleteUser() {
    localStorage.removeItem(this.keyUser);
  }

  public getUser() {
    var data = localStorage.getItem(this.keyUser);
    var result: any;
    if (data == null) {
      result = null;
    } else {
      try {
        result = JSON.parse(data);
      } catch (e) {
        result = null;
      }
    }
    return result;
  }

  public addUser(data) {
    localStorage.setItem(this.keyUser, JSON.stringify(data));
  }

}
