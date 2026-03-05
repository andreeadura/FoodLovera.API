import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { Router } from '@angular/router';

import { BdButtonComponent } from '../../modules/shared/bd-button/bd-button.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, TranslateModule, BdButtonComponent],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderComponent {
  @Input() isAuthenticated = false;
  @Output() menuClick = new EventEmitter<void>();

  constructor(private readonly router: Router) {}

  openMenu(): void {
    this.menuClick.emit();
  }

  onLogin(): void {
  this.router.navigateByUrl('/(modal:auth/login)');
}
}