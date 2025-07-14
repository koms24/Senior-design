import { Injectable, OnDestroy, OnInit } from '@angular/core';
import { of, Subscription, switchMap, timer } from 'rxjs';
import { SensorDataService } from './sensor-data.service';
import { UrgentMessageService } from './urgent-message.service';


@Injectable({
  providedIn: 'root'
})
export class SensorMonitorService implements OnInit, OnDestroy {
  monitorSubscription?: Subscription;
  constructor(
    private sensorDataService: SensorDataService,
    private urgentMessageService: UrgentMessageService
  ) { }

  ngOnInit(): void {
    this.monitorSubscription = timer(0, 10000).pipe(
      switchMap(() => of(this.sensorDataService.getAllSensors()))
    ).subscribe(result => result.forEach(sensorObject => {
      if (!sensorObject.IsInRange)
        this.urgentMessageService.addMessage(`${sensorObject.name} is out of range with a value of: ${sensorObject.value}`);
    }))
  }

  ngOnDestroy(): void {
    this.monitorSubscription?.unsubscribe();
  }


}
