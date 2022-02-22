import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';
import { DeleteScheduleModalComponent } from './delete-schedule-modal.component';

describe('DeleteScheduleModalComponent', () => {
  let component: DeleteScheduleModalComponent;
  let fixture: ComponentFixture<DeleteScheduleModalComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [DeleteScheduleModalComponent],
        imports: [IonicModule.forRoot()],
      }).compileComponents();

      fixture = TestBed.createComponent(DeleteScheduleModalComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    })
  );

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
