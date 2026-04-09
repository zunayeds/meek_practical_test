import { Component, inject, signal } from '@angular/core';

import { StudentService } from '../../../../core/services/student.service'; 
import { ViewLayoutComponent } from '../../../../layouts/view-layout/view-layout.component';
import { Student, StudentClassResponse, StudentCourseResponse } from '../../../../core/models/student.model';
import { TabsModule } from 'primeng/tabs';
import { DatePipe } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ColumnModel } from '../../../../core/models/column.model';
import { ListTableComponent } from '../../../../shared/components/list-table/list-table.component';


@Component({
  selector: 'app-student-view',
  imports: [ViewLayoutComponent, TabsModule, DatePipe, ListTableComponent, CardModule],
  templateUrl: './student-view.component.html'
})
export class StudentViewComponent {
  readonly studentService = inject(StudentService);

  student = signal<Student | undefined>(undefined);

  courses = signal<StudentCourseResponse[]>([]);
  coursesloading = signal(false);
  courseColumns: ColumnModel[] = [
    { field: 'name', header: 'Name', type: 'string' },
    { field: 'assignedBy', header: 'Assigned By', type: 'string' },
    { field: 'assignedAt', header: 'Assigned At', type: 'dateTime' },
  ];
  courseLoaded = signal(false);

  classes = signal<StudentClassResponse[]>([]);
  classesloading = signal(false);
  classColumns: ColumnModel[] = [
    { field: 'name', header: 'Name', type: 'string' },
    { field: 'assignedBy', header: 'Assigned By', type: 'string' },
    { field: 'assignedAt', header: 'Assigned At', type: 'dateTime' },
  ];
  classLoaded = signal(false);

  onTabChange(tab: any) {
    if (tab === 'basic' || !this.student()) {
      return;
    } else if (tab === 'courses' && !this.courseLoaded()) {
      this.courseLoaded.set(true);
      this.coursesloading.set(true);
      this.studentService.getCoursesByStudentId(this.student()!.studentId).subscribe(result => {
        this.coursesloading.set(false);
        this.courses.set(result);
      });
    } else if (tab === 'classes' && !this.classLoaded()) {
      this.classLoaded.set(true);
      this.classesloading.set(true);
      this.studentService.getClassesByStudentId(this.student()!.studentId).subscribe(result => {
        this.classesloading.set(false);
        this.classes.set(result);
      });
    }
  }

  onDataLoaded(result: any) {
    this.student.set(result);
  }
}
