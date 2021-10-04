import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ModalDefaultContentButton } from '../../models/ModalDefaultContentButton';

@Component({
  selector: '[modal-default-content]',
  templateUrl: './modal-default-content.component.html',
  styleUrls: ['./modal-default-content.component.scss'],
  host: {
    class: 'inner-content'
  }
})
export class ModalDefaultContentComponent {

  @Input()
  cancelButton: ModalDefaultContentButton;
  @Input()
  successButton: ModalDefaultContentButton;
  @Output()
  buttonClicked = new EventEmitter<'success' | 'cancel'>();

  constructor() { }

  emitClick(result: 'success' | 'cancel'){
    this.buttonClicked.emit(result);
  }

}
