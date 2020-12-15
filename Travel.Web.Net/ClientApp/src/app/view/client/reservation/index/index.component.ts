import { Component, OnInit, ViewChild } from '@angular/core';
import { ExternalDataService } from '../../../../controllers/external/data/data.service';
import { InternalDataService } from '../../../../controllers/internal/data/data.service';
import { GlobalsService } from '../../../../methods/globals/globals.service';
import { ValidationsService } from '../../../../methods/validations/validations.service';
import { UserService } from '../../../../services/user/user.service';
import { LodgingComponent } from '../lodging/lodging.component';
import { TransportComponent } from '../transport/transport.component';
import { TravelExpensesComponent } from '../travel-expenses/travel-expenses.component';
import { TravelComponent } from '../travel/travel.component';

declare var $: any;


@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {
  @ViewChild(LodgingComponent) lodgingComponent: LodgingComponent;
  @ViewChild(TravelExpensesComponent) travelExpensesComponent: TravelExpensesComponent;
  @ViewChild(TransportComponent) transportComponent: TransportComponent;
  @ViewChild(TravelComponent) travelComponent: TravelComponent;

  dtRegister = {
    codeCompany: null,
    company: null,
    name: null,
    identification: null,
    email: null,
    department: null,
    position: null,
    city: null
  };

  vlRegister: any = {
    company: null,
    name: null
  }

  dtTravel = {
    startDate: new Date(),
    startDateTime: new Date(),
    endDate: new Date(),
    endDateTime: new Date(),
    travelType: 'Terrestre',
    shortName: null,
    requiresLodging: 'No',
    numbersNight: 0,
    trainingTrip: 'Sí',
    reasonTrip: 'YES NIGGA'
  };

  vlTravel = {
    startDate: null,
    startDateTime: null,
    endDate: null,
    endDateTime: null,
    travelType: null,
    shortName: null,
    requiresLodging: null,
    numbersNight: null,
    trainingTrip: null,
    reasonTrip: null
  }

  tempIdRuta;

  dtTransport = {
    destination: null,
    nameDestination: null,
    requiresPassage: null,
    requiresMovilization: null,

    broadcastDate: null,
    airlineName: null,
    numberPassage: null,
    referencePay: null,
    valuePay: null,
    airTax: null,
    tax: null,
    valueTax: null,
    valuePassage: null,

    numberKilometers: null,
    valueKilometers: null,

    dtImpuesto: null,
    total: null
  };

  vlTransport = {
    destination: null,
    requiresPassage: null,
    requiresMovilization: null,

    broadcastDate: null,
    airlineName: null,
    numberPassage: null,
    referencePay: null,
    valuePay: null,
    airTax: null,
    tax: null,
    valueTax: null,
    valuePassage: null,

    numberKilometers: null,
    valueKilometers: null,

    total: null
  };

  dtHotel = {
    hotel: null,
    rate: null
  };

  vlHotel = {
    hotel: null,
    rate: null
  };

  dtMoney = {
    days: 1,
    nights: null,
    moneyDays: null,
    moneyNights: null,
    totalMoneyDaysNights: null,
    selectedMoney: null,
    totalMoney: null,
    descriptionMoney: null,
    totalHotel: null,
    total: 0,
    numberKilometers: null,
    valueKilometers: null,
    requiresMovilization: null,
    otherMoney: 'No',
    valueOtherMoney: 0,
    justifyMoney: null
  }

  lstParameters = [];

  nombres = [];
  apellidos = [];
  flagRute = 0;

  constructor(private user: UserService, private data_internal: InternalDataService, public global: GlobalsService, public validations_: ValidationsService) { }

  ngAfterContentInit() {
    this.getParameters();
  }
  ngOnInit() {
    this.getRegister();
  }


  public getRegister() {
    var user = this.user.getUser();
    this.dtRegister.codeCompany = user.CompaniaCodigo;
    this.dtRegister.company = user.CompaniaDescripcion.toUpperCase() == "NO EXISTE" ? "" : user.CompaniaDescripcion.toUpperCase();
    this.dtRegister.name = user.NombreCompleto.toUpperCase();
    this.dtRegister.identification = user.Cedula.toUpperCase();
    this.dtRegister.email = user.Email;
    this.dtRegister.department = user.Departamento.toUpperCase();
    this.dtRegister.position = user.Cargo.toUpperCase();
    this.dtRegister.city = user.CiudadDescripcion.toUpperCase();
    this.nombres = user.Nombres != "" || user.Nombres != null || user.Nombres != undefined ? user.Nombres.split(' ') : [];
    this.apellidos = user.Apellidos != "" || user.Apellidos != null || user.Apellidos != undefined ? user.Apellidos.split(' ') : [];
    this.dtTravel.shortName = (this.nombres.length == 0 ? "" : this.nombres[0]) + " " + (this.apellidos.length == 0 ? "" : this.apellidos[0]);
  }

  public getParameters() {
    this.data_internal.getParameters().then(res => {
      this.lstParameters = res;
      console.log(res)
    }).catch(err => {

      console.log(err);
    });
  }

  public styleStep(tab) {
    if (tab == 0) {
      $('#navigation-register a[id="register-tab"]').tab('show');
      $("#register").addClass("show active");
      $("#travel").removeClass("show active");
      $("#transport").removeClass("show active");
      $("#hotel").removeClass("show active");
      $("#money").removeClass("show active");
      $("#approver").removeClass("show active");
    } else if (tab == 1) {
      $('#navigation-register a[id="travel-tab"]').tab('show');
      $("#register").removeClass("show active");
      $("#travel").addClass("show active");
      $("#transport").removeClass("show active");
      $("#hotel").removeClass("show active");
      $("#money").removeClass("show active");
      $("#approver").removeClass("show active");
    } else if (tab == 2) {
      $('#navigation-register a[id="transport-tab"]').tab('show');
      $("#register").removeClass("show active");
      $("#travel").removeClass("show active");
      $("#transport").addClass("show active");
      $("#hotel").removeClass("show active");
      $("#money").removeClass("show active");
      $("#approver").removeClass("show active");
    } else if (tab == 3) {
      $('#navigation-register a[id="hotel-tab"]').tab('show');
      $("#register").removeClass("show active");
      $("#travel").removeClass("show active");
      $("#transport").removeClass("show active");
      $("#hotel").addClass("show active");
      $("#money").removeClass("show active");
      $("#approver").removeClass("show active");
    } else if (tab == 4) {
      $('#navigation-register a[id="money-tab"]').tab('show');
      $("#register").removeClass("show active");
      $("#travel").removeClass("show active");
      $("#transport").removeClass("show active");
      $("#hotel").removeClass("show active");
      $("#money").addClass("show active");
      $("#approver").removeClass("show active");
    } else if (tab == 5) {
      $('#navigation-register a[id="approver-tab"]').tab('show');
      $("#register").removeClass("show active");
      $("#travel").removeClass("show active");
      $("#transport").removeClass("show active");
      $("#hotel").removeClass("show active");
      $("#money").removeClass("show active");
      $("#approver").addClass("show active");
    }
  }

  public nextSteep(tab) {
    this.management();
    if (tab == 1) {
      var validationRegister = this.validations(tab);
      if (validationRegister) {
        $("#tab1").css("pointer-events", "all");
        $("#tab2").css("pointer-events", "all");
        this.styleStep(tab);
      } else {
        $("#tab2").css("pointer-events", "none");
      }
    } else if (tab == 2) {
      var validationTravel = this.validations(tab);
      if (validationTravel) {
        $("#tab3").css("pointer-events", "all");
        this.styleStep(tab);
        this.clearDataTransport();
      } else {
        $("#tab3").css("pointer-events", "none");
      }
    } else if (tab == 3) {
      var validationTransport = this.validations(tab);
      console.log(validationTransport);
      if (validationTransport) {
        $("#tab4").css("pointer-events", "all");
        this.styleStep(tab);
        this.lodgingComponent.getHotel();

        this.flagRute++;

        if (this.transportComponent.dtListRouteSelected != undefined) {
          if (this.flagRute == 1) {
            this.tempIdRuta = this.transportComponent.dtListRouteSelected.IdRuta;
          }
          if (this.tempIdRuta != this.transportComponent.dtListRouteSelected.IdRuta) {
            this.lodgingComponent.dtHotel.hotel = null;
            this.lodgingComponent.dtHotel.rate = null;
            this.lodgingComponent.dtHotelSelected = null;
            this.tempIdRuta = this.transportComponent.dtListRouteSelected.IdRuta;
          }
        }

      } else {
        $("#tab4").css("pointer-events", "none");
      }
    } else if (tab == 4) {
      var validationLodging = this.validations(tab);
      if (validationLodging) {
        $("#tab5").css("pointer-events", "all");
        this.styleStep(tab);
      } else {
        $("#tab5").css("pointer-events", "none");
      }

    }else if (tab == 5) {
      var validationTravelExpenses = this.validations(tab);
      if (validationTravelExpenses) {
        $("#tab6").css("pointer-events", "all");
        this.styleStep(tab);
      } else {
        $("#tab6").css("pointer-events", "none");
      }

    }
  }

  public navigationBack(tab) {
    this.management();
    if (tab == 0) {
      this.styleStep(tab);
    } else if (tab == 1) {
      this.styleStep(tab);
    } else if (tab == 2) {
      this.styleStep(tab);
      this.clearDataTransport();
    } else if (tab == 3) {
      this.styleStep(tab);
    } else if (tab == 4) {
      this.styleStep(tab);
    } else if (tab == 5) {
      this.styleStep(tab);
    }
  }

  public validations(tab) {
    var state = true;
    if (tab == 1) {
      var validation = this.validations_.validationsRegister(this.dtRegister);
      this.vlRegister = validation.vlRegister;
      state = validation.state;
    } else if (tab == 2) {
      var validation2 = this.validations_.validationsTravel(this.dtTravel);
      this.vlTravel = validation2.vlTravel;
      state = validation2.state;
    } else if (tab == 3) {
      var validation3 = this.validations_.validationsTransport(this.dtTransport, this.dtTravel);
      this.vlTransport = validation3.vlTransport;
      state = validation3.state;
    } else if (tab == 4) {
      var validation4 = this.validations_.validationsLodging(this.dtHotel);
      this.vlHotel = validation4.vlHotel;
      state = validation4.state;
    }
    return state;
  }

  public management() {
    this.travelExpensesComponent.getCalculations();
  }

  public clearDataTransport() {
    if (this.travelComponent.dtTravel.travelType == "Aéreo") {
      this.transportComponent.dtTransport.requiresMovilization = null;
      this.transportComponent.dtTransport.numberKilometers = null;
      this.transportComponent.dtTransport.valueKilometers = null;
    } else if (this.travelComponent.dtTravel.travelType == "Terrestre") {
      this.transportComponent.dtTransport.requiresPassage = null;
      //this.transportComponent.dtTransport.total = null;
      this.transportComponent.dtTransport.tax = null;
      this.transportComponent.dtTransport.dtImpuesto = null;
      this.transportComponent.dtTransport.airTax = null;
      this.transportComponent.dtTransport.valuePay = null;
      this.transportComponent.dtTransport.referencePay = null;
      this.transportComponent.dtTransport.numberPassage = null;
      this.transportComponent.dtTransport.airlineName = null;
      this.transportComponent.dtTransport.broadcastDate = null;
    }
  }



  public finalizar() {
    console.log(this.dtRegister);
    //PONER UN SPINNER
  }


}
