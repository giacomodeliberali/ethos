import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: '[loading-icon]',
  template: `
    <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="-1 -1 40 40" [attr.stroke]="color$ | async">
        <g fill="none" fill-rule="evenodd">
            <g transform="translate(1 1)" stroke-width="3">
                <circle stroke-opacity=".5" cx="18" cy="18" r="18"/>
                <path d="M36 18c0-9.94-8.06-18-18-18">
                    <animateTransform attributeName="transform" type="rotate" from="0 18 18" to="360 18 18" dur="1s" repeatCount="indefinite"/>
                </path>
            </g>
        </g>
    </svg>
  `,
  styles: [`
    svg{
      display: block;
      height: 100%;
      width: auto;
    }
  `]
})
export class LoadingComponent implements OnInit {
    @Input()
    type: 'primary' | 'secondary' | 'tertiary' | 'light' = 'primary';
    color$: BehaviorSubject<string> = new BehaviorSubject<string>('#000');
  
    ngOnInit(){
      this.color$.next(getComputedStyle(document.documentElement).getPropertyValue(`--ion-color-${this.type}`).trim())
    }
  
    ngOnChanges(changes: SimpleChanges){
      if(changes.type){
        this.color$.next(getComputedStyle(document.documentElement).getPropertyValue(`--ion-color-${this.type}`).trim())
      }
    }

}