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
  CreateSingleScheduleRequestDto,
  GeneratedScheduleDto,
  RecurringScheduleOperationType,
  UpdateRecurringScheduleRequestDto,
  UpdateSingleScheduleRequestDto,
  UserDto,
} from '@core/services/ethos.generated.service';
import { ModalController } from '@ionic/angular';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment-timezone';

const daysValidator = (control: FormControl) => {
  const isRecurring = control.parent?.get('isRecurring').value;
  return isRecurring && control.value.length <= 0 ? { required: false } : null;
};

const endDateValidator = (control: FormControl) => {
  const isRecurring = control.parent?.get('isRecurring').value;
  return isRecurring ? Validators.required(control) : null;
};

@Component({
  selector: 'app-create-edit-schedule-modal',
  templateUrl: './create-edit-schedule-modal.component.html',
  styleUrls: ['./create-edit-schedule-modal.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateEditScheduleModalComponent implements OnInit {
  @Input()
  currentDate: string;
  @Input()
  trainers: UserDto[];
  @Input()
  schedule: GeneratedScheduleDto;
  @ViewChild('generalContainer', { read: ElementRef })
  generalContainer: ElementRef;
  _showTrainersSearch = false;
  get showTrainersSearch() {
    return this._showTrainersSearch;
  }

  set showTrainersSearch(value: boolean) {
    if (this.generalContainer) {
      this.generalContainer.nativeElement.style.height =
        this.generalContainer.nativeElement.offsetHeight;
    }
    this._showTrainersSearch = value;
  }

  trainerSearchInput: FormControl = new FormControl(null);

  get nextWeekDate(): string {
    const nextWeek = new Date(this.currentDate);
    nextWeek.setDate(nextWeek.getDate() + 7);
    return nextWeek.toISOString();
  }

  scheduleForm: FormGroup = new FormGroup({
    scheduleId: new FormControl(null),
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    isRecurring: new FormControl(false),
    startDate: new FormControl(null, [Validators.required]),
    endDate: new FormControl(null, [endDateValidator]),
    time: new FormControl(null, [Validators.required]),
    days: new FormControl([], [daysValidator]),
    organizerId: new FormControl(null, [Validators.required]),
    participantsMaxNumber: new FormControl(null, [Validators.required]),
    durationInMinutes: new FormControl(null, [Validators.required]),
    updateInstanceAndFuture: new FormControl(false),
  });

  set isRecurring(val: boolean) {
    this.scheduleForm.get('isRecurring').setValue(val);
    this.scheduleForm.get('days').updateValueAndValidity();
  }

  get isRecurring(): boolean {
    return this.scheduleForm.get('isRecurring').value;
  }

  constructor(
    private modalCtrl: ModalController,
    private toastSvc: ToastService
  ) {}

  ngOnInit() {
    this.scheduleForm.get('startDate').setValue(this.currentDate);
    this.scheduleForm.get('endDate').setValue(this.nextWeekDate);
    this.scheduleForm.patchValue(this.schedule);
    this.scheduleForm.get('time').patchValue(this.schedule?.startDate);
    this.scheduleForm
      .get('organizerId')
      .patchValue(this.schedule?.organizer?.id);
  }

  /**
   * If the user clicked on the save button, prepare the object and send it to the parent else close the modal
   *
   * @param event tell if the user clicked on the success button or the cancel one
   * @returns nothing
   */
  closeModal(event: 'success' | 'cancel') {
    if (event === 'cancel') {
      this.modalCtrl.dismiss();
      return;
    }

    if (!this.scheduleForm.valid) {
      this.toastSvc.addErrorToast({
        message: 'Controlla di avere compilato correttamente i campi',
      });
      return;
    }

    const isRecurring = this.scheduleForm.get('isRecurring').value;

    let schedule:
      | CreateSingleScheduleRequestDto
      | CreateRecurringScheduleRequestDto
      | UpdateSingleScheduleRequestDto
      | UpdateRecurringScheduleRequestDto = null;

    if (isRecurring) {
      if (this.schedule) {
        // update
        schedule = this.updateRecurringSchedule();
      } else {
        schedule = this.createRecurringSchedule();
      }
    } else {
      if (this.schedule) {
        // update
        schedule = this.updateSingleSchedule();
      } else {
        schedule = this.createSingleSchedule();
      }
    }
    this.modalCtrl.dismiss({
      schedule,
      isRecurring,
    });
  }

  toggleClicked(event: Event) {
    event.stopPropagation();
    this.isRecurring = !this.isRecurring;
  }

  private createSingleSchedule(): CreateSingleScheduleRequestDto {
    return {
      description: this.scheduleForm.get('description').value,
      startDate: moment(this.scheduleForm.get('startDate').value)
        .set({
          hour: new Date(this.scheduleForm.get('time').value).getHours(),
          minute: new Date(this.scheduleForm.get('time').value).getMinutes(),
          second: 0,
          millisecond: 0,
        })
        .tz("Europe/Rome")
        .format(),
      name: this.scheduleForm.get('name').value,
      organizerId: this.scheduleForm.get('organizerId').value,
      participantsMaxNumber: this.scheduleForm.get('participantsMaxNumber')
        .value,
      durationInMinutes: this.scheduleForm.get('durationInMinutes').value,
    };
  }

  private updateSingleSchedule(): UpdateSingleScheduleRequestDto {
    return {
      id: this.scheduleForm.get('scheduleId').value,
      description: this.scheduleForm.get('description').value,
      startDate: moment(this.scheduleForm.get('startDate').value)
        .set({
          hour: new Date(this.scheduleForm.get('time').value).getHours(),
          minute: new Date(this.scheduleForm.get('time').value).getMinutes(),
          second: 0,
          millisecond: 0,
        })
        .tz("Europe/Rome")
        .format(),
      name: this.scheduleForm.get('name').value,
      organizerId: this.scheduleForm.get('organizerId').value,
      participantsMaxNumber: this.scheduleForm.get('participantsMaxNumber')
        .value,
      durationInMinutes: this.scheduleForm.get('durationInMinutes').value,
    };
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
    };
  }

  private updateRecurringSchedule(): UpdateRecurringScheduleRequestDto {
    const updateInstanceAndFuture = this.scheduleForm.get(
      'updateInstanceAndFuture'
    ).value as boolean;

    return {
      id: this.scheduleForm.get('scheduleId').value,
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
      instanceEndDate: this.schedule.startDate,
      instanceStartDate: this.schedule.endDate,
      recurringScheduleOperationType: updateInstanceAndFuture
        ? RecurringScheduleOperationType.InstanceAndFuture
        : RecurringScheduleOperationType.Instance,
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
