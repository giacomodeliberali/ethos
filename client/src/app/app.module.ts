import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouteReuseStrategy } from '@angular/router';
import { CoreModule } from '@core/core.module';
import { TokenInterceptor } from '@core/interceptors/token.interceptor';
import { API_BASE_URL } from '@core/services/ethos.generated.service';
import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { navAnimation } from '@shared/animations/nav.animations';
import { SharedModule } from '@shared/shared.module';
import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServiceWorkerModule, SwUpdate } from '@angular/service-worker';
import { ToastService } from '@shared/services/toast.service';

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
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
  constructor(updates: SwUpdate, toastService: ToastService) {
    updates.available.subscribe(async () => {
      toastService.addInfoToast({
        header: 'Aggiornamento',
        message: 'Nuova versione disponibile, la pagina verrà ricaricata',
      });
      await updates.activateUpdate();
      document.location.reload();
    });
  }
}
