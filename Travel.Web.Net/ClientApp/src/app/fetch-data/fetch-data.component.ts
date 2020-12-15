import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    console.log(baseUrl)
    http.get(baseUrl + 'api/User/getUser?username=EMPALOMO').subscribe(res => {
      console.log(res);
    }, err => {
      console.log(err)
    });
  }
}

interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
