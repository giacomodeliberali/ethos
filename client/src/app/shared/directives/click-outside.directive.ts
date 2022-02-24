import {
  Directive,
  ElementRef,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';

@Directive({
  selector: '[appClickOutside]',
})
export class ClickOutsideDirective implements OnDestroy, OnInit {
  @Output()
  outsideClick = new EventEmitter<void>();

  constructor(private element: ElementRef) {}

  public ngOnInit() {
    this.onDocumentClick = this.onDocumentClick.bind(this);
    document.addEventListener('click', this.onDocumentClick);
  }

  public ngOnDestroy() {
    document.removeEventListener('click', this.onDocumentClick);
  }

  protected onDocumentClick(event: MouseEvent) {
    if (this.element.nativeElement.contains(event.target)) {
      return;
    }
    this.outsideClick.emit();
  }
}
