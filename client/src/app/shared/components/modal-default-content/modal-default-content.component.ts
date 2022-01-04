import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ModalDefaultContentButton } from '../../models/ModalDefaultContentButton';

@Component({
  selector: '[modal-default-content]',
  templateUrl: './modal-default-content.component.html',
  styleUrls: ['./modal-default-content.component.scss'],
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: {
    class: 'inner-content',
  },
})
export class ModalDefaultContentComponent {
  @Input()
  cancelButton: ModalDefaultContentButton;
  @Input()
  successButton: ModalDefaultContentButton;
  @Output()
  closeModalEvent: EventEmitter<'success' | 'cancel'> = new EventEmitter<
    'success' | 'cancel'
  >();

  closeModal(result: 'success' | 'cancel') {
    this.closeModalEvent.emit(result);
  }
}
