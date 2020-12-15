import { Injectable } from '@angular/core';

@Injectable()
export class ValidationsService {

  constructor() { }

  public validationsRegister(dtRegister: any) {
    var vlRegister = {
      company: null,
      name: null
    }
    var params = { state: true, vlRegister: vlRegister };

    if (dtRegister.company == "") {
      vlRegister.company = true;
      params = { state: false, vlRegister: vlRegister };
    } if (dtRegister.name == "") {
      vlRegister.name = true;
      params = { state: false, vlRegister: vlRegister };
    }

    if (vlRegister.company == false && vlRegister.name == false) {
      vlRegister.company = false;
      vlRegister.name = false;
      params = { state: true, vlRegister: vlRegister };
    }

    return params;
  }

  public validationsTravel(dtTravel: any) {
    var vlTravel = {
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

    var params = { state: true, vlTravel: vlTravel };

    if (dtTravel.travelType == "" || dtTravel.travelType == null) {
      vlTravel.travelType = true;
      params = { state: false, vlTravel: vlTravel };
    }

    if (dtTravel.requiresLodging == "" || dtTravel.requiresLodging == null) {
      vlTravel.requiresLodging = true;
      params = { state: false, vlTravel: vlTravel };
    }

    if (dtTravel.trainingTrip == "" || dtTravel.trainingTrip == null) {
      vlTravel.trainingTrip = true;
      params = { state: false, vlTravel: vlTravel };
    }

    if (dtTravel.shortName == "" || dtTravel.shortName == null) {
      vlTravel.shortName = true;
      params = { state: false, vlTravel: vlTravel };
    }
    if (dtTravel.reasonTrip == "" || dtTravel.reasonTrip == null) {
      vlTravel.reasonTrip = true;
      params = { state: false, vlTravel: vlTravel };
    }

    /* if (vlTravel.shortName == false) {
       vlTravel.shortName = false;
       vlTravel.startDate = false;
       params = { state: true, vlTravel: vlTravel };
     }*/

    return params;
  }

  public validationsTransport(dtTransport: any, dtTravel: any) {

    var vlTransport = {
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
    }
    var params = { state: true, vlTransport: vlTransport };

    if (dtTransport.destination == "" || dtTransport.destination == null) {
      vlTransport.destination = true;
      params = { state: false, vlTransport: vlTransport };
    }

    if (dtTravel.travelType == "Aéreo") {

      if (dtTransport.requiresPassage == "" || dtTransport.requiresPassage == null) {
        vlTransport.requiresPassage = true;
        params = { state: false, vlTransport: vlTransport };
      } else if (dtTransport.requiresPassage == "No") {

        if (dtTransport.broadcastDate == "" || dtTransport.broadcastDate == null) {
          vlTransport.broadcastDate = true;
          params = { state: false, vlTransport: vlTransport };
        }

        if (dtTransport.airlineName == "" || dtTransport.airlineName == null) {
          vlTransport.airlineName = true;
          params = { state: false, vlTransport: vlTransport };
        }

        if (dtTransport.numberPassage == "" || dtTransport.numberPassage == null) {
          vlTransport.numberPassage = true;
          params = { state: false, vlTransport: vlTransport };
        }

        if (dtTransport.referencePay == "" || dtTransport.referencePay == null) {
          vlTransport.referencePay = true;
          params = { state: false, vlTransport: vlTransport };
        }

        if (dtTransport.valuePay == "" || dtTransport.valuePay == null) {
          vlTransport.valuePay = true;
          params = { state: false, vlTransport: vlTransport };
        }

        if (dtTransport.airTax == "" || dtTransport.airTax == null) {
          vlTransport.airTax = true;
          params = { state: false, vlTransport: vlTransport };
        }

        if (dtTransport.tax == "" || dtTransport.tax == null) {
          vlTransport.tax = true;
          params = { state: false, vlTransport: vlTransport };
        }

      }

    } else if (dtTravel.travelType == "Terrestre") {
      

      if (dtTransport.requiresMovilization == "" || dtTransport.requiresMovilization == null) {
        vlTransport.requiresMovilization = true;
        params = { state: false, vlTransport: vlTransport };
      }

      if(dtTransport.requiresMovilization == "Sí"){
        if (dtTransport.numberKilometers == "" || dtTransport.numberKilometers == null) {
          vlTransport.numberKilometers = true;
          params = { state: false, vlTransport: vlTransport };
        }      
      }      
      
      return params;
    }
  }

  public validationsLodging(dtHotel: any) {
    var vlHotel = {
      hotel: null,
      rate: true
    }
    console.log(dtHotel)
    var params = { state: true, vlHotel: vlHotel };

    if (dtHotel.hotel == "" || dtHotel.hotel == null) {
      vlHotel.hotel = true;
      params = { state: false, vlHotel: vlHotel };
    }

    if (vlHotel.hotel == false) {
      vlHotel.hotel = false;
      params = { state: true, vlHotel: vlHotel };
    }

    return params;
  }

}
