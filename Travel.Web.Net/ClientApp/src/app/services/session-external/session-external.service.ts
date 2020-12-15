import { Injectable } from '@angular/core';

@Injectable()
export class SessionExternalService {

  private keySession: string = "l:key-sesion-external";

  constructor() { }

  deleteKeyExternal() {
    localStorage.removeItem(this.keySession);
  }

  addKeyExternal(data) {
    localStorage.setItem(this.keySession, JSON.stringify(data));
  }


  getKeyExternal() {
    var data = localStorage.getItem(this.keySession);

    if (data == null) {
      return {
        "access_token": null,
        "error": null,
        "error_description": null,
        "expires": null,
        "expires_in": null,
        "issued": null,
        "refresh_token": null,
        "token_type": null
      }
    } else {
      return JSON.parse(data);
    }

  }
}
