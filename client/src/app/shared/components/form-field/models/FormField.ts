import { BehaviorSubject } from 'rxjs';

export interface FormField {
  placeholder: string;
  type: FormFieldType;
  name: string;
  disabled: boolean;
  valueChange: BehaviorSubject<any>;
  focusChange: BehaviorSubject<'focus' | 'blur'>;
}

export const formFieldInstance: FormField = {
  placeholder: '',
  type: 'text',
  name: '',
  disabled: false,
  valueChange: null,
  focusChange: null,
};
