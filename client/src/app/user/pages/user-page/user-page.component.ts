import { Component, ViewChild } from '@angular/core';
import { MediaService } from '@core/services/media.service';
import { IonSlides } from '@ionic/angular';

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent {
  @ViewChild('slideWithNav', { static: false }) slideWithNav: IonSlides;

  slides = ['Uno', 'Due', 'Tre', 'Quattro', 'Cinque', 'Sei', 'Sette', 'Otto'];

  slideOpts = {
    // Default parameters
    slidesPerView: 1,
    spaceBetween: 0,
    // Responsive breakpoints
    breakpoints: {
      768: {
        slidesPerView: 3,
      },
      1024: {
        slidesPerView: 5,
      },
    },
  };

  constructor(public mediaSvc: MediaService) {}
}
