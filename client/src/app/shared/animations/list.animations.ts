import {
  animate,
  animateChild,
  query,
  sequence,
  stagger,
  style,
  transition,
  trigger,
} from '@angular/animations';

export const itemsAnimation = trigger('itemsAnimation', [
  transition(':enter', [
    style({
      transform: 'scale(0.5)',
      opacity: 0,
      width: '0px',
      marginRight: '0px',
      padding: '0px',
    }),
    sequence([
      animate(
        '0.1s ease-in',
        style({
          width: '*',
          marginRight: '*',
          padding: '*',
        })
      ),
      animate(
        '0.3s cubic-bezier(.8, -0.6, 0.2, 1.5)',
        style({ transform: 'scale(1)', opacity: 1 })
      ),
    ]),
  ]),
  transition(':leave', [
    style({ transform: 'scale(1)', opacity: 1 }),
    sequence([
      animate(
        '0.3s cubic-bezier(.8, -0.6, 0.2, 1.5)',
        style({
          transform: 'scale(0.5)',
          opacity: 0,
        })
      ),
      animate(
        '0.1s ease-in',
        style({
          width: '0px',
          marginRight: '0px',
          padding: '0px',
        })
      ),
    ]),
  ]),
]);

export const listAnimation = trigger('listAnimation', [
  transition('* => *', [
    query('.placeholder', style({ display: 'none' }), { optional: true }),
    query(':enter', stagger(100, animateChild()), { optional: true }),
    query(':leave', stagger(-100, animateChild()), { optional: true }),
  ]),
]);

export const selectionAnimation = trigger('selectionAnimation', [
  transition(':enter', [
    style({ transform: 'translateX(100%)', opacity: 0 }),
    animate(
      '0.1s ease-out',
      style({
        transform: 'translateX(0)',
        opacity: 1,
      })
    ),
  ]),
  transition(':leave', [
    animate(
      '0.1s ease-out',
      style({
        transform: 'translateX(100%)',
        opacity: 0,
      })
    ),
  ]),
]);
