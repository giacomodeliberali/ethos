import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

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
    name: new FormControl(),
    description: new FormControl(),
    isRecurrent: new FormControl(),
    fromDate: new FormControl(),
    toDate: new FormControl(),
    time: new FormControl(),
  });

  set isRecurrent(val: boolean) {
    this.scheduleForm.get('isRecurrent').setValue(val);
  }

  get isRecurrent(): boolean {
    return this.scheduleForm.get('isRecurrent').value;
  }

  constructor() {}

  ngOnInit() {
    this.scheduleForm.get('fromDate').setValue(this.currentDate);
    this.scheduleForm.get('toDate').setValue(this.nextWeekDate);
  }
}
