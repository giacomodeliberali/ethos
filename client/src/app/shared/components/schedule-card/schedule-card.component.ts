import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  GeneratedScheduleDto,
  GeneratedScheduleDto_BookingDto,
  UserDto,
} from '@core/services/ethos.generated.service';
import moment from 'moment';

@Component({
  selector: 'app-schedule-card',
  templateUrl: './schedule-card.component.html',
  styleUrls: ['./schedule-card.component.scss'],
})
export class ScheduleCardComponent implements OnInit {
  @Input()
  schedule: GeneratedScheduleDto;
  @Input()
  currentUser: UserDto;
  @Input()
  mode: 'user' | 'admin' = 'user';
  @Output()
  deleteClick = new EventEmitter<GeneratedScheduleDto>();
  @Output()
  editClick = new EventEmitter<GeneratedScheduleDto>();
  @Output()
  bookClick = new EventEmitter<GeneratedScheduleDto>();
  @Output()
  unbookClick = new EventEmitter<string>();
  @Output()
  showBookingsClick = new EventEmitter<GeneratedScheduleDto_BookingDto[]>();

  showActionButtons = false;

  ngOnInit(): void {}

  get time() {
    if (this.schedule) {
      return moment(this.schedule.startDate).local().format('H:mm');
    }
    return null;
  }

  get freeSpots() {
    return this.schedule.participantsMaxNumber - this.schedule.bookings.length;
  }

  get isValidDate() {
    return new Date(this.schedule.startDate) > new Date();
  }

  get isBooked() {
    return (
      this.mode === 'user' &&
      this.schedule.bookings
        .map((x) => x.user?.id)
        .includes(this.currentUser?.id)
    );
  }

  deleteClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.deleteClick.emit(this.schedule);
  }

  editClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.editClick.emit(this.schedule);
  }

  bookClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.bookClick.emit(this.schedule);
  }

  unbookClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.unbookClick.emit(
      this.schedule.bookings.find((x) => x.user?.id === this.currentUser.id).id
    );
  }

  showBookingsClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.showBookingsClick.emit(this.schedule.bookings);
  }
}
