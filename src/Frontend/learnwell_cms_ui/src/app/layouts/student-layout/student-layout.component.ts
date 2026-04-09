import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { BaseLayoutComponent } from '../base-layout/base-layout.component';

@Component({
  selector: 'app-student-layout',
  imports: [BaseLayoutComponent],
  templateUrl: './student-layout.component.html',
  styleUrl: './student-layout.component.scss'
})
export class StudentLayoutComponent {
  navItems: MenuItem[] = [
    { label: 'Personal Info', routerLink: '/student/info' }
  ];
}
