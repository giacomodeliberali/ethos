import { Component, Input, OnInit } from '@angular/core';
import { GeneratedScheduleDto_BookingDto } from '@core/services/ethos.generated.service';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'app-show-booking-modal',
  templateUrl: './show-booking-modal.component.html',
  styleUrls: ['./show-booking-modal.component.scss'],
})
export class ShowBookingModalComponent implements OnInit {
  @Input()
  bookings: GeneratedScheduleDto_BookingDto[];

  constructor(private modalCtrl: ModalController) {}

  ngOnInit() {}

  closeModal(ev: string) {
    this.modalCtrl.dismiss();
  }
}
