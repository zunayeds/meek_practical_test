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
import { CourseAssignStudentsComponent } from './features/staff/courses/course-assign-students/course-assign-students.component';
import { ClassAssignStudentsComponent } from './features/staff/classes/class-assign-students/class-assign-students.component';
import { StudentLayoutComponent } from './layouts/student-layout/student-layout.component';
import { studentGuard } from './core/guards/student.guard';
import { StudentInfoComponent } from './features/student/student-info/student-info.component';
import { OtherStudentListComponent } from './features/student/other-student-list/other-student-list.component';

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
                    { path: ':id/assign-students', component: CourseAssignStudentsComponent },
                ]
            },
            {
                path: 'classes',
                children: [
                    { path: '', component: ClassListComponent },
                    { path: 'create', component: ClassFormComponent },
                    { path: ':id/edit', component: ClassFormComponent },
                    { path: ':id/view', component: ClassViewComponent },
                    { path: ':id/assign-students', component: ClassAssignStudentsComponent },
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
    },
    {
        path: 'student',
        component: StudentLayoutComponent,
        canActivate: [authGuard, studentGuard],
        children: [
            { path: '', redirectTo: 'info', pathMatch: 'full' },
            { path: 'info', component: StudentInfoComponent },
            { path: 'class/:classId/list', component: OtherStudentListComponent }
        ]
    }
];
