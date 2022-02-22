import { Component } from '@angular/core';
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
import { ModalController } from '@ionic/angular';
import { LogoutModalComponent } from '@shared/components/logout-modal/logout-modal.component';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment';
import { of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent extends BaseDirective {
  user: UserDto;

  schedules: {
    morning: Array<GeneratedScheduleDto>;
    afternoon: Array<GeneratedScheduleDto>;
    evening: Array<GeneratedScheduleDto>;
  };

  private _currentDate: string;
  set currentDate(date: string) {
    this._currentDate = date;
    this.loadSchedules(date);
  }
  get currentDate() {
    return this._currentDate;
  }

  get dateLimits() {
    const nextDate = moment(new Date()).add(5, 'days');
    return {
      lowerLimit: moment(new Date()).format('YYYY-MM-DD'),
      upperLimit: nextDate.format('YYYY-MM-DD'),
    };
  }

  constructor(
    public mediaSvc: MediaService,
    private schedulesSvc: SchedulesService,
    private loadingSvc: LoadingService,
    private settingsSvc: SettingsService,
    private userSvc: UserService,
    private toastSvc: ToastService,
    private router: Router,
    private bookingsSvc: BookingsService,
    private modalCtrl: ModalController
  ) {
    super();
    this.currentDate = moment().toDate().toISOString();
    this.user = this.userSvc.getUser();
  }

  loadSchedules(date: string) {
    const startDate = moment(date);
    const endDate = moment(date);
    startDate.set({
      hour: 0,
      minute: 0,
      second: 0,
      millisecond: 0,
    });
    endDate.set({
      hour: 23,
      minute: 59,
      second: 59,
      millisecond: 0,
    });
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

  goToUserSettings() {
    this.router.navigate(['admin', 'user-settings']);
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

  async openLogoutModal() {
    const logoutModal = await this.modalCtrl.create({
      component: LogoutModalComponent,
      cssClass: MediaService.isSmartphone ? 'bottom' : '',
      swipeToClose: true,
      mode: 'ios',
    });
    await logoutModal.present();
    const { data } = await logoutModal.onWillDismiss();
    if (data.logout) {
      this.userSvc.removeOldAuthentication();
      this.router.navigate(['']);
    }
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
}
