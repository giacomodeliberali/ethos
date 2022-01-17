import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GeneratedScheduleDto } from '@core/services/ethos.generated.service';

@Component({
  selector: 'app-schedule-card',
  templateUrl: './schedule-card.component.html',
  styleUrls: ['./schedule-card.component.scss'],
})
export class ScheduleCardComponent {
  @Input()
  schedule: GeneratedScheduleDto;
  @Input()
  mode: 'user' | 'admin' = 'user';
  @Output()
  deleteClick = new EventEmitter<GeneratedScheduleDto>();
  @Output()
  editClick = new EventEmitter<GeneratedScheduleDto>();

  showActionButtons = false;

  get time() {
    if (this.schedule) {
      const date = new Date(this.schedule.startDate);
      return (
        (date.getHours() > 9 ? '' : '0') +
        date.getHours() +
        ':' +
        (date.getMinutes() > 9 ? '' : '0') +
        date.getMinutes()
      );
    }
    return null;
  }

  get freeSpots() {
    return this.schedule.participantsMaxNumber - this.schedule.bookings.length;
  }

  get isValidDate() {
    return new Date(this.schedule.startDate) > new Date();
  }

  constructor() {}

  deleteClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.deleteClick.emit(this.schedule);
  }

  editClicked(ev: Event) {
    ev.stopImmediatePropagation();
    this.editClick.emit(this.schedule);
  }
}
