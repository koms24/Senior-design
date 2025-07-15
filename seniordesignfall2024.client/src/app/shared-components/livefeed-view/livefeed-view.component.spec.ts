import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LivefeedViewComponent } from './livefeed-view.component';

describe('LivefeedViewComponent', () => {
  let component: LivefeedViewComponent;
  let fixture: ComponentFixture<LivefeedViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LivefeedViewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LivefeedViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
