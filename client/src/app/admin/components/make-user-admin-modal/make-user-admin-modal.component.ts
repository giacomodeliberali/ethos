import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { UserDto } from '@core/services/ethos.generated.service';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-make-user-admin-modal',
  templateUrl: './make-user-admin-modal.component.html',
  styleUrls: ['./make-user-admin-modal.component.scss'],
})
export class MakeUserAdminModalComponent implements OnInit {
  @Input()
  user: UserDto;

  constructor(private modalCtrl: ModalController) {}

  ngOnInit() {}

  closeModal(e: 'success' | 'cancel') {
    this.modalCtrl.dismiss(e);
  }
}
