import { Component, Input, OnInit } from '@angular/core';
import { ExternalDataService } from '../../../../controllers/external/data/data.service';
import { ValidationsService } from '../../../../methods/validations/validations.service';
import { SessionExternalService } from '../../../../services/session-external/session-external.service';
import { SessionInternalService } from '../../../../services/session-internal/session-internal.service';

@Component({
  selector: 'app-reservation-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Input() dtRegister = {
    codeCompany: null,
    company: null,
    name: null,
    identification: null,
    email: null,
    department: null,
    position: null,
    city: null
  };

  @Input() vlRegister = {
    company: null,
    name: null
  }

  dtCompany: any;

  lstCompany = [];

  constructor(private data_external: ExternalDataService, private session_external: SessionExternalService, public validations_: ValidationsService) { }

  ngOnInit() {
    
    this.getCompany();
  }

  public getCompany() {
    var token = this.session_external.getKeyExternal();
    this.data_external.getCompany(token).then(res => {
      this.lstCompany = res;
      this.dtCompany = {Nombre: "", Codigo: this.dtRegister.codeCompany == null ? "" : this.dtRegister.codeCompany };
    }).catch(err => {
      console.log(err);
    });

  }

  public setCompany() {
    console.log(this.dtCompany);
    var company = this.dtCompany;
    this.dtRegister.company = company.Nombre;
    this.dtRegister.codeCompany = company.Codigo;
    this.validations();
  }

  public validations() {
    var validation = this.validations_.validationsRegister(this.dtRegister);
    this.vlRegister = validation.vlRegister;
  }

}
