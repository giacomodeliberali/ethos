import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BookingDto } from '@core/services/ethos.generated.service';
import moment from 'moment';

@Component({
  selector: 'app-booking-card',
  templateUrl: './booking-card.component.html',
  styleUrls: ['./booking-card.component.scss'],
})
export class BookingCardComponent implements OnInit {
  @Input()
  booking: BookingDto;

  @Output()
  unbookClick = new EventEmitter<string>();

  showActionButtons = false;

  ngOnInit(): void {}

  get time() {
    if (this.booking) {
      return moment(this.booking.startDate).calendar();
    }
    return null;
  }

  unbookClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.unbookClick.emit(this.booking.id);
  }
}
