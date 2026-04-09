import { Component, inject, OnInit, signal } from '@angular/core';

import { StudentService } from '../../../core/services/student.service'; 

import { StudentBase, StudentClassResponse, StudentCourseResponse } from '../../../core/models/student.model';
import { TabsModule } from 'primeng/tabs';
import { CardModule } from 'primeng/card';
import { ColumnModel } from '../../../core/models/column.model';
import { ListTableComponent } from '../../../shared/components/list-table/list-table.component';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ListExtraActionsDirective } from '../../../shared/directives/list-extra-actions.directive';

@Component({
  selector: 'app-student-info',
  imports: [ListExtraActionsDirective, TabsModule, ListTableComponent, CardModule, ButtonModule],
  templateUrl: './student-info.component.html'
})
export class StudentInfoComponent implements OnInit {
  readonly studentService = inject(StudentService);
  private readonly router = inject(Router);

  student = signal<StudentBase | undefined>(undefined);

  courses = signal<StudentCourseResponse[]>([]);
  coursesloading = signal(false);
  courseColumns: ColumnModel[] = [
    { field: 'name', header: 'Name', type: 'string' },
    { field: 'assignedAt', header: 'Assigned At', type: 'dateTime' }
  ];
  courseLoaded = signal(false);

  classes = signal<StudentClassResponse[]>([]);
  classesloading = signal(false);
  classColumns: ColumnModel[] = [
    { field: 'name', header: 'Name', type: 'string' },
    { field: 'assignedAt', header: 'Assigned At', type: 'dateTime' },
    { field: 'actions', header: 'Actions', type: 'action' }
  ];
  classLoaded = signal(false);

  ngOnInit() {
    this.studentService.getOwnInfo().subscribe(result => {
      this.student.set(result);
    });
  }

  onTabChange(tab: any) {
    if (tab === 'info' || !this.student()) {
      return;
    } else if (tab === 'courses' && !this.courseLoaded()) {
      this.courseLoaded.set(true);
      this.coursesloading.set(true);
      this.studentService.getCourses().subscribe(result => {
        this.coursesloading.set(false);
        this.courses.set(result);
      });
    } else if (tab === 'classes' && !this.classLoaded()) {
      this.classLoaded.set(true);
      this.classesloading.set(true);
      this.studentService.getClasses().subscribe(result => {
        this.classesloading.set(false);
        this.classes.set(result);
      });
    }
  }
  
  onViewStudents(cls: StudentClassResponse) {
    this.router.navigate([`/student/class/${cls.classId}/list`]);
  }
}
