import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { PlantData } from '../data-classes/plant-data';

@Injectable({
  providedIn: 'root'
})
export class PlantDataService {

  constructor() { }

  public getAllPlants(): PlantData[] {
    return environment.testData['plant-data'];
  }
}
