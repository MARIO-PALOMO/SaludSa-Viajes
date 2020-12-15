import { Injectable } from '@angular/core';
import { ApiService } from '../../../services/api/api.service';

@Injectable()
export class InternalDataService {

  constructor(private connection: ApiService) { }

  public obtenerRutas() {
    return new Promise<any>((resolve, reject) => {
      this.connection.get("AccessService/AccessService.svc/interno/viaje/listar/rutas").subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          reject(err);
        }
      );
    });
  }

  public getParameters() {
    return new Promise<any>((resolve, reject) => {
      this.connection.get("AccessService/AccessService.svc/interno/parametros/listar").subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          reject(err);
        }
      );
    });
  }


  public getHotels(destination) {
    return new Promise<any>((resolve, reject) => {
      this.connection.get("AccessService/AccessService.svc/interno/hoteles/listar/" + destination).subscribe(
        (res: any) => {
          resolve(res);
        },
        err => {
          reject(err);
        }
      );
    });
  }

}
