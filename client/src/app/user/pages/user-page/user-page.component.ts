import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { SchedulesService } from "@core/services/ethos.generated.service";
import { MediaService } from "@core/services/media.service";
import { IonSlides } from "@ionic/angular";

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent implements OnInit {

  private _slider: IonSlides
  @ViewChild('test', {static: true, read: ElementRef}) slider: ElementRef;


  slides = ['Uno', 'Due', 'Tre', 'Quattro', 'Cinque', 'Sei', 'Sette', 'Otto']



  slideOpts = {
    // Default parameters
    slidesPerView: 1,
    spaceBetween: 0,
    // Responsive breakpoints
    breakpoints: {
      768: {
        slidesPerView: 3
      },
      1024: {
        slidesPerView: 5
      }
    }
  }

  constructor(public mediaSvc: MediaService, private schedulesSvc: SchedulesService) {
    let nextWeek = new Date();
    nextWeek.setDate(new Date().getDate() + 7);
    this.schedulesSvc.getAllInRange(new Date().toISOString(), nextWeek.toISOString()).subscribe(result => console.log(result))
  
  }

  ngOnInit(){
    this._slider = (this.slider.nativeElement as IonSlides);
  }

  async slideChange(ev){
    console.log('SLIDE CHANGING', await this._slider.getActiveIndex());
  }

}
