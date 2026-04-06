import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { BaseLayoutComponent } from '../base-layout/base-layout.component';

@Component({
  selector: 'app-staff-layout',
  imports: [BaseLayoutComponent],
  templateUrl: './staff-layout.component.html',
  styleUrl: './staff-layout.component.scss'
})
export class StaffLayoutComponent {
  navItems: MenuItem[] = [
    { label: 'Courses', routerLink: '/staff/courses' },
    { label: 'Classes', routerLink: '/staff/classes' }
  ];
}
