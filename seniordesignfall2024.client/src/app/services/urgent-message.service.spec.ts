import { TestBed } from '@angular/core/testing';

import { UrgentMessageService } from './urgent-message.service';

describe('UrgentMessageService', () => {
  let service: UrgentMessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UrgentMessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
