import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

export interface GpioStatus {
  PinNum: number;
  Mode: string;
  Value: string;
}

@Injectable({
  providedIn: 'root'
})
export class SensorDataService {

  constructor(private sensorHttp: HttpClient) { }

  getStatus(pinNum: number): Observable<GpioStatus> {
    return this.sensorHttp.get<GpioStatus>(`/api/Gpio/${pinNum}`).pipe(
      tap(response => {
        console.log('HTTP response:', response); // Log the entire response
      }),
      map(response => {
        if (response && 'PinNum' in response && 'Mode' in response && 'Value' in response) {
          return response;  // Return the response if it has the expected properties
        } else {
          throw new Error('Unexpected response structure: ' + JSON.stringify(response));
        }
      }),
      catchError(err => {
        console.error('Request failed:', err);
        return of({ PinNum: pinNum, Mode: 'Error', Value: 'Error' });  // return a default error status
      })
    );
  }
  
  readPin(pinNum: number): Observable<GpioStatus> {
    return this.sensorHttp.get<GpioStatus>(`/api/Gpio/${pinNum}/Read`).pipe(
      tap(response => {
        console.log('HTTP response:', response); // Log the entire response
      }),
      map(response => {
        if (response && 'PinNum' in response && 'Mode' in response && 'Value' in response) {
          return response;  // Return the response if it has the expected properties
        } else {
          throw new Error('Unexpected response structure: ' + JSON.stringify(response));
        }
      }),
      catchError(err => {
        console.error('Request failed:', err);
        return of({ PinNum: pinNum, Mode: 'Error', Value: 'Error' });  // return a default error status
      })
    );
  }

  
  
}
