import { Component, Input, OnInit } from '@angular/core';
import { GlobalsService } from '../../../../methods/globals/globals.service';
import { ValidationsService } from '../../../../methods/validations/validations.service';

@Component({
  selector: 'app-reservation-travel-expenses',
  templateUrl: './travel-expenses.component.html',
  styleUrls: ['./travel-expenses.component.css']
})
export class TravelExpensesComponent implements OnInit {

  @Input() dtTravel = {
    startDate: null,
    startDateTime: new Date(),
    endDate: null,
    endDateTime: new Date(),
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
    total: null,
    numberKilometers: null,
    valueKilometers: null,
    dtImpuesto: null
  };

  @Input() dtHotel = {
    hotel: null,
    rate: null
  };

  @Input() dtMoney = {
    days: 1,
    nights: null,
    moneyDays: null,
    moneyNights: null,
    totalMoneyDaysNights: null,
    selectedMoney: null,
    totalMoney: null,
    descriptionMoney: null,
    totalHotel: null,
    total:0,

    numberKilometers: null,
    valueKilometers: null,
    requiresMovilization: null,
    otherMoney: 'No',
    valueOtherMoney: 0,
    justifyMoney: null
  }

  stateOtherMoney = false;

  @Input() lstParameters = [];
  lstOtherMoney: Array<string> = ['Sí', 'No'];

  constructor(public global: GlobalsService, public validations_: ValidationsService) { }

  ngOnInit() {
    setTimeout(() => {
      this.getCalculations();
    }, 1000);
  }

  public getViewOtherMoney(){
    if(this.dtMoney.otherMoney == "Sí"){
      this.stateOtherMoney = true;
    }else{
      this.stateOtherMoney = false;
      this.dtMoney.valueOtherMoney = 0;
      this.dtMoney.justifyMoney = null;
    }

    this.getCalculations();
  }

  public getCalculations() {
    var valueNight;
    var valueDay;
    if (this.dtTravel.trainingTrip == "Sí") {
      valueNight = this.lstParameters.find(e => e.NombreParametro == "ValorNocheCapacitacion");
      valueDay = this.lstParameters.find(e => e.NombreParametro == "ValorDiaCapacitacion");
    } else {
      valueNight = this.lstParameters.find(e => e.NombreParametro == "ValorNocheSinCapacitacion");
      valueDay = this.lstParameters.find(e => e.NombreParametro == "ValorDiaSinCapacitacion");
    }

    this.dtMoney.moneyDays = parseFloat(valueDay.ValorParametro) * this.dtMoney.days;
    this.dtMoney.moneyNights = parseFloat(valueNight.ValorParametro) * this.dtTravel.numbersNight;

    this.dtMoney.numberKilometers = this.dtTransport.numberKilometers;
    this.dtMoney.valueKilometers = this.dtTransport.valueKilometers == null ? 0 : this.dtTransport.valueKilometers;
    this.dtMoney.requiresMovilization = this.dtTransport.requiresMovilization;

    this.dtMoney.totalMoneyDaysNights = parseFloat(this.dtMoney.moneyDays) + parseFloat(this.dtMoney.moneyNights);
    this.dtMoney.totalHotel = parseFloat(this.dtHotel.rate) * parseInt(this.dtTravel.numbersNight);

    this.dtMoney.total = parseFloat(this.dtMoney.totalMoneyDaysNights) + parseFloat(this.dtMoney.valueKilometers) + this.dtMoney.valueOtherMoney;

    console.log(this.dtMoney.valueKilometers);
  }

}
