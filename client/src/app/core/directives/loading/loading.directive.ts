import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[loadingHost]',
})
export class LoadingDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
