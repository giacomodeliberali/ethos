import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { BaseDirective } from '@core/directives';
import {
  CreateRecurringScheduleRequestDto,
  CreateSingleScheduleRequestDto,
  GeneratedScheduleDto,
  GeneratedScheduleDto_BookingDto,
  IdentityService,
  RecurringScheduleOperationType,
  RecurringSchedulesService,
  SchedulesService,
  SingleSchedulesService,
  UpdateRecurringScheduleRequestDto,
  UpdateSingleScheduleRequestDto,
  UserDto,
} from '@core/services/ethos.generated.service';
import { MediaService } from '@core/services/media.service';
import { SettingsService } from '@core/services/settings.service';
import { UserService } from '@core/services/user.service';
import { IonModal, ModalController } from '@ionic/angular';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment';
import { of } from 'rxjs';
import { catchError, finalize, map } from 'rxjs/operators';
import { CreateEditScheduleModalComponent } from '../../components/create-edit-schedule-modal/create-edit-schedule-modal.component';
import { DeleteScheduleModalComponent } from '../../components/delete-schedule-modal/delete-schedule-modal.component';
import { ShowBookingModalComponent } from '../../components/show-booking-modal/show-booking-modal.component';

@Component({
  selector: 'app-admin-schedules-page',
  templateUrl: './admin-schedules-page.component.html',
  styleUrls: ['./admin-schedules-page.component.scss'],
})
export class AdminSchedulesPageComponent extends BaseDirective {
  trainers: UserDto[];

  schedules: {
    morning: Array<GeneratedScheduleDto>;
    afternoon: Array<GeneratedScheduleDto>;
    evening: Array<GeneratedScheduleDto>;
  };

  @ViewChild(IonModal)
  modal: IonModal;

  private _currentDate: string;
  set currentDate(date: string) {
    if (this.currentDate !== date || !this.currentDate) {
      this.loadSchedules(date);
    }
    this._currentDate = date;
  }
  get currentDate() {
    return this._currentDate;
  }

  constructor(
    public mediaSvc: MediaService,
    private schedulesSvc: SchedulesService,
    private singleScheduleSvc: SingleSchedulesService,
    private recurringScheduleSvc: RecurringSchedulesService,
    private loadingSvc: LoadingService,
    private settingsSvc: SettingsService,
    private identitySvc: IdentityService,
    private modalCtrl: ModalController,
    private toastSvc: ToastService,
    private router: Router,
    private userSvc: UserService
  ) {
    super();
    this.currentDate = moment().toDate().toISOString();
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

  async openCreateEditModal(
    trainers: UserDto[],
    schedule?: GeneratedScheduleDto
  ) {
    const editModal = await this.modalCtrl.create({
      component: CreateEditScheduleModalComponent,
      componentProps: { currentDate: this.currentDate, trainers, schedule },
      canDismiss: false,
      backdropDismiss: false,
      mode: 'ios',
    });
    await editModal.present();

    const { data } = await editModal.onWillDismiss<{
      schedule: any;
      isRecurring: boolean;
    }>();

    if (data) {
      if (schedule) {
        this.callUpdateSchedule(data.schedule, schedule.isRecurring);
      } else {
        this.callCreateSchedule(data.schedule, data.isRecurring);
      }
    }
  }

  callCreateSchedule(
    schedule: CreateSingleScheduleRequestDto &
      CreateRecurringScheduleRequestDto,
    isRecurring: boolean
  ) {
    const createSchedule = isRecurring
      ? this.recurringScheduleSvc.createRecurringSchedule(schedule)
      : this.singleScheduleSvc.createSingleSchedule(schedule);

    createSchedule.subscribe({
      next: (result) => {
        this.toastSvc.addSuccessToast({
          header: 'Corso creato!',
          message: 'Il corso è stato creato con successo',
        });
        this.loadSchedules(this.currentDate);
      },
      error: (result) => {
        this.toastSvc.addErrorToast({
          message:
            result?.error?.message || 'Errore durante la creazione del corso',
        });
      },
    });
  }

  callUpdateSchedule(
    schedule: UpdateSingleScheduleRequestDto &
      UpdateRecurringScheduleRequestDto,
    isRecurring: boolean
  ) {
    const updateSchedule = isRecurring
      ? this.recurringScheduleSvc.updateRecurringSchedule(schedule)
      : this.singleScheduleSvc.updateSingleSchedule(schedule);

    updateSchedule.subscribe({
      next: () => {
        this.toastSvc.addSuccessToast({
          header: 'Corso modificato!',
          message: 'Il corso è stato modificato con successo',
        });
        this.loadSchedules(this.currentDate);
      },
      error: (result) => {
        this.toastSvc.addErrorToast({
          message:
            result?.error?.message || 'Errore durante la modifica del corso',
        });
      },
    });
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

  enterEditMode(schedule: GeneratedScheduleDto) {
    this.openEditModal(schedule);
  }

  enterAddMode(e: MouseEvent) {
    e?.stopImmediatePropagation();
    this.openEditModal();
  }

  async enterDeleteMode(schedule: GeneratedScheduleDto) {
    const showDeleteModal = await this.modalCtrl.create({
      component: DeleteScheduleModalComponent,
      componentProps: { isRecurring: schedule.isRecurring },
      canDismiss: true,
      backdropDismiss: true,
      breakpoints: [0.35, 0.5],
      initialBreakpoint: 0.35,
    });
    await showDeleteModal.present();
    const { data } = await showDeleteModal.onWillDismiss();
    if (data != null) {
      const deleteSchedule = schedule.isRecurring
        ? this.recurringScheduleSvc.deleteRecurringSchedule(
            schedule.scheduleId,
            {
              id: schedule.scheduleId,
              recurringScheduleOperationType: data
                ? RecurringScheduleOperationType.InstanceAndFuture
                : RecurringScheduleOperationType.Instance,
              instanceStartDate: schedule.startDate,
              instanceEndDate: schedule.endDate,
            }
          )
        : this.singleScheduleSvc.deleteSingleSchedule(schedule.scheduleId, {
            id: schedule.scheduleId,
          });

      this.loadingSvc
        .startLoading(this, 'DELETE_SCHEDULE', deleteSchedule, {
          message: 'Sto eliminando il corso.',
        })
        .subscribe({
          next: () => {
            this.loadSchedules(this.currentDate);
          },
          error: (result) => {
            this.toastSvc.addErrorToast({
              message: result.error.message,
            });
          },
        });
    }
  }

  async showBookings(bookings: GeneratedScheduleDto_BookingDto[]) {
    const showBookingsModal = await this.modalCtrl.create({
      component: ShowBookingModalComponent,
      componentProps: { bookings },
      canDismiss: true,
      backdropDismiss: true,
      breakpoints: [0.35, 1],
      initialBreakpoint: 0.35,
    });
    await showBookingsModal.present();
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

  openEditModal(schedule?: GeneratedScheduleDto) {
    (this.trainers
      ? of(this.trainers)
      : this.loadingSvc.startLoading(
          this,
          'GET_ADMINS',
          this.identitySvc.getAllAdmins(),
          {
            message: 'Sto caricando',
          }
        )
    ).subscribe((trainers) => {
      this.trainers = trainers;
      this.openCreateEditModal(trainers, schedule);
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
