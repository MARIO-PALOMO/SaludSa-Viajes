import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { GlobalsService } from '../../../../methods/globals/globals.service';
import { ValidationsService } from '../../../../methods/validations/validations.service';
import { TravelExpensesComponent } from '../travel-expenses/travel-expenses.component';

declare var $: any;
declare var moment: any;

@Component({
  selector: 'app-reservation-travel',
  templateUrl: './travel.component.html',
  styleUrls: ['./travel.component.css']
})
export class TravelComponent implements OnInit {

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
    reasonTrip: null,
    block: null
  };

  @Input() vlTravel = {
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

  dtTransport = {
  };

  vlTransport = {
    typeTransport: null
  };

  lstTipoViaje: Array<string> = ['Aéreo', 'Terrestre'];
  lstHospedaje: Array<string> = ['Sí', 'No'];
  lstCapacitacion: Array<string> = ['Sí', 'No'];
  lstNoches: Array<any> = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  firstDateRange: Date = new Date(moment().format('YYYY-MM-DD LT'));
  secondDataRange: Date = null;

  constructor(public global: GlobalsService, public validations_: ValidationsService) { }

  ngOnInit() {
  }

  public validations() {
    var validation = this.validations_.validationsTravel(this.dtTravel);
    this.vlTravel = validation.vlTravel;
    if (validation.state == false) {
      $("#tab3").css("pointer-events", "none");
    }
  }

  public validateDate(date: any) {
    this.dtTravel.endDate = null;
    this.secondDataRange = new Date(moment(date).format('YYYY-MM-DD LT'));;
  }


  public validateNightsTravel(date1: any, date2: any) {

    var diferencia = moment(date2).startOf('day').diff(moment(date1).startOf('day'), 'days');

    console.log('diferencia', diferencia);

    this.dtTravel.numbersNight = diferencia;

    if (diferencia == 0) {
      this.dtTravel.requiresLodging = "No";
      this.dtTravel.block = true;
    } else {
      this.dtTravel.requiresLodging = "Si";
      this.dtTravel.block = false;
    }

    this.getCalculations();
  }

  public getCalculations() {
    this.validations();
  }


}
