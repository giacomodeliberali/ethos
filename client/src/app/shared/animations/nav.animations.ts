/* eslint-disable max-len */
import { MediaService } from '@core/services/media.service';
import { Animation, AnimationController } from '@ionic/angular';

type PageName = 'APP-LOGIN-PAGE' | 'APP-RESET-PASSWORD-PAGE';

const DURATION = 1000;
const animationCtrl = new AnimationController();

const loginToResetPassword = (opts: any) => {
  const showPageBehind = animationCtrl
    .create()
    .addElement(opts.enteringEl)
    .duration(1)
    .afterStyles({ opacity: 1 });
  const hideImage = MediaService.isSmartphone
    ? animationCtrl.create().addAnimation([
        animationCtrl
          .create()
          .addElement(opts.enteringEl.querySelector('.logo-container'))
          .duration(DURATION / 2)
          .beforeStyles({ opacity: 0 })
          .fromTo('transform', 'translateX(0)', 'translateX(0)'),
        animationCtrl
          .create()
          .addElement(opts.leavingEl.querySelector('.logo-container'))
          .duration(DURATION / 2)
          .fromTo('opacity', '1', '0')
          .onFinish((_) => {
            animationCtrl
              .create()
              .addElement(opts.enteringEl.querySelector('.logo-container'))
              .duration(DURATION / 2)
              .fromTo('opacity', '0', '1')
              .afterClearStyles(['opacity'])
              .play();
          }),
      ])
    : animationCtrl
        .create()
        .addElement(opts.leavingEl.querySelector('.logo-container'))
        .duration(DURATION)
        .beforeStyles({ opacity: 0 })
        .fromTo('transform', 'translateX(0)', 'translateX(0)')
        .afterClearStyles(['opacity']);
  const hideBackground = animationCtrl
    .create()
    .addElement(opts.enteringEl.querySelector('ion-content'))
    .duration(DURATION)
    .beforeStyles({ '--background': 'transparent' })
    .afterClearStyles(['--background']);
  const animationEnter = animationCtrl
    .create()
    .addElement(opts.enteringEl.querySelector('ion-card'))
    .duration(DURATION)
    .easing('ease-out')
    .beforeStyles({ opacity: 1 });
  const animationLeave = animationCtrl
    .create()
    .addElement(opts.leavingEl.querySelector('ion-card'))
    .duration(DURATION)
    .easing('ease-out');
  const direction = MediaService.isSmartphone ? 'X' : 'Y';
  const unity = MediaService.isSmartphone ? 'vw' : 'vh';

  if (opts.enteringEl.nodeName === 'APP-RESET-PASSWORD-PAGE') {
    return animationCtrl
      .create()
      .addAnimation([
        showPageBehind,
        hideImage,
        hideBackground.fromTo('transform', 'translateX(0)', 'translateX(0)'),
        animationEnter.fromTo(
          'transform',
          'translate' + direction + '(100' + unity + ')',
          'translate' + direction + '(0)'
        ),
        animationLeave.fromTo(
          'transform',
          'translate' + direction + '(0)',
          'translate' + direction + '(-100' + unity + ')'
        ),
      ]);
  } else {
    return animationCtrl
      .create()
      .addAnimation([
        showPageBehind,
        hideImage,
        hideBackground.fromTo('transform', 'translateX(0)', 'translateX(0)'),
        animationEnter.fromTo(
          'transform',
          'translate' + direction + '(-100' + unity + ')',
          'translate' + direction + '(0)'
        ),
        animationLeave.fromTo(
          'transform',
          'translate' + direction + '(0)',
          'translate' + direction + '(100' + unity + ')'
        ),
      ]);
  }
};

export const navAnimation = (baseEl: HTMLElement, opts?: any): Animation => {
  if (
    (opts.enteringEl.nodeName === 'APP-LOGIN-PAGE' &&
      opts.leavingEl.nodeName === 'APP-RESET-PASSWORD-PAGE') ||
    (opts.enteringEl.nodeName === 'APP-RESET-PASSWORD-PAGE' &&
      opts.leavingEl.nodeName === 'APP-LOGIN-PAGE')
  ) {
    return loginToResetPassword(opts);
  }
  return null;
};
