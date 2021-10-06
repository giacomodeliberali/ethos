import { animate, style, transition, trigger } from '@angular/animations';

export const enterFromRightAnimation = trigger('enterFromRightAnimation', [
    transition(':enter', [
        style({transform: 'translateX(100%)'}),
        animate('0.1s ease-out',
            style({
                transform: 'translateX(0)'
            }))
    ]),
    transition(':leave', [
        animate('0.1s ease-in',
        style({
            transform: 'translateX(100%)'
        }))
    ]),
])

export const enterFromTopAnimation = trigger('enterFromTopAnimation', [
    transition(':enter', [
        style({transform: 'translateY(-100%)'}),
        animate('0.2s ease-out',
            style({
                transform: 'translateX(0)'
            }))
    ]),
    transition(':leave', [
        animate('0.2s ease-in',
        style({
            transform: 'translateY(-100%)'
        }))
    ]),
])