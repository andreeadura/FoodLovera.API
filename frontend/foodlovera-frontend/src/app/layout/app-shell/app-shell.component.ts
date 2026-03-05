import { ChangeDetectionStrategy, Component, HostListener } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

import { HeaderComponent } from '../header/header.component';
import { BurgerMenuComponent } from '../burger-menu/burger-menu.component';

import { Store } from '@ngrx/store';
import { inject } from '@angular/core';
import { selectIsAuthenticated } from '../../modules/auth/store/selectors/auth.selectors';
import { AuthActions } from '../../modules/auth/store/actions/auth.actions';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent, BurgerMenuComponent],
  templateUrl: './app-shell.component.html',
  styleUrls: ['./app-shell.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})


export class AppShellComponent {
    private readonly store = inject(Store);

    readonly isAuthenticated$ = this.store.select(selectIsAuthenticated);

    menuOpen = false;

    constructor() {
        this.store.dispatch(AuthActions.hydrateFromStorage());
    }

    openMenu(): void { this.menuOpen = true; }
    closeMenu(): void { this.menuOpen = false; }

    @HostListener('document:keydown.escape')
        onEsc(): void {
        this.closeMenu();
    }
  
}