import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ControlData, ControlState } from '../data-classes/control-data';

@Injectable({
  providedIn: 'root'
})
export class ControlDataService {

  constructor() { }

  getAllControls(): ControlData[] {
    return environment.testData['control-data'];
  }
  turnOn(controlName: string, controlState: ControlState): ControlState | null {
    const controlData = environment.testData['control-data'].find((data) => data.name === controlName);
    if (controlData) {
      // Toggle the state of the control
      controlData.state = controlData.state === ControlState.ON ? ControlState.OFF ? ControlState.UNKOWN : ControlState.ON : 1;
      return controlData.state;
    } else {
      return null;
    }
  }
}
export { ControlData, ControlState };

