import { Injectable } from '@angular/core';
import { ApiService } from '../../../services/api/api.service';

@Injectable()
export class ExternalDataService {

  constructor(private connection: ApiService) { }


  public getTokenExternal() {
    return new Promise<any>((resolve, reject) => {
      this.connection.get("AccessService/AccessService.svc/externo/usuario/obtener/token").subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          //AQUI UNA ALERTA VISIBLE EN CASO DE ERROR 
          //AQUI DEBE EXISTIR UN METODO QUE GUARDE EL ERROR EN CASO DE QUE EXISTA
          reject(err);
        }
      );
    });
  }

  public getUser(token: any, username: string) {
    return new Promise<any>((resolve, reject) => {
      this.connection.post("AccessService/AccessService.svc/externo/usuario/listar/" + username, token).subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          //AQUI UNA ALERTA VISIBLE EN CASO DE ERROR 
          //AQUI DEBE EXISTIR UN METODO QUE GUARDE EL ERROR EN CASA DE QUE EXISTA
          reject(err);
        }
      );
    });
  }

  public getCompany(token: any) {
    return new Promise<any>((resolve, reject) => {
      this.connection.post("AccessService/AccessService.svc/externo/empresa/listar", token).subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          //AQUI UNA ALERTA VISIBLE EN CASO DE ERROR 
          //AQUI DEBE EXISTIR UN METODO QUE GUARDE EL ERROR EN CASA DE QUE EXISTA
          reject(err);
        }
      );
    });
  }

  public getBossGroup(token: any) {
    return new Promise<any>((resolve, reject) => {
      this.connection.post("AccessService/AccessService.svc/externo/usuario/listar/grupos", token).subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          //AQUI UNA ALERTA VISIBLE EN CASO DE ERROR 
          //AQUI DEBE EXISTIR UN METODO QUE GUARDE EL ERROR EN CASA DE QUE EXISTA
          reject(err);
        }
      );
    });
  }
}
