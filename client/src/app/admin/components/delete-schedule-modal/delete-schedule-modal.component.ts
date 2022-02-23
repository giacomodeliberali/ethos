import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-delete-schedule-modal',
  templateUrl: './delete-schedule-modal.component.html',
  styleUrls: ['./delete-schedule-modal.component.scss'],
})
export class DeleteScheduleModalComponent implements OnInit {
  @Input()
  isRecurring: boolean;

  deleteAllSchedule = new FormControl(false);
  constructor(private modalCtrl: ModalController) {}

  ngOnInit() {}

  closeModal(e: 'success' | 'cancel') {
    this.modalCtrl.dismiss(
      e === 'success' ? this.deleteAllSchedule.value : null
    );
  }
}
