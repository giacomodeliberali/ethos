import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MediaService {
  static get isSmartphone(): boolean{
    return (window.innerWidth < 768);
  }
  constructor() { }
}
