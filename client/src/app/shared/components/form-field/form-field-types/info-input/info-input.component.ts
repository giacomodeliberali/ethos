import { Component, Input } from '@angular/core';
import { DefaultInputComponent } from '../default-input/default-input.component';

@Component({
  selector: 'app-info-input',
  templateUrl: './info-input.component.html',
  styleUrls: ['../input-base-style.scss', './info-input.component.scss'],
})
export class InfoInputComponent extends DefaultInputComponent {
  @Input()
  info: string;
  @Input()
  inputType: string;
  constructor() {
    super();
  }
}
