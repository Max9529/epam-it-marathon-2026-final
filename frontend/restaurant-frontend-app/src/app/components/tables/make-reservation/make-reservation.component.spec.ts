import { API_BASE_URL } from '@app/shared/config/api-base-url.token';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MakeReservationComponent } from './make-reservation.component';

describe('MakeReservationComponent', () => {
  let component: MakeReservationComponent;
  let fixture: ComponentFixture<MakeReservationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting(), { provide: API_BASE_URL, useValue: 'https://test.api' }],
      imports: [MakeReservationComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MakeReservationComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('table', { id: 'demo-table', tableNumber: 1, capacity: 4, locationId: 'demo', locationAddress: 'Demo restaurant', timeRanges: [{ start: '2026-11-07T13:00:00Z', end: '2026-11-07T14:30:00Z' }] });
    fixture.componentRef.setInput('selectedTimeRange', { start: '2026-11-07T13:00:00Z', end: '2026-11-07T14:30:00Z' });
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
