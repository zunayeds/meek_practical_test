import { Component, inject, input } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../core/services/auth.service';
import { MenuItem } from 'primeng/api';
import { MenubarModule } from 'primeng/menubar';

@Component({
  selector: 'app-base-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MenubarModule, ButtonModule],
  templateUrl: './base-layout.component.html',
  styleUrl: './base-layout.component.scss'
})
export class BaseLayoutComponent {
  protected readonly authService = inject(AuthService);

  navbarClass = input<string>('base-menubar');
  navItems = input<MenuItem[]>([]);

  logout(): void {
    this.authService.logout();
  }
}
