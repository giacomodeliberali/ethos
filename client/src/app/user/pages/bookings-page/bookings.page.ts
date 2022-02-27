import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  BookingDto,
  BookingsService,
  UserDto,
} from '@core/services/ethos.generated.service';
import { UserService } from '@core/services/user.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.page.html',
  styleUrls: ['./bookings.page.scss'],
})
export class BookingsPage implements OnInit {
  bookings$: Observable<BookingDto[]>;

  constructor(private router: Router, private bookingsSvc: BookingsService) {
    this.bookings$ = this.bookingsSvc.getFutureBookings();
  }

  ngOnInit() {}

  goToSchedulePage() {
    this.router.navigate(['user']);
  }
}
