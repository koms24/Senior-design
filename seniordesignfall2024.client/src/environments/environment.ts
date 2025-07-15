// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.


import { SensorData } from "../app/data-classes/sensor-data";
import { ControlData, ControlState } from "../app/data-classes/control-data";
import { PlantData } from "../app/data-classes/plant-data";

export const environment = {
  production: false,
 
  testData: {
    'sensor-data': [
      new SensorData('temp', 55),
      new SensorData('humidity', 10)
    ],
    'control-data': [
      new ControlData("Water Valve", ControlState.ON),
      new ControlData("Grow light", ControlState.OFF)

    ],
    'plant-data': [
      new PlantData(50, 50, 45, 100),
      new PlantData(500, 120, 225, 85),
      new PlantData(170, 320, 15, 70),
      new PlantData(50, 210, 85, 55),
      new PlantData(350, 210, 15, 40),
      new PlantData(400, 210, 15, 25),
      new PlantData(200, 70, 55, 10)
    ]

  }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
