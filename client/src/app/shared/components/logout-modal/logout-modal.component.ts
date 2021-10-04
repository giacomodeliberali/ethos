import { Component } from '@angular/core';
import { ModalController } from "@ionic/angular";

@Component({
  selector: 'app-logout-modal',
  templateUrl: './logout-modal.component.html',
  styleUrls: ['./logout-modal.component.scss']
})
export class LogoutModalComponent {

  constructor(private modalCtrl: ModalController) { }

  closeModal(ev){
    this.modalCtrl.dismiss({
      logout: ev === 'success'
    })
  }
}
