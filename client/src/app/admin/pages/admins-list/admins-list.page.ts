import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseDirective } from '@core/directives';
import {
  IdentityService,
  UserDto,
} from '@core/services/ethos.generated.service';
import { MediaService } from '@core/services/media.service';
import { ModalController } from '@ionic/angular';
import { SearchbarChangeEventDetail } from '@ionic/core';
import { LoadingService } from '@shared/services/loading.service';
import { ToastService } from '@shared/services/toast.service';
import { Observable, of } from 'rxjs';
import { MakeUserAdminModalComponent } from '../../components/make-user-admin-modal/make-user-admin-modal.component';

@Component({
  selector: 'app-admins-list',
  templateUrl: './admins-list.page.html',
  styleUrls: ['./admins-list.page.scss'],
})
export class AdminsListPage extends BaseDirective implements OnInit {
  users$: Observable<UserDto[]>;

  private lastSearch: string;

  constructor(
    private router: Router,
    private identityService: IdentityService,
    private modalCtrl: ModalController,
    private loadingSvc: LoadingService,
    private toastSvc: ToastService
  ) {
    super();
  }

  ngOnInit() {}

  onSearchChange(event: SearchbarCustomEvent) {
    this.lastSearch = event.detail.value;

    if (this.lastSearch.length === 0) {
      this.users$ = of([]);
      return;
    }

    this.searchUsers(this.lastSearch);
  }

  searchUsers(text: string) {
    this.users$ = this.identityService.searchUsers(text);
  }

  async onUserClick(user: UserDto) {
    if (user.roles.includes('admin')) {
      return;
    }

    const showDeleteModal = await this.modalCtrl.create({
      component: MakeUserAdminModalComponent,
      componentProps: { user },
      cssClass: MediaService.isSmartphone ? 'bottom' : '',
      swipeToClose: true,
      backdropDismiss: false,
      mode: 'ios',
    });
    await showDeleteModal.present();
    const { data } = await showDeleteModal.onWillDismiss();

    if (data === 'cancel') {
      return;
    }

    this.makeAdministrator(user);
  }

  makeAdministrator(user: UserDto) {
    this.loadingSvc
      .startLoading(
        this,
        'UNBOOK_COURSE',
        this.identityService.addAdminRole(user.id),
        {
          message: 'Sto aggiungendo il ruolo di amministratore',
        }
      )
      .subscribe({
        next: (course) => {
          this.toastSvc.addSuccessToast({
            header: 'Ruolo aggiunto!',
            message: `${user.fullName} è ora un amministratore.`,
          });
          this.searchUsers(this.lastSearch);
        },
        error: () => {
          this.toastSvc.addErrorToast({
            message: `Errore durante l'aggiunta del ruolo.`,
          });
        },
      });
  }

  goToSchedulePage() {
    this.router.navigate(['admin']);
  }
}

interface SearchbarCustomEvent extends CustomEvent {
  detail: SearchbarChangeEventDetail;
  target: HTMLIonSearchbarElement;
}
