import { Component, NgModule, OnInit } from '@angular/core';
import { catchError, map, Observable, of, Subject, tap } from 'rxjs';
import { Router, RouterModule } from '@angular/router';
import { ControlData, ControlState } from '../data-classes/control-data';
import { SensorDataService, GpioStatus } from '../services/sensor-data.service';
import { ControlDataService } from '../services/control-data.service';
import { LivefeedViewComponent } from '../shared-components/livefeed-view/livefeed-view.component';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { StepperMotorService, StepperStatus } from '../services/stepper-motor.service';
import { SafeUrl } from '@angular/platform-browser';
import { ImagingService } from '../services/imaging.service';

@Component({
  selector: 'home-page',
  standalone: true,
  imports: [LivefeedViewComponent, CommonModule, HttpClientModule, RouterModule],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
  providers: [ControlDataService,SensorDataService, StepperMotorService ]
})
export class HomePageComponent implements OnInit {
  controls: ControlData[] = [];
  precentages: number[] = [];
  public camerasInfo: {
    nextCam: Subject<boolean | string>,
    nextCamOb: Observable<boolean | string>,
    devInfo: MediaDeviceInfo
  }[] = [];
  public videoOptions: MediaTrackConstraints = {
    // width: {ideal: 1024},
    // height: {ideal: 576}
  };
  public imgSrc!: Observable<SafeUrl>;
  
  constructor(
    private sensorHttp: HttpClient,
    private router: Router,
    public sensorService: SensorDataService,
    public controlService: ControlDataService,
    public stepperService: StepperMotorService,
    public imageService: ImagingService
  ) { this.imgSrc = imageService.getJpeg();
  }

  public gpioStatusWater?: Observable<GpioStatus> ;
  public gpioStatusLight?: Observable<GpioStatus> ;
  public stepperStatus?: Observable<StepperStatus>;
 


  ngOnInit(): void {
    this.gpioStatusWater = this.sensorService.getStatus(13);
    this.gpioStatusLight = this.sensorService.getStatus(15);
    
  
  }
  writePinHighWater(pinNum: number): void{
   this.gpioStatusWater = this.sensorHttp.get<GpioStatus>(`/api/Gpio/${pinNum}/SetPinHigh`).pipe(
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
  writePinLowWater(pinNum: number): void {
    this.gpioStatusWater = this.sensorHttp.get<GpioStatus>(`/api/Gpio/${pinNum}/SetPinLow`).pipe(
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
  writePinHighLight(pinNum: number): void{
    this.gpioStatusLight = this.sensorHttp.get<GpioStatus>(`/api/Gpio/${pinNum}/SetPinHigh`).pipe(
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
   writePinLowLight(pinNum: number): void {
     this.gpioStatusLight = this.sensorHttp.get<GpioStatus>(`/api/Gpio/${pinNum}/SetPinLow`).pipe(
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

  getRun(stepper_uid: string, milliseconds: number): void {
     this.sensorHttp.get<StepperStatus>(`/api/Stepper/${stepper_uid}/RunFor/${milliseconds}`).pipe(
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
    ).subscribe();
  }
  getStep(stepper_uid: string, steps: number): void {
     this.sensorHttp.get<StepperStatus>(`/api/Stepper/${stepper_uid}/Move/${steps}`).pipe(
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
    ).subscribe();
  }
  
  buttonsClick() {
    // Prevent scrolling on every click!
    document.body.addEventListener("click", (evt: MouseEvent) => {
      const { target } = evt;
      if (target instanceof HTMLElement) {
        evt.preventDefault();
      }
    });
  
    let dpads = Array.prototype.slice.call(document.getElementsByClassName('d-pad'), 0),
      opads = Array.prototype.slice.call(document.getElementsByClassName('o-pad'), 0),
      els = dpads.concat(opads);
  
    function dir(direction: string) {
      for (let i = 0; i < els.length; i++) {
        const el = els[i];
        const isDpad = el.className.indexOf('d-') !== -1;
        const what = isDpad ? 'd-pad' : 'o-pad';
        console.log(what);
        el.className = `${what} ${direction}`;
      }
    }
  
    function keyDown(evt: KeyboardEvent) {
      alert(
        `onkeydown handler:\n` +
        `key property: ${evt.key}\n` 
        
      );
  
      switch (evt.key) {
        case 'ArrowLeft':
          dir('left');
          break;
        case 'ArrowRight':
          dir('right');
          break;
        case 'ArrowUp':
          dir('up');
          break;
        case 'ArrowDown':
          dir('down');
          break;
        default:
          console.log('Unsupported key');
      }
    }
  
    // Add the event listener for keydown
    window.addEventListener('keydown', keyDown);
  }
  
}


