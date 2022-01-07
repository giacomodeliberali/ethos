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
import { takeUntil } from 'rxjs/operators';
import { DefaultInputComponent } from './form-field-types/default-input/default-input.component';
import { PasswordInputComponent } from './form-field-types/password-input/password-input.component';
import { FormField, formFieldInstance } from './models';

const typeComponentMap: Map<FormFieldType, Type<FormField>> = new Map([
  ['password', PasswordInputComponent],
]);

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
  fieldOptions: any;
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
  componentRef: ComponentRef<FormField>;
  valueChange: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  focusChange: BehaviorSubject<'focus' | 'blur'> = new BehaviorSubject<any>(
    'blur'
  );

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

  private _value: any;
  @Input()
  set value(val: any) {
    this._value = val;
    this.onChange(this._value);
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
      .pipe(takeUntil(this.destroy$))
      .subscribe((val) => (this.value = val));
    this.focusChange
      .pipe(takeUntil(this.destroy$))
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
        typeComponentMap.get(this.type) || DefaultInputComponent
      );
    this.componentRef = this.container.createComponent(factory);
    console.log(this.componentRef);
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
    if (obj) {
      for (const [key, value] of Object.entries(obj)) {
        this.setComponentProperty(key, value);
      }
    }
  }

  private setComponentProperty<T>(name: string, value: T) {
    this.componentRef.instance[name] = value;
    this.changeDetectorRef.markForCheck();
  }
}
