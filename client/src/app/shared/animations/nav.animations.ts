import { Animation, AnimationController } from "@ionic/angular";

type PageName = 'APP-RESTAURANT'| 'APP-SETTINGS'| 'APP-LOGIN'| 'APP-ORDERS'| 'APP-RECURRENT-DISHES' | 'APP-EMPLOYEE';

export const navAnimation = (baseEl: HTMLElement, opts?: any): Animation => {
    const DURATION = 500;
    const animationCtrl = new AnimationController();
    let animationEnter = animationCtrl.create()
        .addElement(opts.enteringEl)
        .duration(DURATION)
        .easing('ease-out')
    let animationLeave = animationCtrl.create()
        .addElement(opts.leavingEl)
        .duration(DURATION)
        .easing('ease-out')
    if(checkIfForwardOrBackward(opts.enteringEl.nodeName, opts.leavingEl.nodeName) === 'FORWARD'){
        return animationCtrl.create()
            .addAnimation([
                animationEnter.fromTo('transform', 'translateX(100%)', 'translateX(0)').beforeStyles({ opacity: 1 }),
                animationLeave.fromTo('transform', 'translateX(0)', 'translateX(-100%)'),
            ])
    } else {
        return animationCtrl.create()
        .addAnimation([
            animationEnter.fromTo('transform', 'translateX(-100%)', 'translateX(0)').beforeStyles({ opacity: 1 }),
            animationLeave.fromTo('transform', 'translateX(0)', 'translateX(100%)'),
        ])
    }
}

function checkIfForwardOrBackward(enteringPageName: PageName, leavingPageName: PageName): 'FORWARD' | 'BACKWARD'{
    if(
        (leavingPageName === 'APP-LOGIN') || 
        ((leavingPageName === 'APP-RESTAURANT') && (enteringPageName !== 'APP-LOGIN'))
    )
        return 'FORWARD'
    else
        return 'BACKWARD'
}