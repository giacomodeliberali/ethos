import { Component, Input } from '@angular/core';
import { GeneratedScheduleDto } from '@core/services/ethos.generated.service';
import moment from 'moment';

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

  constructor() {}
}
