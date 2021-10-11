import { Component } from '@angular/core';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-unsaved-changes',
  templateUrl: './unsaved-changes.component.html',
  styleUrls: ['./unsaved-changes.component.scss'],
})
export class UnsavedChangesComponent {

  constructor(private modalCtrl: ModalController) { }

  dismiss(result: 'success' | 'cancel'){
    this.modalCtrl.dismiss({
      close: (result === 'success')
    });
  }

}
