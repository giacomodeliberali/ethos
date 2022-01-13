import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ComponentFactory,
  ComponentFactoryResolver,
  ComponentRef,
  ElementRef,
  forwardRef,
  HostBinding,
  Input,
  OnChanges,
  SimpleChanges,
  Type,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import { BaseDirective } from '@core/directives';
import { BehaviorSubject } from 'rxjs';
import { distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DateTimeInputComponent } from './form-field-types/date-time-input/date-time-input.component';
import { DefaultInputComponent } from './form-field-types/default-input/default-input.component';
import { InfoInputComponent } from './form-field-types/info-input/info-input.component';
import { PasswordInputComponent } from './form-field-types/password-input/password-input.component';
import { TextareaInputComponent } from './form-field-types/textarea-input/textarea-input.component';
import { FormField, formFieldInstance } from './models';

const typeComponentMap: { [key in FormFieldType]?: Type<FormField> } = {
  textarea: TextareaInputComponent,
  password: PasswordInputComponent,
  datetime: DateTimeInputComponent,
  info: InfoInputComponent,
};

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
export class FormFieldComponent
  extends BaseDirective
  implements ControlValueAccessor, OnChanges, FormField, AfterViewInit
{
  @ViewChild('formFieldTemplateContainer', { read: ViewContainerRef })
  container;
  @Input()
  icon: string;
  @Input()
  type: FormFieldType;
  @Input()
  placeholder: string;
  @Input()
  name: string;
  @Input()
  label: string;
  @Input()
  fieldOptions: any;
  @Input()
  errorMessage = 'Il campo non è valido';
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
  componentRef: ComponentRef<FormField>;
  valueChange: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  focusChange: BehaviorSubject<'focus' | 'blur'> = new BehaviorSubject<any>(
    'blur'
  );

  private _value: any;
  @Input()
  set value(val: any) {
    this._value = val;
    this.onChange(this._value);
    this.onTouched();
  }
  get value() {
    return this._value;
  }

  get invalid(): boolean {
    return this.control ? this.control.invalid : false;
  }

  constructor(
    private elRef: ElementRef,
    private resolver: ComponentFactoryResolver,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    super();
    this.valueChange
      .pipe(takeUntil(this.destroy$), distinctUntilChanged())
      .subscribe((val) => (this.value = val));
    this.focusChange
      .pipe(takeUntil(this.destroy$), distinctUntilChanged())
      .subscribe((focus) => this.changeFocus(focus === 'focus'));
  }

  ngAfterViewInit() {
    this.createFormFieldComponent();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.type && this.componentRef) {
      this.createFormFieldComponent();
    }
    if (changes.value) {
      this.valueChange.next(this.value);
    }
    for (const [key, value] of Object.entries(changes)) {
      if (
        Object.keys(formFieldInstance).includes(key) &&
        this.componentRef?.instance
      ) {
        this.setComponentProperty(key, value);
      }
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
    this.setComponentProperty('disabled', this.disabled);
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

  private createFormFieldComponent() {
    this.container.clear();
    const factory: ComponentFactory<FormField> =
      this.resolver.resolveComponentFactory(
        typeComponentMap[this.type] || DefaultInputComponent
      );
    this.componentRef = this.container.createComponent(factory);
    this.setAllFormFieldComponentProperties();
  }

  private setAllFormFieldComponentProperties() {
    this.setComponentProperty('type', this.type);
    this.setComponentProperty('placeholder', this.placeholder);
    this.setComponentProperty('name', this.name);
    this.setComponentProperty('disabled', this.disabled);
    this.setComponentProperty('valueChange', this.valueChange);
    this.setComponentProperty('focusChange', this.focusChange);
    this.setComponentPropertiesFromObject(this.fieldOptions);
  }

  private setComponentPropertiesFromObject(obj: any) {
    if (obj && this.componentRef) {
      for (const [key, value] of Object.entries(obj)) {
        this.setComponentProperty(key, value);
      }
    }
  }

  private setComponentProperty<T>(name: string, value: T) {
    // Set timeout to avoid detection error
    setTimeout(() => {
      this.componentRef.instance[name] = value;
      this.changeDetectorRef.markForCheck();
    });
  }
}
