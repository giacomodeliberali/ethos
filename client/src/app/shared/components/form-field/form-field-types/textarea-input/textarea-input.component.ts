import { Component } from '@angular/core';
import { DefaultInputComponent } from '../default-input/default-input.component';

@Component({
  selector: 'app-textarea-input',
  templateUrl: './textarea-input.component.html',
  styleUrls: ['./textarea-input.component.scss'],
})
export class TextareaInputComponent extends DefaultInputComponent {
  rows = 3;
  constructor() {
    super();
  }
}
