import { Component, Input } from '@angular/core';
import { GeneratedScheduleDto } from '@core/services/ethos.generated.service';

@Component({
  selector: 'app-schedule-card',
  templateUrl: './schedule-card.component.html',
  styleUrls: ['./schedule-card.component.scss'],
})
export class ScheduleCardComponent{

  @Input()
  schedule: GeneratedScheduleDto;

  get time(){
    if(this.schedule){
      const date = new Date(this.schedule.startDate);
      return ((date.getHours() > 9) ? '' : '0') + date.getHours() + ':' + ((date.getMinutes() > 9) ? '' : '0') + date.getMinutes();
    }
    return null;
  }

  get freeSpots(){
    return this.schedule.participantsMaxNumber - this.schedule.bookings.length;
  }

  get isValidDate(){
    return new Date(this.schedule.startDate) > new Date();
  }

  constructor() {
  }

}
