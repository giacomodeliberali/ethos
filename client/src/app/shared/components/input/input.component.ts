import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  forwardRef,
  HostBinding,
  Input,
  OnChanges,
  Optional,
  Self,
  SimpleChanges,
} from '@angular/core';
import {
  ControlValueAccessor,
  NgControl,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';

type InputType =
  | 'button'
  | 'checkbox'
  | 'color'
  | 'date'
  | 'datetime-local'
  | 'email'
  | 'file'
  | 'hidden'
  | 'image'
  | 'month'
  | 'number'
  | 'password'
  | 'radio'
  | 'range'
  | 'reset'
  | 'search'
  | 'submit'
  | 'tel'
  | 'text'
  | 'time'
  | 'url'
  | 'week';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR, // Is an InjectionToken required by the ControlValueAccessor interface to provide a form value
      useExisting: forwardRef(() => InputComponent), // tells Angular to use the existing instance
      multi: true,
    },
  ],
})
export class InputComponent implements ControlValueAccessor, OnChanges {
  @Input()
  icon: string;
  @Input()
  type: InputType;
  @Input()
  placeholder: string;
  @Input()
  name: string;
  @Input()
  errorMessage = 'Il campo non è valido';
  // If the input is type text it could be a multiline one (texarea)
  @Input()
  @HostBinding('class.multiline')
  multiline = false;
  @HostBinding('class.input-focused') isFocused = false;
  @HostBinding('class.error') private isNotValid = false;
  showPassword = false;

  private _format: string;
  @Input()
  set format(value: string) {
    if (value) {
      this._format = value;
    } else {
      switch (this.type) {
        case 'date':
          this._format = 'DD/MM/YYYY';
          break;
        case 'time':
          this._format = 'HH:mm';
          break;
        //Add cases if needed with other input types
        default:
          this._format = null;
      }
    }
  }
  get format() {
    return this._format;
  }

  private specialTypes = ['time', 'date'];
  get isSpecialType() {
    return this.specialTypes.includes(this.type);
  }

  private _value: string;
  @Input()
  set value(val: string) {
    this._value = val;
    this.onChange(this._value);
  }
  get value() {
    return this._value;
  }

  get invalid(): boolean {
    this.isNotValid = this.control.invalid;
    if (this.control.invalid) {
      this.elRef.nativeElement.style.setProperty(
        '--line-color',
        'var(--ion-color-danger)'
      );
    } else {
      this.elRef.nativeElement.style.setProperty(
        '--line-color',
        'var(--ion-color-secondary)'
      );
    }
    return this.control ? this.control.invalid : false;
  }

  get showError(): boolean {
    if (!this.control) {
      return false;
    }

    const { dirty, touched } = this.control;

    return this.invalid ? dirty || touched : false;
  }

  constructor(
    @Self() @Optional() public control: NgControl,
    private elRef: ElementRef
  ) {
    if (this.control) {
      this.control.valueAccessor = this;
    }
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

  writeValue(value: string): void {
    this.value = value;
  }

  changeValue(ev: KeyboardEvent | Event) {
    this.value = (ev.target as any).value;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.type) {
      this.format = this.format || null;
    }
  }

  changeFocus(focus: boolean) {
    if (!focus) {
      this.onTouched();
    }
    this.isFocused = focus;
  }

  changeShowPassword() {
    this.showPassword = !this.showPassword;
    this.type = this.showPassword ? 'text' : 'password';
  }
}
