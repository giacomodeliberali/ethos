import {
  ChangeDetectionStrategy,
  Component,
  forwardRef,
  Input,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { UserDto } from '@core/services/ethos.generated.service';

@Component({
  selector: 'app-user-selector',
  templateUrl: './user-selector.component.html',
  styleUrls: ['./user-selector.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UserSelectorComponent),
      multi: true,
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserSelectorComponent implements ControlValueAccessor, OnChanges {
  @Input()
  users: (UserDto & { selected?: boolean })[];
  @Input()
  multiSelection = false;
  private _value: string[] | string;
  @Input()
  set value(val: string[] | string) {
    this._value = val;
    this.onChange(this._value);
    this.onTouched();
  }
  get value() {
    return this._value;
  }

  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.value || changes.users) this.setupSelection();
  }

  onChange = (_: any) => {}; // Called on a value change
  onTouched = () => {}; // Called if you care if the form was touched
  onValidatorChange = () => {}; // Called on a validator change or re-validation;

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  writeValue(value: string[] | string): void {
    this.value = value;
  }

  changeValue(val: UserDto) {
    if (this.multiSelection) {
      const currVal = Array.isArray(this.value) ? (this.value as string[]) : [];
      this.value = currVal.includes(val.id)
        ? currVal.filter((x) => x === val.id)
        : [...currVal, val.id];
    } else {
      this.value = this.value === val.id ? null : val.id;
    }
    this.setupSelection();
  }

  setupSelection() {
    if (this.users) {
      if (Array.isArray(this.value)) {
        this.users.forEach(
          (user) => (user.selected = this.value?.includes(user.id))
        );
      } else {
        this.users.forEach((x) => (x.selected = false));
        const user = this.users.find((x) => x.id === this.value);
        if (user) user.selected = true;
      }
    }
  }
}
