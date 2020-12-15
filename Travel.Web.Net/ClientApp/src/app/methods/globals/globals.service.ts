import { Injectable } from '@angular/core';
import Swal from 'sweetalert2'

@Injectable()
export class GlobalsService {

  constructor() { }

  public getDate() {
    var separador = "-";
    var fecha = new Date();
    var anio = fecha.getFullYear();
    var mes: any = fecha.getMonth() + 1;
    var dia: any = fecha.getDate() -1;

    mes < 10 ? mes = "0" + mes : mes;
    dia < 10 ? dia = "0" + dia : dia;

    return anio + separador + mes + separador+ dia;
  }


  public formatField(valor, restriccion, caracteres, tipo) {
    var out = '';
    var filtro = '' + restriccion + '';
    for (var i = 0; i < valor.length; i++) {
      if (filtro.indexOf(valor.charAt(i)) != -1) {
        if (out.length >= caracteres) {
          out.substr(0, caracteres);
        } else {
          out += valor.charAt(i);
        }
      }
    }
    return (tipo == 1) ? out.toUpperCase() : out;
  }

  public formatNumber(amount, decimals) {
    amount += '';
    amount = parseFloat(amount.replace(/[^0-9\.]/g, ''));

    decimals = decimals || 0;

    if (isNaN(amount) || amount === 0)
      return parseFloat("0").toFixed(decimals);

    amount = '' + amount.toFixed(decimals);

    var amount_parts = amount.split('.'),
      regexp = /(\d+)(\d{3})/;

    while (regexp.test(amount_parts[0]))
      amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

    return amount_parts.join('.');
  }

  public showToast(texto, tipo, posicion) {
    var tipo_ = "";
    var fondo = "";
    if (tipo == "success") {
      tipo_ = '<i class="mdi mdi-checkbox-marked-circle-outline" style="font-size: 20px; color: #fff; padding-right: 8px"></i>'
      fondo = "#28B463";
    } else if (tipo == "warning") {
      tipo_ = '<i class="mdi mdi-alert-outline" style="font-size: 20px; color: #fff; padding-right: 8px"></i>'
      fondo = "#D4AC0D";
    } else if (tipo == "error") {
      tipo_ = '<i class="mdi mdi-alert-octagon" style="font-size: 20px; color: #fff; padding-right: 8px"></i>'
      fondo = "#CB4335";
    }

    const Toast = Swal.mixin({
      toast: true,
      position: posicion,
      showConfirmButton: false,
      timer: 3000
    });

    Toast.fire({
      html: tipo_ + '<span style="color: #FFF; font-size: 12.5px !important;">' + texto + '</span>',
      background: fondo
    })
  }

  public showTimeAlert(titulo, texto, tipo) {
    Swal.fire({
      title: titulo,
      html: texto,
      type: tipo,
      confirmButtonText: 'Aceptar',
      timer: 3000
    }).then((result) => {
      if (result.dismiss === Swal.DismissReason.timer) {

      }
    })
  }

  public showAlert(titulo, texto, tipo) {
    Swal.fire({
      title: titulo,
      html: texto,
      type: tipo
    });
  }
}
