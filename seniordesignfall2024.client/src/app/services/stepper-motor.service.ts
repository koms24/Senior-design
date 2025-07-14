import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

export interface StepperStatus {
  StepperUid: string;
  Value: string;
}

@Injectable({
  providedIn: 'root'
})
export class StepperMotorService {

  constructor(private sensorHttp: HttpClient) {}

  getRun(stepper_uid: string, milliseconds: number): Observable<StepperStatus> {
    return this.sensorHttp.get<StepperStatus>(`/api/Stepper/${stepper_uid}/RunFor/${milliseconds}`).pipe(
      tap(response => {
        console.log('HTTP response:', response); // Log the response
      }),
      map(response => {
        // Directly return the response if it contains expected properties
        if (response && response.StepperUid && response.Value) {
          return response;
        } else {
          throw new Error('Unexpected response structure');
        }
      }),
      catchError(err => {
        console.error('Run request failed', err);
        return of({ StepperUid: 'Error', Value: 'Error' });  // return a default error status
      })
    );
  }

  getStep(stepper_uid: string, steps: number): Observable<StepperStatus> {
    return this.sensorHttp.get<StepperStatus>(`/api/Stepper/${stepper_uid}/Move/${steps}`).pipe(
      tap(response => {
        console.log('HTTP response:', response); // Log the response
      }),
      map(response => {
        // Directly return the response if it contains expected properties
        if (response && response.StepperUid && response.Value) {
          return response;
        } else {
          throw new Error('Unexpected response structure');
        }
      }),
      catchError(err => {
        console.error('Move request failed', err);
        return of({ StepperUid: 'Error', Value: 'Error' });  // return a default error status
      })
    );
  }
}
