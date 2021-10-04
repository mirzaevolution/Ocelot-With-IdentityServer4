import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent implements OnInit {
  public forecasts: WeatherForecast[];

  constructor(private _httpClient: HttpClient) {

  }
  ngOnInit(): void {
    let isAuthenticated: string = localStorage.getItem(environment.authKeys.isAuthenticated);
    if (isAuthenticated == "1") {

      let baseUrl: string = environment.apiUrl;
      let accessToken: string = localStorage.getItem(environment.authKeys.accessToken);
      this._httpClient.get<WeatherForecast[]>(baseUrl + '/weatherforecast',
        {
          headers: new HttpHeaders({
            "Authorization": `Bearer ${accessToken}`
          })
        }).subscribe(result => {
        this.forecasts = result;
      }, error => console.error(error));

    } else {
      alert("User not authenticated!");
    }
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
