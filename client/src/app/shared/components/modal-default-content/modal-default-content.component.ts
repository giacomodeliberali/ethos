import { Component } from '@angular/core';

@Component({
  selector: 'modal-default-content',
  templateUrl: './modal-default-content.component.html',
  styleUrls: ['./modal-default-content.component.scss'],
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: {
    class: 'inner-content',
  },
})
export class ModalDefaultContentComponent {}
