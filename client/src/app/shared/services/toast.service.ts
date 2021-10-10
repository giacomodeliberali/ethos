import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { IonicSafeString, ToastOptions } from '@ionic/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  /** toastQueue */
  private toastQueue: ToastOptions[] = [];

  constructor(private toastController: ToastController) {}

  async closeTop() {
    const toast = await this.toastController.getTop();
    if (toast) {
      toast.dismiss();
    }
  }



  public addErrorToast(options: { message: string }, stack: boolean = true) {
    this.addToast({
      header: 'Ops...',
      message: options.message,
      color: 'danger'
    }, stack);
  }

  public addInfoToast(options: { header: string; message: string }, stack: boolean = true) {
    this.addToast({
      header: options.header,
      message: options.message,
      color: 'tertiary'
    }, stack);
  }

  public addSuccessToast(options: { header: string; message: string }, stack: boolean = true) {
    this.addToast({
      header: options.header,
      message: options.message,
      color: 'success'
    }, stack);
  }

  /** Show next toast */
  private async displayNext() {
    const toast = await this.toastController.create(this.toastQueue[0]);
    toast.present();
    toast.onDidDismiss().then(() => {
      this.toastQueue.shift();
      if (this.toastQueue.length > 0) {
        this.displayNext();
      }
    });
  }

  /**
   * Aggiunge un nuovo toast alla coda
   *
   * @param options Aspetto e contenuto del toast
   */
  private addToast(
    options: { message: string | IonicSafeString; header: string; color?: 'tertiary' | 'danger' | 'warning' | 'success' },
    stack: boolean) {
    const color = options.color ?? 'tertiary';
    const colorToIcon = {
      tertiary: '',
      danger: '<ion-icon name="close-circle-outline" color="danger" style="font-size: 1.5rem"></ion-icon>',
      warning: '<ion-icon name="alert-circle-outline" color="warning" style="font-size: 1.5rem"></ion-icon>',
      success: '<ion-icon name="checkmark-circle-outline" color="success" style="font-size: 1.5rem"></ion-icon>'
    };
    console.log(options.message);
    document.documentElement.style.setProperty('--ionic-toast-color', `var(--ion-color-${color})`);
    if(stack || this.toastQueue.length <= 0){
      this.toastQueue.push({
        // eslint-disable-next-line max-len
        message: new IonicSafeString(`${colorToIcon[color]}<div class="text-container" style="padding-left:0.75rem"><div class="header" style="font-weight:bold;padding-bottom:0.25rem">${options.header}</div><div class="message">${options.message}</div></div>`),
        duration: 5000,
        mode: 'ios',
        cssClass: 'ionic-toast'
      });
      if (this.toastQueue.length === 1) {
        this.displayNext();
      }
    }
  }
}
