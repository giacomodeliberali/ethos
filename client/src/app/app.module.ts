import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouteReuseStrategy } from '@angular/router';
import { CoreModule } from "@core/core.module";
import { TokenInterceptor } from "@core/interceptors/token.interceptor";
import { API_BASE_URL } from "@core/services/ethos.generated.service";
import { IonicModule, IonicRouteStrategy } from '@ionic/angular';
import { navAnimation } from "@shared/animations/nav.animations";
import { SharedModule } from "@shared/shared.module";
import { environment } from "src/environments/environment";
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';



@NgModule({
  declarations: [AppComponent],
  entryComponents: [],
  imports: [
    BrowserModule, 
    BrowserAnimationsModule, 
    IonicModule.forRoot({
      navAnimation
    }), 
    AppRoutingModule, 
    ReactiveFormsModule, 
    SharedModule, 
    HttpClientModule, 
    CoreModule
  ],
  providers: [{ provide: RouteReuseStrategy, useClass: IonicRouteStrategy }, {
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true,
  },
  {provide: API_BASE_URL, useValue: environment.baseUrl}],
  bootstrap: [AppComponent],
})
export class AppModule {}
