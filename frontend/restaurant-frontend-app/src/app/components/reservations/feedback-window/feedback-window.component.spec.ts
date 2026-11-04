import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FeedbackWindowComponent } from './feedback-window.component';
import { API_BASE_URL } from '@app/shared/config/api-base-url.token';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('FeedbackWindowComponent', () => {
  let component: FeedbackWindowComponent;
  let fixture: ComponentFixture<FeedbackWindowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), { provide: API_BASE_URL, useValue: 'https://test.api' }],
      imports: [FeedbackWindowComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(FeedbackWindowComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('selectedReservation', { id: 'demo-feedback', tableId: 'demo-table', startTime: '2026-11-07T13:00:00Z', endTime: '2026-11-07T14:30:00Z', guestCount: 2, status: 0, customerId: 'demo-customer', waiterId: 'demo-waiter' });
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
