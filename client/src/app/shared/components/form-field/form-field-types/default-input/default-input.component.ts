import { Component } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { FormField } from '../../models';

@Component({
  selector: 'app-default-input',
  templateUrl: './default-input.component.html',
  styleUrls: ['../input-base-style.scss', './default-input.component.scss'],
})
export class DefaultInputComponent implements FormField {
  type: FormFieldType;
  placeholder: string;
  name: string;
  disabled: boolean;
  valueChange: BehaviorSubject<any>;
  focusChange: BehaviorSubject<'focus' | 'blur'>;

  changeValue(ev: Event) {
    this.valueChange?.next((ev.target as HTMLInputElement).value);
  }

  changeFocus(focus: 'focus' | 'blur') {
    this.focusChange?.next(focus);
  }
}
