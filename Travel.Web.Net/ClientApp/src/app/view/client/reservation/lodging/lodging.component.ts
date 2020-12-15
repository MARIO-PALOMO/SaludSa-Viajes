import { Component, Input, OnInit } from '@angular/core';
import { InternalDataService } from '../../../../controllers/internal/data/data.service';
import { ValidationsService } from '../../../../methods/validations/validations.service';
declare var $: any;
@Component({
  selector: 'app-reservation-lodging',
  templateUrl: './lodging.component.html',
  styleUrls: ['./lodging.component.css']
})
export class LodgingComponent implements OnInit {

  @Input() dtHotel = {
    hotel: null,
    rate: null
  }

  @Input() vlHotel = {
    hotel: null,
    rate: null
  }

  @Input() dtTransport = {
    nameDestination: null
  };

  lstHotels = [];
  dtHotelSelected: any;

  constructor(private data_internal: InternalDataService, public validations_: ValidationsService) { }

  ngOnInit() {

  }


  public getHotel() {
    this.data_internal.getHotels(this.dtTransport.nameDestination).then(res => {
      this.lstHotels = res;
    }).catch(err => {
      console.log(err);
    });

  }

  public selectionData() {
    this.dtHotel.hotel = this.dtHotelSelected.IdHotel;
    this.dtHotel.rate = this.dtHotelSelected.TarifaHotel;
    this.validations();
  }

  public validations() {
    var validation = this.validations_.validationsLodging(this.dtHotel);
    this.vlHotel = validation.vlHotel;
    if (validation.state == false) {
      $("#tab5").css("pointer-events", "none");
    }
  }

}
