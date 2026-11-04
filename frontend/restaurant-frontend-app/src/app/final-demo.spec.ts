import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { MenuPageComponent } from './pages/menu-page/menu-page.component';
import { MainPageComponent } from './pages/main-page/main-page.component';
import { DishModalComponent } from './components/dish-modal/dish-modal.component';
import { API_BASE_URL } from './shared/config/api-base-url.token';
import { ToastService } from './services/toast/toast.service';

describe('Final demo menu states', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MenuPageComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'https://test.api' },
        { provide: ToastService, useValue: { error: jasmine.createSpy(), success: jasmine.createSpy() } }],
    }).compileComponents();
  });

  it('explains loading and an empty category', () => {
    const fixture = TestBed.createComponent(MenuPageComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Loading the menu');
    http.expectOne(request => request.url === 'https://test.api/dishes').flush([]);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('No dishes are available in this category');
    expect(fixture.nativeElement.textContent).not.toContain('Loading the menu');
    http.verify();
  });

  it('shows a recoverable error and retries the request', () => {
    const fixture = TestBed.createComponent(MenuPageComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    http.expectOne(request => request.url === 'https://test.api/dishes')
      .flush({}, { status: 503, statusText: 'Unavailable' });
    fixture.detectChanges();
    const retry = fixture.nativeElement.querySelector('[role="alert"] button') as HTMLButtonElement;
    expect(retry.textContent).toContain('Try again');
    retry.click();
    http.expectOne(request => request.url === 'https://test.api/dishes').flush([]);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[role="alert"]')).toBeNull();
    expect(fixture.nativeElement.textContent).toContain('No dishes are available');
    http.verify();
  });
});

describe('Final demo popular dish navigation regression', () => {
  it('opens details instead of navigating to a nonexistent dish route', async () => {
    await TestBed.configureTestingModule({
      imports: [MainPageComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'https://test.api' }],
    }).compileComponents();
    const fixture = TestBed.createComponent(MainPageComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    http.expectOne('https://test.api/dishes/popular').flush([{
      id: 'demo', name: 'Spring Salad', price: 18, weight: '420g', description: 'Fresh greens', imageUrl: '',
    }]);
    http.expectOne('https://test.api/locations').flush([]);
    fixture.detectChanges();
    const card = fixture.nativeElement.querySelector('button.dish-card') as HTMLButtonElement;
    expect(card).not.toBeNull();
    card.click();
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[role="dialog"]')?.textContent).toContain('Spring Salad');
    http.verify();
  });
});

describe('Final demo dish dialog accessibility', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [DishModalComponent] }).compileComponents();
  });

  it('identifies the dialog and closes with Escape', () => {
    const fixture = TestBed.createComponent(DishModalComponent);
    fixture.componentRef.setInput('dish', {
      id: 'demo', name: 'Spring Salad', description: 'Fresh seasonal greens', imageUrl: '',
      price: 18, weight: '420g', calories: '320', proteins: '12', fats: '14', carbohydrates: '38', vitamins: 'A, C',
    });
    const closed = spyOn(fixture.componentInstance.closed, 'emit');
    fixture.detectChanges();
    const dialog = fixture.nativeElement.querySelector('[role="dialog"]') as HTMLElement;
    expect(dialog.getAttribute('aria-modal')).toBe('true');
    expect(dialog.querySelector('#dish-dialog-title')?.textContent).toContain('Spring Salad');
    expect(dialog.querySelector('button')?.getAttribute('aria-label')).toBe('Close dish details');
    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true }));
    expect(closed).toHaveBeenCalledTimes(1);
  });
});
