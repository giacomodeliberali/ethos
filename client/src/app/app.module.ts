import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { LOCALE_ID, NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouteReuseStrategy } from '@angular/router';
import { CoreModule } from '@core/core.module';
import { TokenInterceptor } from '@core/interceptors/token.interceptor';
import { API_BASE_URL } from '@core/services/ethos.generated.service';
import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { SharedModule } from '@shared/shared.module';
import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {
  ServiceWorkerModule,
  SwUpdate,
  VersionReadyEvent,
} from '@angular/service-worker';
import { ToastService } from '@shared/services/toast.service';
import localeIt from '@angular/common/locales/it';
import { registerLocaleData } from '@angular/common';
import { filter, map } from 'rxjs/operators';
registerLocaleData(localeIt);

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    IonicModule.forRoot(),
    AppRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    HttpClientModule,
    CoreModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production,
      // Register the ServiceWorker as soon as the app is stable
      // or after 30 seconds (whichever comes first).
      registrationStrategy: 'registerWhenStable:30000',
    }),
  ],
  providers: [
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
    },
    { provide: API_BASE_URL, useValue: environment.baseUrl },
    { provide: LOCALE_ID, useValue: 'it-IT' },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(swUpdate: SwUpdate, toastService: ToastService) {
    swUpdate.versionUpdates
      .pipe(
        filter((evt): evt is VersionReadyEvent => evt.type === 'VERSION_READY'),
        map((evt) => ({
          type: 'UPDATE_AVAILABLE',
          current: evt.currentVersion,
          available: evt.latestVersion,
        }))
      )
      .subscribe(async (update) => {
        toastService.addInfoToast({
          header: 'Aggiornamento',
          message: 'Nuova versione disponibile, l\'app verrà ricaricata fra 3 secondi',
        });
        await swUpdate.activateUpdate();
        setTimeout(() => {
          document.location.reload();
        }, 3000);
      });
  }
}
