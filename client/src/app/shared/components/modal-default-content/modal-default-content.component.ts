import { Component } from '@angular/core';

@Component({
  selector: '[modal-default-content]',
  templateUrl: './modal-default-content.component.html',
  styleUrls: ['./modal-default-content.component.scss'],
  host: {
    class: 'inner-content'
  }
})
export class ModalDefaultContentComponent {}
