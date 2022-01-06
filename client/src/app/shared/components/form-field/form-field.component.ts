import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  forwardRef,
  HostBinding,
  Input,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
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
  selector: 'app-form-field',
  templateUrl: './form-field.component.html',
  styleUrls: ['./form-field.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR, // Is an InjectionToken required by the ControlValueAccessor interface to provide a form value
      useExisting: forwardRef(() => FormFieldComponent), // tells Angular to use the existing instance
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => FormFieldComponent),
      multi: true,
    },
  ],
})
export class FormFieldComponent implements ControlValueAccessor, OnChanges {
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
  @HostBinding('class.error')
  get showError(): boolean {
    if (!this.control) {
      return false;
    }
    const { dirty, touched } = this.control;
    const invalidity = this.invalid && (dirty || touched);
    if (invalidity) {
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
    return invalidity;
  }

  showPassword = false;
  control: FormControl;
  disabled = false;

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
    return this.control ? this.control.invalid : false;
  }

  constructor(private elRef: ElementRef) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes.type) {
      this.format = this.format || null;
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

  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  changeValue(ev: KeyboardEvent | Event) {
    this.value = (ev.target as any).value;
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

  validate(c: FormControl) {
    if (!this.control) this.control = c;
    return this.control.valid;
  }
}
