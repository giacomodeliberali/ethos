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
  CreateSingleScheduleRequestDto,
  CreateRecurringScheduleRequestDto,
  GeneratedScheduleDto,
  UpdateSingleScheduleRequestDto,
  UserDto,
  UpdateRecurringScheduleInstanceRequestDto,
} from '@core/services/ethos.generated.service';
import { ModalController } from '@ionic/angular';
import { ToastService } from '@shared/services/toast.service';
import moment from 'moment';

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
    if (this.scheduleForm.valid && event === 'success') {
      if (this.scheduleForm.valid) {
        const schedule: Partial<
          CreateSingleScheduleRequestDto &
            CreateRecurringScheduleRequestDto &
            UpdateSingleScheduleRequestDto &
            UpdateRecurringScheduleInstanceRequestDto
        > = {
          id: this.scheduleForm.get('scheduleId').value,
          description: this.scheduleForm.get('description').value,
          startDate: moment(this.scheduleForm.get('startDate').value)
            .set({
              hour: this.isRecurring
                ? 0
                : new Date(this.scheduleForm.get('time').value).getHours(),
              minute: this.isRecurring
                ? 0
                : new Date(this.scheduleForm.get('time').value).getMinutes(),
              second: 0,
              millisecond: 0,
            })
            .toISOString(),
          name: this.scheduleForm.get('name').value,
          organizerId: this.scheduleForm.get('organizerId').value,
          participantsMaxNumber: this.scheduleForm.get('participantsMaxNumber')
            .value,
          durationInMinutes: this.scheduleForm.get('durationInMinutes').value,
          endDate: null,
        };
        if (this.scheduleForm.get('isRecurring').value) {
          schedule.endDate = moment(this.scheduleForm.get('endDate').value)
            .set({
              hour: 23,
              minute: 59,
              second: 59,
              millisecond: 0,
            })
            .toISOString();

          schedule.recurringCronExpression = this.createCronExpression(
            this.scheduleForm.get('days').value as string[],
            this.scheduleForm.get('time').value as Date
          );
        }
        this.modalCtrl.dismiss({
          schedule,
          isRecurring: this.scheduleForm.get('isRecurring').value,
        });
        return;
      }
      this.toastSvc.addErrorToast({
        message: 'Controlla di avere compilato correttamente i campi',
      });
    } else if (event === 'cancel') {
      this.modalCtrl.dismiss();
    }
  }

  toggleClicked(event: Event) {
    event.stopPropagation();
    this.isRecurring = !this.isRecurring;
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
    const timeString = moment.utc(time);
    const hour = timeString.hour();
    const minute = timeString.minute();
    console.log(`${minute} ${hour} ? * ${daysString}`);
    return `${minute} ${hour} ? * ${daysString}`;
  }
}
