import { ChangeDetectionStrategy, Component, ElementRef, forwardRef, HostBinding, Input, OnChanges, SimpleChanges } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: "app-input",
  templateUrl: "./input.component.html",
  styleUrls: ["./input.component.scss"],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR, // Is an InjectionToken required by the ControlValueAccessor interface to provide a form value
      useExisting: forwardRef(() => InputComponent), // tells Angular to use the existing instance
      multi: true
    }
  ]
})
export class InputComponent implements ControlValueAccessor, OnChanges {
  @Input()
  icon: string;
  @Input()
  type: string;
  @Input()
  placeholder: string;
  @Input()
  name: string;
  @Input()
  isValid = true;
  @Input()
  errorMessage = "Il campo non è valido";
  @Input()
  @HostBinding("class.multiline")
  multiline = false;
  @HostBinding("class.input-focused") isFocused = false;
  @HostBinding("class.error") _isNotValid = false;

  showPassword = false;

  private _value: string;
  @Input()
  set value(val: string) {
    this._value = val;
    this.onChange(this._value);
  }

  get value() {
    return this._value;
  }

  constructor(private elRef: ElementRef) {}

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

  changeValue(ev: KeyboardEvent) {
    this.value = (ev.target as any).value;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.isValid) {
      this._isNotValid = !changes.isValid.currentValue;
      if (changes.isValid.currentValue === false) {
        this.elRef.nativeElement.style.setProperty("--line-color", "var(--ion-color-danger)");
      } else {
        this.elRef.nativeElement.style.setProperty("--line-color", "var(--ion-color-secondary)");
      }
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
    this.type = this.showPassword ? "text" : "password";
  }
}
