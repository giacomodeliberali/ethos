import { Component } from '@angular/core';
import { DefaultInputComponent } from '../default-input/default-input.component';

@Component({
  selector: 'app-date-time-input',
  templateUrl: './date-time-input.component.html',
  styleUrls: ['../input-base-style.scss', './date-time-input.component.scss'],
})
export class DateTimeInputComponent extends DefaultInputComponent {
  format: string;
  minuteValues: string;
  constructor() {
    super();
  }
}
