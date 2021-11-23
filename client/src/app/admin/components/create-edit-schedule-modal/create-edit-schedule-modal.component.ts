import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

@Component({
  selector: "app-create-edit-schedule-modal",
  templateUrl: "./create-edit-schedule-modal.component.html",
  styleUrls: ["./create-edit-schedule-modal.component.scss"]
})
export class CreateEditScheduleModalComponent implements OnInit {
  scheduleForm: FormGroup = new FormGroup({
    name: new FormControl(),
    description: new FormControl(),
    isRecurrent: new FormControl()
  });

  constructor() {}

  ngOnInit() {}
}
