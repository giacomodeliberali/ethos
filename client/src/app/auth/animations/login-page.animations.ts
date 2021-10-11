import { animate, group, query, sequence, style, transition, trigger } from '@angular/animations';

export const loginToRegister = trigger('loginToRegister', [
    transition('login => register', [
        group([
            query('ion-card-header', sequence([
                style({
                    opacity: 1,
                    'background-image': 'url(../../../../assets/images/login-img.jpg)'
                }),
                animate('0.5s ease-in', style({ opacity: 0.2 })),
                style({ 'background-image': 'url(../../../../assets/images/register-img.jpg)' }),
                animate('0.5s ease-out', style({ opacity: 1 })),
            ])),
            sequence([
                query(':enter', style({
                    height: '0px',
                    opacity: 0
                })),
                query(':leave', animate('0.5s ease-in', style({ opacity: 0 }))),
                query(':leave', style({ height: '0px'})),
                query(':enter', style({ height: 'auto'})),
                query(':enter', animate('0.5s ease-out', style({ opacity: 1})))
            ])
        ])
    ]),
    transition('register => login', [
        group([
            query('ion-card-header', sequence([
                style({
                    opacity: 1,
                    'background-image': 'url(../../../../assets/images/register-img.jpg)'
                }),
                animate('0.5s ease-in', style({ opacity: 0.2 })),
                style({ 'background-image': 'url(../../../../assets/images/login-img.jpg)' }),
                animate('0.5s ease-out', style({ opacity: 1 })),
            ])),
            sequence([
                query(':enter', style({
                    height: '0px',
                    opacity: 0
                })),
                query(':leave', animate('0.5s ease-in', style({ opacity: 0 }))),
                query(':leave', style({ height: '0px'})),
                query(':enter', style({ height: 'auto'})),
                query(':enter', animate('0.5s ease-out', style({ opacity: 1})))
            ])
        ]),
    ]),
]);
