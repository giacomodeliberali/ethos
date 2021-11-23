import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  readonly afternoonStart = {hour:12, minutes: 0};
  readonly eveningStart = {hour: 18, minutes: 0};
  constructor() {}
}