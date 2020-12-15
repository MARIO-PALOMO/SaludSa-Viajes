import { Component, Input, OnInit } from '@angular/core';
import { InternalDataService } from '../../../../controllers/internal/data/data.service';
import { ValidationsService } from '../../../../methods/validations/validations.service';
import { GlobalsService } from '../../../../methods/globals/globals.service';

declare var $: any;

@Component({
  selector: 'app-reservation-transport',
  templateUrl: './transport.component.html',
  styleUrls: ['./transport.component.css']
})
export class TransportComponent implements OnInit {


  @Input() dtTravel = {
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
  };

  @Input() dtTransport = {
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

    dtImpuesto: null
  };

  @Input() vlTransport = {
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
    valueKilometers: null
  };

  @Input() lstParameters = [];

  dtListRouteSelected;

  lstRutas: any = [];

  lstRequiresPassage: Array<string> = ['Sí', 'No'];


  //ESTO TIENEN QUE TRAER DE LA BASE DE DATOS
  lstTypeTaxes = [];

  kilometers = 0;

  constructor(public dataInternalService: InternalDataService, public validations_: ValidationsService, public global: GlobalsService) { }


  ngAfterContentInit() {
    this.obtenerRutas();
    setTimeout(() => {
      this.gerParemeters();
    }, 4000);
  }

  ngOnInit() {

  }

  gerParemeters() {
    var tmpKilometers = this.lstParameters.find(e => e.NombreParametro == "CalculoKilometros");
    this.kilometers = parseFloat(tmpKilometers.ValorParametro);
    var tmpLstTypeTaxes = this.lstParameters.find(e => e.NombreParametro == "TipoImpuestos");
    this.lstTypeTaxes = JSON.parse(tmpLstTypeTaxes.ValorParametro);
  }

  public obtenerRutas() {
    this.dataInternalService.obtenerRutas().then(res => {
      this.lstRutas = res;
    }).catch(err => {

      console.log(err);
    });
  }

  public calculateKilometeValue(numberOfKilometers: any) {
    this.dtTransport.valueKilometers = this.global.formatNumber((this.kilometers * numberOfKilometers), 2);
  }

  public selectionData() {
    this.dtTransport.destination = this.dtListRouteSelected.IdRuta,
      this.dtTransport.nameDestination = this.dtListRouteSelected.DestinoRuta,

      //this.lodgingComponent.selectionData();

      this.validations();
  }

  public selectionRequiresMovilization() {
    var require = this.dtTransport.requiresMovilization;
    if (require == "No") {
      this.dtTransport.numberKilometers = null;
      this.dtTransport.valueKilometers = null;
    }

    this.validations();
  }

  public validations() {
    var validation = this.validations_.validationsTransport(this.dtTransport, this.dtTravel);
    this.vlTransport = validation.vlTransport;
    if (validation.state == false) {
      $("#tab4").css("pointer-events", "none");
    }
  }

}
