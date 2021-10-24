/* eslint-disable prettier/prettier */
import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
} from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: '[loading-icon]',
  templateUrl: './loading.component.html',
  styles: [
    `
      svg {
        display: block;
        height: 100%;
        width: auto;
      }
    `,
  ],
})
export class LoadingComponent implements OnInit, OnChanges {
  @Input()
  type: 'primary' | 'secondary' | 'tertiary' | 'light' = 'primary';
  color$: BehaviorSubject<string> = new BehaviorSubject<string>('#000');

  ngOnInit() {
    this.color$.next(
      getComputedStyle(document.documentElement)
        .getPropertyValue(`--ion-color-${this.type}`)
        .trim()
    );
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.type) {
      this.color$.next(
        getComputedStyle(document.documentElement)
          .getPropertyValue(`--ion-color-${this.type}`)
          .trim()
      );
    }
  }
}
