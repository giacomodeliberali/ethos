import { Component } from '@angular/core';
import { BaseDirective } from '@core/directives';
import {
  CreateScheduleRequestDto,
  GeneratedScheduleDto,
  IdentityService,
  SchedulesService,
  UpdateSingleScheduleRequestDto,
  UserDto,
} from '@core/services/ethos.generated.service';
import { MediaService } from '@core/services/media.service';
import { SettingsService } from '@core/services/settings.service';
import { ModalController } from '@ionic/angular';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment';
import { of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { CreateEditScheduleModalComponent } from '../../components/create-edit-schedule-modal/create-edit-schedule-modal.component';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.scss'],
})
export class AdminPageComponent extends BaseDirective {
  editModal: HTMLIonModalElement;
  trainers: UserDto[];

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

  constructor(
    public mediaSvc: MediaService,
    private schedulesSvc: SchedulesService,
    private loadingSvc: LoadingService,
    private settingsSvc: SettingsService,
    private identitySvc: IdentityService,
    private modalCtrl: ModalController,
    private toastSvc: ToastService
  ) {
    super();
    this.currentDate = moment().toDate().toISOString();
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

  async openCreateEditModal(
    trainers: UserDto[],
    schedule?: GeneratedScheduleDto
  ) {
    this.editModal = await this.modalCtrl.create({
      component: CreateEditScheduleModalComponent,
      componentProps: { currentDate: this.currentDate, trainers, schedule },
      cssClass: MediaService.isSmartphone ? 'bottom' : '',
      swipeToClose: true,
      mode: 'ios',
    });
    await this.editModal.present();
    const { data } = await this.editModal.onWillDismiss();
    if (data) {
      if (schedule) {
        this.callUpdateSchedule(data);
      } else {
        this.callCreateSchedule(data);
      }
    }
  }

  callCreateSchedule(schedule: CreateScheduleRequestDto) {
    this.schedulesSvc.createSchedule(schedule).subscribe({
      next: (result) => {
        this.toastSvc.addSuccessToast({
          header: 'Corso creato!',
          message: 'Il corso è stato creato con successo',
        });
        this.loadSchedules(this.currentDate);
      },
      error: () => {
        this.toastSvc.addErrorToast({
          message: 'Errore durante la creazione del corso',
        });
      },
    });
  }

  callUpdateSchedule(schedule: UpdateSingleScheduleRequestDto) {
    this.schedulesSvc.updateSchedule(schedule).subscribe({
      next: (result) => {
        this.toastSvc.addSuccessToast({
          header: 'Corso modificato!',
          message: 'Il corso è stato modificato con successo',
        });
        this.loadSchedules(this.currentDate);
      },
      error: () => {
        this.toastSvc.addErrorToast({
          message: 'Errore durante la modifica del corso',
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

  editButtonClicked(schedule: GeneratedScheduleDto) {
    console.log(schedule);
    this.enterEditMode(schedule);
  }

  addButtonClicked(e: MouseEvent) {
    e?.stopImmediatePropagation();
    this.enterEditMode();
  }

  private enterEditMode(schedule?: GeneratedScheduleDto) {
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
}
