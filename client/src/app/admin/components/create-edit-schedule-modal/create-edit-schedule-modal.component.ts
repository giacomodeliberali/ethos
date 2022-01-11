import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserDto } from '@core/services/ethos.generated.service';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-create-edit-schedule-modal',
  templateUrl: './create-edit-schedule-modal.component.html',
  styleUrls: ['./create-edit-schedule-modal.component.scss'],
})
export class CreateEditScheduleModalComponent implements OnInit {
  @Input()
  currentDate: string;
  @Input()
  trainers: UserDto[];
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
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    isRecurrent: new FormControl(false),
    startDate: new FormControl(null, [Validators.required]),
    endDate: new FormControl(null),
    time: new FormControl(null, [Validators.required]),
    days: new FormControl([], [Validators.required]),
    organizerId: new FormControl(null, [Validators.required]),
    participantsMaxNumber: new FormControl(null, [Validators.required]),
  });

  set isRecurrent(val: boolean) {
    this.scheduleForm.get('isRecurrent').setValue(val);
  }

  get isRecurrent(): boolean {
    return this.scheduleForm.get('isRecurrent').value;
  }

  constructor(private modalCtrl: ModalController) {}

  ngOnInit() {
    this.scheduleForm.get('startDate').setValue(this.currentDate);
    this.scheduleForm.get('endDate').setValue(this.nextWeekDate);
  }

  closeModal(event: 'success' | 'cancel') {
    if (this.scheduleForm.valid && event === 'success') {
      this.modalCtrl.dismiss(this.scheduleForm.value);
    } else if (event === 'cancel') {
      this.modalCtrl.dismiss();
    }
  }
}
