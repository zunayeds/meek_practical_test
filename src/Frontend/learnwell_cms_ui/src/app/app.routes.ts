import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { CourseListComponent } from './features/staff/courses/course-list/course-list.component';
import { StaffLayoutComponent } from './layouts/staff-layout/staff-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { staffGuard } from './core/guards/staff.guard';
import { ClassListComponent } from './features/staff/classes/class-list/class-list.component';
import { StudentListComponent } from './features/staff/students/student-list/student-list.component';
import { CourseFormComponent } from './features/staff/courses/course-form/course-form.component';
import { ClassFormComponent } from './features/staff/classes/class-form/class-form.component';
import { StudentFormComponent } from './features/staff/students/student-form/student-form.component';

export const routes: Routes = [
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    {
        path: 'staff',
        component: StaffLayoutComponent,
        canActivate: [authGuard, staffGuard],
        children: [
            { path: '', redirectTo: 'courses', pathMatch: 'full' },
            {
                path: 'courses',
                children: [
                    { path: '', component: CourseListComponent },
                    { path: 'create', component: CourseFormComponent },
                    { path: ':id/edit', component: CourseFormComponent }
                ]
            },
            {
                path: 'classes',
                children: [
                    { path: '', component: ClassListComponent },
                    { path: 'create', component: ClassFormComponent },
                    { path: ':id/edit', component: ClassFormComponent }
                ]
            },
            {
                path: 'students',
                children: [
                    { path: '', component: StudentListComponent },
                    { path: 'create', component: StudentFormComponent },
                    { path: ':id/edit', component: StudentFormComponent }
                ]
            }
        ]
    }
];
