import { Component, Input, OnInit } from '@angular/core';
import { UserDto } from '@core/services/ethos.generated.service';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss'],
})
export class UserCardComponent implements OnInit {
  @Input()
  user: UserDto;

  constructor() {}

  ngOnInit() {
    console.log(this.user);
  }
}
