import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../../../../environments/environment';
import { ExternalDataService } from '../../../../controllers/external/data/data.service';
import { SessionExternalService } from '../../../../services/session-external/session-external.service';
import { SessionInternalService } from '../../../../services/session-internal/session-internal.service';
import { UserService } from '../../../../services/user/user.service';

@Component({
  selector: 'app-client-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  @Input() userData = { "Apellidos": "", "ApellidosNombres": "", "Cargo": "", "Cedula": "", "CiudadCodigo": "", "CiudadDescripcion": "", "CompaniaCodigo": "", "CompaniaDescripcion": "", "Departamento": "", "Email": "", "": "", "JefeInmediato": { "Apellidos": "", "ApellidosNombres": "", "Cargo": "", "Cedula": "", "CiudadCodigo": "", "CiudadDescripcion": "", "CompaniaCodigo": "", "CompaniaDescripcion": "", "Departamento": "", "Email": "", "Extension": "", "JefeInmediato": null, "NombreCompleto": "", "Nombres": "", "NombresApellidos": "", "Telefono": "", "Usuario": "mverduga", "UsuarioDominio": "" }, "NombreCompleto": "", "Nombres": "", "NombresApellidos": "", "Telefono": "", "Usuario": "", "UsuarioDominio": "" };

  variables = environment;

  constructor(private user: UserService, private activate_router: ActivatedRoute, private router: Router, private data_external: ExternalDataService, private session_external: SessionExternalService, private session_internal: SessionInternalService) { }

  ngOnInit() {
    if (this.user.getUser() == null) {
      this.getTokenExternal();
    } else {
      this.userData = this.user.getUser();
    }
  }

  public getTokenExternal() {
    //PONER UN SPINNER
    this.data_external.getTokenExternal().then(res => {
      var token = res;
      this.session_external.addKeyExternal(token);
      this.getRegister(token);
    }).catch(err => {
      console.log(err);
    });
  }

  public getRegister(token) {
    //PONER UN SPINNER
    this.data_external.getUser(token, this.activate_router.snapshot.params.data).then(res => {
      this.userData = res;
      this.user.addUser(this.userData);
    }).catch(err => {
      console.log(err);
    });
  }

  navigation(page) {
    if (page == 1) {
      var user = this.user.getUser();
      this.router.navigate(['/client/reservation/list/' + user.Usuario]);
    } else if (page == 2) {
      this.router.navigate(['/client/reservation']);
    } else if (page == 3) {
      this.user.deleteUser();
      window.location.href = this.variables.urlCompra;
    } else if (page == 4) {
      this.user.deleteUser();
      window.location.href = this.variables.urlPago;
    }
  }

}
