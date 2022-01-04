import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-create-edit-schedule-modal',
  templateUrl: './create-edit-schedule-modal.component.html',
  styleUrls: ['./create-edit-schedule-modal.component.scss'],
})
export class CreateEditScheduleModalComponent implements OnInit {
  @Input()
  currentDate: Date;

  get nextWeekDate(): Date {
    const nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);
    return nextWeek;
  }
  scheduleForm: FormGroup = new FormGroup({
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    isRecurrent: new FormControl(false),
    fromDate: new FormControl(null, [Validators.required]),
    toDate: new FormControl(null),
    time: new FormControl(null, [Validators.required]),
  });

  set isRecurrent(val: boolean) {
    this.scheduleForm.get('isRecurrent').setValue(val);
  }

  get isRecurrent(): boolean {
    return this.scheduleForm.get('isRecurrent').value;
  }

  constructor(private modalCtrl: ModalController) {}

  ngOnInit() {
    this.scheduleForm.get('fromDate').setValue(this.currentDate);
    this.scheduleForm.get('toDate').setValue(this.nextWeekDate);
  }

  closeModal(event: 'success' | 'cancel') {
    if (this.scheduleForm.valid && event === 'success') {
      this.modalCtrl.dismiss(this.scheduleForm.value);
    } else if (event === 'cancel') {
      this.modalCtrl.dismiss();
    }
  }
}
