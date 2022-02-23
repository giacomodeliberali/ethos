import { KeyValue } from '@angular/common';
import {
  Component,
  forwardRef,
  Input,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
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
export class WeekDaySelectorComponent
  implements OnChanges, ControlValueAccessor
{
  @Input()
  weekDaysLocalization = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'];

  weekDays: {
    [key: string]: 'mon' | 'tue' | 'wed' | 'thu' | 'fri' | 'sat' | 'sun';
  } = {
    mon: 'mon',
    tue: 'tue',
    wed: 'wed',
    thu: 'thu',
    fri: 'fri',
    sat: 'sat',
    sun: 'sun',
  };

  private _value: string[];
  @Input()
  set value(val: string[]) {
    this._value = val;
    this.onChange(this._value);
    this.onTouched();
  }
  get value() {
    return this._value;
  }

  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.weekDaysLocalization) {
      this.weekDays = {};
      this.weekDays[this.weekDaysLocalization[0]] = 'mon';
      this.weekDays[this.weekDaysLocalization[1]] = 'tue';
      this.weekDays[this.weekDaysLocalization[2]] = 'wed';
      this.weekDays[this.weekDaysLocalization[3]] = 'thu';
      this.weekDays[this.weekDaysLocalization[4]] = 'fri';
      this.weekDays[this.weekDaysLocalization[5]] = 'sat';
      this.weekDays[this.weekDaysLocalization[6]] = 'sun';
    }
  }

  daysOrder = (
    a: KeyValue<string, string>,
    b: KeyValue<string, string>
  ): number =>
    this.weekDaysLocalization.findIndex((x) => x === a.key) <
    this.weekDaysLocalization.findIndex((y) => y === b.key)
      ? -1
      : 1;

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
