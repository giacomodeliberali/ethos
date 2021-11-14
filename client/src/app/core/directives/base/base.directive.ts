import { Directive, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Directive({
  selector: '[appBase]',
})
export class BaseDirective implements OnDestroy {
  destroy$: Subject<void> = new Subject();

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.unsubscribe();
  }
}
