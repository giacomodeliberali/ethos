import { Component } from '@angular/core';
import { BaseDirective } from '@core/directives';
import { GeneratedScheduleDto, SchedulesService } from "@core/services/ethos.generated.service";
import { MediaService } from "@core/services/media.service";
import { LoadingService } from '@shared/services/loading.service';
import { map } from 'rxjs/operators';

const afternoonStart = {hour:12, minutes: 0};
const eveningStart = {hour: 18, minutes: 0};

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent extends BaseDirective{

  currentDate: string = new Date().toISOString();
  schedules: {
    morning: Array<GeneratedScheduleDto>;
    afternoon: Array<GeneratedScheduleDto>;
    evening: Array<GeneratedScheduleDto>;
  };


  constructor(public mediaSvc: MediaService, private schedulesSvc: SchedulesService, private loadingSvc: LoadingService) {
    super();
    // let nextWeek = new Date();
    // nextWeek.setDate(new Date().getDate() + 7);
    // this.schedulesSvc.getAllSchedulesInRange(new Date().toISOString(), nextWeek.toISOString()).subscribe(result => console.log(result));
    this.dateChanged(this.currentDate);
  }
  dateChanged(date: string){
    const startDate = new Date(date);
    const endDate = new Date(date);
    startDate.setHours(0,0,0);
    endDate.setHours(23,59,59);
    this.loadingSvc.startLoading(
      this,
      'GET_SCHEDULES',
      this.schedulesSvc.getAllSchedulesInRange(startDate.toISOString(), endDate.toISOString()),
      {
        message: 'Sto caricando gli allenamenti della giornata'
      }
    ).pipe(
      map(schedules => {
        const schedulesByDayPortion = {
          morning: [],
          afternoon: [],
          evening: []
        };
        for (const schedule of schedules){
          const currentDate = new Date(schedule.startDate);
          const hour = currentDate.getHours();
          const minutes = currentDate.getMinutes();
          if(hour >= eveningStart.hour && minutes >= eveningStart.minutes){
            schedulesByDayPortion.evening.push(schedule);
            continue;
          }
          if(hour >= afternoonStart.hour && minutes >= afternoonStart.minutes){
            schedulesByDayPortion.afternoon.push(schedule);
            continue;
          }
          schedulesByDayPortion.morning.push(schedule);
        }
        return schedulesByDayPortion;
      })
    ).subscribe(schedules => this.schedules = schedules);
  }

}
