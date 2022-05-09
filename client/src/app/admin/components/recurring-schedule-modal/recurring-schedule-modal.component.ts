import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
  CreateRecurringScheduleRequestDto,
  GeneratedScheduleDto,
  RecurringScheduleOperationType,
  UpdateRecurringScheduleRequestDto,
  UserDto,
} from '@core/services/ethos.generated.service';
import { ModalController } from '@ionic/angular';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment-timezone';

@Component({
  selector: 'app-recurring-schedule-modal',
  templateUrl: './recurring-schedule-modal.component.html',
  styleUrls: ['./recurring-schedule-modal.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RecurringScheduleModalComponent implements OnInit {
  @Input()
  currentDate: string;
  @Input()
  trainers: UserDto[];
  @Input()
  schedule: GeneratedScheduleDto;

  get nextWeekDate(): string {
    const nextWeek = new Date(this.currentDate);
    nextWeek.setDate(nextWeek.getDate() + 7);
    return nextWeek.toISOString();
  }

  scheduleForm: FormGroup = new FormGroup({
    scheduleId: new FormControl(null),
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    startDate: new FormControl(null, [Validators.required]),
    endDate: new FormControl(null, [Validators.required]),
    time: new FormControl(null, [Validators.required]),
    // days: new FormControl([], [daysValidator]),
    organizerId: new FormControl(null, [Validators.required]),
    participantsMaxNumber: new FormControl(null, [Validators.required]),
    durationInMinutes: new FormControl(null, [Validators.required]),
    updateInstanceAndFuture: new FormControl(false),
  });

  constructor(
    private modalCtrl: ModalController,
    private toastSvc: ToastService
  ) {}

  ngOnInit() {
    this.scheduleForm.get('startDate').setValue(this.currentDate);
    this.scheduleForm.get('endDate').setValue(this.nextWeekDate);
    this.scheduleForm.patchValue(this.schedule);
    this.scheduleForm
      .get('time')
      .patchValue(
        this.schedule?.startDate || moment().startOf('hour').toISOString()
      );
    this.scheduleForm
      .get('organizerId')
      .patchValue(this.schedule?.organizer?.id);

    this.scheduleForm.get('durationInMinutes').setValue(60);
    this.scheduleForm.get('participantsMaxNumber').setValue(15);
  }

  /**
   * If the user clicked on the save button, prepare the object and send it to the parent else close the modal
   *
   * @param event tell if the user clicked on the success button or the cancel one
   * @returns nothing
   */
  async closeModal(event: 'success' | 'cancel') {
    const modal = await this.modalCtrl.getTop();
    if (event === 'cancel') {
      modal.canDismiss = true;
      await this.modalCtrl.dismiss();
      return;
    }

    if (!this.scheduleForm.valid) {
      this.toastSvc.addErrorToast({
        message: 'Controlla di avere compilato correttamente i campi',
      });
      return;
    }

    let schedule:
      | CreateRecurringScheduleRequestDto
      | UpdateRecurringScheduleRequestDto = null;

    if (this.schedule) {
      // update
      schedule = this.updateRecurringSchedule();
    } else {
      schedule = this.createRecurringSchedule();
    }

    modal.canDismiss = true;
    this.modalCtrl.dismiss({
      schedule,
      isRecurring: true,
    });
  }

  private createRecurringSchedule(): CreateRecurringScheduleRequestDto {
    return {
      description: this.scheduleForm.get('description').value,
      startDate: moment(this.scheduleForm.get('startDate').value).toISOString(),
      endDate: moment(this.scheduleForm.get('endDate').value).toISOString(),
      name: this.scheduleForm.get('name').value,
      organizerId: this.scheduleForm.get('organizerId').value,
      participantsMaxNumber: this.scheduleForm.get('participantsMaxNumber')
        .value,
      durationInMinutes: this.scheduleForm.get('durationInMinutes').value,
      recurringCronExpression: this.createCronExpression(
        this.scheduleForm.get('days').value as string[],
        this.scheduleForm.get('time').value as Date
      ),
      timeZone: 'Europe/Rome',
    };
  }

  private updateRecurringSchedule(): UpdateRecurringScheduleRequestDto {
    return {
      id: this.scheduleForm.get('scheduleId').value,
      description: this.scheduleForm.get('description').value,
      startDate: moment(this.scheduleForm.get('startDate').value).toISOString(),
      endDate: this.schedule.endDate, // moment(this.scheduleForm.get('endDate').value).toISOString(),
      name: this.scheduleForm.get('name').value,
      organizerId: this.scheduleForm.get('organizerId').value,
      participantsMaxNumber: this.scheduleForm.get('participantsMaxNumber')
        .value,
      durationInMinutes: this.scheduleForm.get('durationInMinutes').value,
      recurringCronExpression: this.schedule.recurringCronExpression,
      instanceEndDate: this.schedule.startDate,
      instanceStartDate: this.schedule.endDate,
      recurringScheduleOperationType: RecurringScheduleOperationType.Instance, // TODO: only support for instance atm
    };
  }

  /**
   * Create a CRON expression from given days and a time
   *
   * @param days the week days to use
   * @param time the time to use
   * @returns The CRON expression
   */
  private createCronExpression(days: string[], time: Date) {
    const daysString = days.map((x) => x.toUpperCase()).join(',');
    const timeString = moment(time);
    const hour = timeString.hour();
    const minute = timeString.minute();
    console.log(`${minute} ${hour} ? * ${daysString}`);
    return `${minute} ${hour} ? * ${daysString}`;
  }
}
