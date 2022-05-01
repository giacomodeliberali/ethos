import { Component, ViewChild } from '@angular/core';
import { IonModal } from '@ionic/angular';
import { DefaultInputComponent } from '../default-input/default-input.component';

@Component({
  selector: 'app-date-time-input',
  templateUrl: './date-time-input.component.html',
  styleUrls: ['../input-base-style.scss', './date-time-input.component.scss'],
})
export class DateTimeInputComponent extends DefaultInputComponent {
  presentation: string = 'date';
  minuteValues: string;
  format: string = 'full';
  label: string;
  icon: string;

  constructor() {
    super();
  }

  @ViewChild(IonModal)
  modal: IonModal;

  changeValue(ev: Event): void {
    if (this.presentation == 'date') {
      this.closeModal();
    }
    super.changeValue(ev);
  }

  closeModal() {
    this.modal.dismiss();
  }
}
