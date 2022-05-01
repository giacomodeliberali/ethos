import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { BaseDirective } from '@core/directives';
import {
  BookingsService,
  GeneratedScheduleDto,
  SchedulesService,
  UserDto,
} from '@core/services/ethos.generated.service';
import { MediaService } from '@core/services/media.service';
import { SettingsService } from '@core/services/settings.service';
import { UserService } from '@core/services/user.service';
import { IonModal } from '@ionic/angular';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment';
import { of } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';

@Component({
  selector: 'app-user-schedules-page',
  templateUrl: './user-schedules-page.component.html',
  styleUrls: ['./user-schedules-page.component.scss'],
})
export class UserSchedulesPageComponent extends BaseDirective {
  user: UserDto;

  schedules: {
    morning: Array<GeneratedScheduleDto>;
    afternoon: Array<GeneratedScheduleDto>;
    evening: Array<GeneratedScheduleDto>;
  };

  @ViewChild(IonModal)
  modal: IonModal;

  private _currentDate: string;
  set currentDate(date: string) {
    this._currentDate = date;
    this.loadSchedules(date);
  }
  get currentDate() {
    return this._currentDate;
  }

  get dateLimits() {
    const nextDate = moment(new Date()).add(14, 'days');
    return {
      lowerLimit: moment(new Date()).format('YYYY-MM-DD'),
      upperLimit: nextDate.format('YYYY-MM-DD'),
    };
  }

  get hideLeftArrow() {
    return (
      moment(this.currentDate).subtract(1, 'd') <
      moment(this.dateLimits.lowerLimit)
    );
  }

  get hideRightArrow() {
    return (
      moment(this.currentDate).add(1, 'd') > moment(this.dateLimits.upperLimit)
    );
  }

  constructor(
    public mediaSvc: MediaService,
    private schedulesSvc: SchedulesService,
    private loadingSvc: LoadingService,
    private settingsSvc: SettingsService,
    private userSvc: UserService,
    private toastSvc: ToastService,
    private router: Router,
    private bookingsSvc: BookingsService
  ) {
    super();
    this.currentDate = moment().toDate().toISOString();
    this.user = this.userSvc.getUser();
  }

  loadSchedules(date: string) {
    const startDate = moment(date);
    const endDate = moment(date);
    this.loadingSvc
      .startLoading(
        this,
        'GET_SCHEDULES',
        this.schedulesSvc.getAllSchedulesInRange(
          startDate.toISOString(),
          endDate.toISOString()
        ),
        {
          message: 'Sto caricando gli allenamenti della giornata',
        }
      )
      .pipe(
        catchError((error) => {
          this.toastSvc.addErrorToast({
            message: 'Errore durante la richiesta dei corsi',
          });
          return of([]);
        }),
        map((schedules) => this.divideScheduleByDayPeriod(schedules))
      )
      .subscribe((schedules) => (this.schedules = schedules));
  }

  divideScheduleByDayPeriod(schedules: GeneratedScheduleDto[]) {
    const schedulesByDayPortion = {
      morning: [],
      afternoon: [],
      evening: [],
    };
    for (const schedule of schedules) {
      const currentDate = new Date(schedule.startDate);
      const hour = currentDate.getHours();
      const minutes = currentDate.getMinutes();
      if (
        hour >= this.settingsSvc.eveningStart.hour &&
        minutes >= this.settingsSvc.eveningStart.minutes
      ) {
        schedulesByDayPortion.evening.push(schedule);
        continue;
      }
      if (
        hour >= this.settingsSvc.afternoonStart.hour &&
        minutes >= this.settingsSvc.afternoonStart.minutes
      ) {
        schedulesByDayPortion.afternoon.push(schedule);
        continue;
      }
      schedulesByDayPortion.morning.push(schedule);
    }
    return schedulesByDayPortion;
  }

  bookCourse(schedule: GeneratedScheduleDto) {
    this.loadingSvc
      .startLoading(
        this,
        'BOOK_COURSE',
        this.bookingsSvc.createBooking(schedule),
        {
          message: 'Sto prenotando il corso.',
        }
      )
      .subscribe({
        next: (course) => {
          this.toastSvc.addSuccessToast({
            header: 'Corso prenotato!',
            message: 'Hai prenotato il corso correttamente.',
          });
          this.loadSchedules(this.currentDate);
        },
        error: () => {
          this.toastSvc.addErrorToast({
            message: 'Errore durante la prenotazione.',
          });
        },
      });
  }

  goToNextDay() {
    this.currentDate = moment(this.currentDate).add(1, 'd').toISOString();
  }

  goToPrevDay() {
    this.currentDate = moment(this.currentDate).subtract(1, 'd').toISOString();
  }

  goToToday() {
    this.currentDate = new Date().toISOString();
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
          this.loadSchedules(this.currentDate);
        },
        error: () => {
          this.toastSvc.addErrorToast({
            message: 'Errore durante la rimozione della prenotazione.',
          });
        },
      });
  }

  refreshCurrentDate(
    event: CustomEvent & { target: { complete: () => void } }
  ) {
    const date = moment(this.currentDate);

    this.schedulesSvc
      .getAllSchedulesInRange(date.toISOString(), date.toISOString())
      .pipe(
        catchError((error) => {
          event.target.complete();
          this.toastSvc.addErrorToast({
            message: 'Errore durante la richiesta dei corsi',
          });
          return of([]);
        }),
        map((schedules) => this.divideScheduleByDayPeriod(schedules))
      )
      .subscribe((schedules) => {
        this.schedules = schedules;
        event.target.complete();
      });
  }

  onDateChange(event: CustomEvent) {
    this.currentDate = event.detail.value;
    this.modal.dismiss();
  }
}
