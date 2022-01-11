import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-week-day-selector',
  templateUrl: './week-day-selector.component.html',
  styleUrls: ['./week-day-selector.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR, // Is an InjectionToken required by the ControlValueAccessor interface to provide a form value
      useExisting: forwardRef(() => WeekDaySelectorComponent), // tells Angular to use the existing instance
      multi: true,
    },
  ],
})
export class WeekDaySelectorComponent implements ControlValueAccessor {
  @Input()
  value: string[] = [];
  @Input()
  weekDaysLocalization;

  weekDays = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'];

  constructor() {}

  onChange = (_: any) => {}; // Called on a value change
  onTouched = () => {}; // Called if you care if the form was touched
  onValidatorChange = () => {}; // Called on a validator change or re-validation;

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  writeValue(value: string[]): void {
    this.value = value;
  }

  changeValue(val: string) {
    this.value = this.value.includes(val)
      ? this.value.filter((x) => x !== val)
      : [...this.value, val];
  }
}
