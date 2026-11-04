import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { Order } from '@app/models/orders/order.model';
import { PreOrdersService } from '@app/services/pre-orders/pre-orders.service';
import { ReservationService } from '@app/services/reservation/reservation.service';
import { Reservation, ReservationStatus } from '@app/models/reservations/reservation.model';

import { CartComponent } from './cart.component';

describe('CartComponent', () => {
  let component: CartComponent;
  let fixture: ComponentFixture<CartComponent>;

  let preOrdersServiceMock: jasmine.SpyObj<PreOrdersService>;
  let routerMock: jasmine.SpyObj<Router>;
  let reservationsMock: jasmine.SpyObj<ReservationService>;

  beforeEach(async () => {
    preOrdersServiceMock =
      jasmine.createSpyObj<PreOrdersService>(
        'PreOrdersService',
        [
          'getPreOrders',
          'cancelPreOrderByReservationId',
          'unsubmitPreOrderByReservationId',
          'submitPreOrderByReservationId',
          'editPreOrderByReservationId',
        ],
      );

    routerMock = jasmine.createSpyObj<Router>(
      'Router',
      ['navigate'],
    );

    preOrdersServiceMock.getPreOrders.and.returnValue(of([]));
    reservationsMock = jasmine.createSpyObj<ReservationService>('ReservationService', ['getReservationsForUser$']);
    reservationsMock.getReservationsForUser$.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [CartComponent],
      providers: [
        { provide: ReservationService, useValue: reservationsMock },
        {
          provide: PreOrdersService,
          useValue: preOrdersServiceMock,
        },
        {
          provide: Router,
          useValue: routerMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CartComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  const savedOrder: Order = {
    orderId: 'order-time', reservationId: 'reservation-time',
    reservationAddress: '9 Abashidze Street',
    reservationTime: 'Nov 13, 2026, 11:30 AM - 1:00 PM',
    orderItems: [], status: 0,
  };

  it('shows the same local reservation time as the reservation card instead of the stored UTC label', () => {
    preOrdersServiceMock.getPreOrders.and.returnValue(of([savedOrder]));
    reservationsMock.getReservationsForUser$.and.returnValue(of([{
      id: 'reservation-time', startTime: new Date(2026, 10, 13, 13, 30),
      endTime: new Date(2026, 10, 13, 15), status: ReservationStatus.Reserved,
    } as Reservation]));
    component.ngOnInit();
    expect(component.orders()[0].reservationTime).toBe('Nov 13, 2026, 1:30 PM - 3:00 PM');
    expect(savedOrder.reservationTime).toBe('Nov 13, 2026, 11:30 AM - 1:00 PM');
  });

  it('keeps cart contents if reservation details cannot be loaded', () => {
    preOrdersServiceMock.getPreOrders.and.returnValue(of([savedOrder]));
    reservationsMock.getReservationsForUser$.and.returnValue(throwError(() => new Error('unavailable')));
    component.ngOnInit();
    expect(component.orders()).toEqual([savedOrder]);
  });

  it('does not request reservations for an empty cart', () => {
    expect(reservationsMock.getReservationsForUser$).not.toHaveBeenCalled();
  });

  it('should emit closeCartEvent when closeCart is called', () => {
    spyOn(component.closeCartEvent, 'emit');

    component.closeCart();

    expect(
      component.closeCartEvent.emit,
    ).toHaveBeenCalled();
  });

  it('should return submitted orders with statuses 1 and 3', () => {
    component.orders.set([
      {
        orderId: 'order-1',
        status: 1,
      },
      {
        orderId: 'order-2',
        status: 2,
      },
      {
        orderId: 'order-3',
        status: 3,
      },
      {
        orderId: 'order-4',
        status: 0,
      },
    ] as Order[]);

    expect(
      component
        .submittedOrders()
        .map((order) => order.status),
    ).toEqual([1, 3]);
  });

  it('should return unsubmitted orders with status 0', () => {
    component.orders.set([
      {
        orderId: 'order-1',
        status: 0,
      },
      {
        orderId: 'order-2',
        status: 1,
      },
      {
        orderId: 'order-3',
        status: 3,
      },
    ] as Order[]);

    expect(
      component
        .unsubmittedOrders()
        .map((order) => order.status),
    ).toEqual([0]);
  });

  it('should navigate to reservations when goToReservations is called', () => {
    component.goToReservations();

    expect(routerMock.navigate).toHaveBeenCalledWith([
      '/reservations',
    ]);
  });
});
