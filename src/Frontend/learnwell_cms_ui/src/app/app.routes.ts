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
import { CourseViewComponent } from './features/staff/courses/course-view/course-view.component';
import { ClassViewComponent } from './features/staff/classes/class-view/class-view.component';
import { StudentViewComponent } from './features/staff/students/student-view/student-view.component';
import { AssignClassComponent } from './features/staff/courses/assign-classes/assign-class.component';
import { AssignStudentsComponent } from './features/staff/courses/assign-students/assign-students.component';

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
                    { path: ':id/edit', component: CourseFormComponent },
                    { path: ':id/view', component: CourseViewComponent },
                    { path: ':id/assign-classes', component: AssignClassComponent },
                    { path: ':id/assign-students', component: AssignStudentsComponent },
                ]
            },
            {
                path: 'classes',
                children: [
                    { path: '', component: ClassListComponent },
                    { path: 'create', component: ClassFormComponent },
                    { path: ':id/edit', component: ClassFormComponent },
                    { path: ':id/view', component: ClassViewComponent }
                ]
            },
            {
                path: 'students',
                children: [
                    { path: '', component: StudentListComponent },
                    { path: 'create', component: StudentFormComponent },
                    { path: ':id/edit', component: StudentFormComponent },
                    { path: ':id/view', component: StudentViewComponent }
                ]
            }
        ]
    }
];
