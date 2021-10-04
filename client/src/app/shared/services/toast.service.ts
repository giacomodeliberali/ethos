import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { ToastOptions } from '@ionic/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  /** toastQueue */
  private toastQueue: ToastOptions[] = [];

  constructor(private toastController: ToastController) {}

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

  async closeTop() {
    const toast = await this.toastController.getTop();
    if (toast) {
      toast.dismiss();
    }
  }

  /**
   * Aggiunge un nuovo toast alla coda
   * @param options Aspetto e contenuto del toast
   */
  private addToast(options: { message: string; header: string; color?: 'primary' | 'danger' | 'warning' | 'success' }, stack: boolean) {
    if(stack || this.toastQueue.length <= 0){
      this.toastQueue.push({
        ...options,
        duration: 4000,
        mode: 'ios',
        cssClass: 'ionic-toast'
      });
      if (this.toastQueue.length === 1) {
        this.displayNext();
      }
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
    }, stack);
  }

  public addSuccessToast(options: { header: string; message: string }, stack: boolean = true) {
    this.addToast({
      header: options.header,
      message: options.message,
      color: 'success',
    }, stack);
  }
}
