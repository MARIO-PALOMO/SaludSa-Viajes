import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reservation-list',
  templateUrl: './reservation-list.component.html',
  styleUrls: ['./reservation-list.component.css']
})
export class ReservationListComponent implements OnInit {

  userData = { "apellidos": "", "apellidosNombres": "", "cargo": "", "cedula": "", "ciudadCodigo": "", "ciudadDescripcion": "", "companiaCodigo": "", "companiaDescripcion": "", "departamento": "", "email": "", "extension": "", "jefeInmediato": { "apellidos": "", "apellidosNombres": "", "cargo": "", "cedula": "", "ciudadCodigo": "", "ciudadDescripcion": "", "companiaCodigo": "", "companiaDescripcion": "", "departamento": "", "email": "", "extension": "", "jefeInmediato": null, "nombreCompleto": "", "nombres": "", "nombresApellidos": "", "telefono": "", "usuario": "", "usuarioDominio": "" }, "nombreCompleto": "", "nombres": "", "nombresApellidos": "", "telefono": "", "usuario": "", "usuarioDominio": "" }
  

  constructor(private router: ActivatedRoute) { }

  ngOnInit() {
    this.userData.usuario = this.router.snapshot.params.data;
  }

}
