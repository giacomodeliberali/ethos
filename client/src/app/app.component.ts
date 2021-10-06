import { Component, ViewChild } from '@angular/core';
import { LoadingDirective } from "@core/directives/loading/loading.directive";
import { LoadingService } from "@shared/services/loading.service";

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent {
  @ViewChild(LoadingDirective, {static: true}) loadingHost!: LoadingDirective;

  constructor(private loadingSvc: LoadingService) {}

  ngAfterViewInit(){
    console.log(this.loadingHost.viewContainerRef)
    this.loadingSvc.loadingHost = this.loadingHost.viewContainerRef;
  }
}
