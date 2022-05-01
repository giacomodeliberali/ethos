import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseDirective } from '@core/directives';
import {
  BookingDto,
  BookingsService,
  UserDto,
} from '@core/services/ethos.generated.service';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.page.html',
  styleUrls: ['./bookings.page.scss'],
})
export class BookingsPage extends BaseDirective implements OnInit {
  bookings$: Observable<BookingDto[]>;

  constructor(
    private router: Router,
    private bookingsSvc: BookingsService,
    private loadingSvc: LoadingService,
    private toastSvc: ToastService
  ) {
    super();
  }

  ngOnInit() {}

  ionViewWillEnter() {
    this.bookings$ = this.bookingsSvc.getFutureBookings();
  }

  goToSchedulePage() {
    this.router.navigate(['user']);
  }

  unbookCourse(bookingId: string) {
    this.loadingSvc
      .startLoading(
        this,
        'UNBOOK_COURSE',
        this.bookingsSvc.deleteBooking(bookingId),
        {
          message: 'Sto rimuovendo la prenotazione',
        }
      )
      .subscribe({
        next: (course) => {
          this.toastSvc.addSuccessToast({
            header: 'Prenotazione rimossa!',
            message: 'Hai rimosso la prenotazione.',
          });
          this.bookings$ = this.bookingsSvc.getFutureBookings();
        },
        error: () => {
          this.toastSvc.addErrorToast({
            message: 'Errore durante la rimozione della prenotazione.',
          });
        },
      });
  }
}
