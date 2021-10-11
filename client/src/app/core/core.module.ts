import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { LoadingDirective } from './directives/loading/loading.directive';


@NgModule({
  declarations: [
    LoadingDirective
  ],
  imports: [
    CommonModule,
    IonicModule,
    ReactiveFormsModule
  ],
  exports: [
    LoadingDirective
  ]
})
export class CoreModule { }
