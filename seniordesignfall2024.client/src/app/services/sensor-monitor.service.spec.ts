import { TestBed } from '@angular/core/testing';

import { SensorMonitorService } from './sensor-monitor.service';

describe('SensorMonitorService', () => {
  let service: SensorMonitorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SensorMonitorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
