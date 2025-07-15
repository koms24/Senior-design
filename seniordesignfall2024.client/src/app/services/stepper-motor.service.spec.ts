import { TestBed } from '@angular/core/testing';

import { StepperMotorService } from './stepper-motor.service';

describe('StepperMotorService', () => {
  let service: StepperMotorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StepperMotorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
