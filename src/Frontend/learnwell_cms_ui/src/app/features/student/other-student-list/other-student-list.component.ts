import { Component, contentChild, inject, OnInit, signal } from '@angular/core';

import { StudentService } from '../../../core/services/student.service'; 

import { StudentNameResponse } from '../../../core/models/student.model';
import { TabsModule } from 'primeng/tabs';
import { CardModule } from 'primeng/card';
import { ColumnModel } from '../../../core/models/column.model';
import { ListTableComponent } from '../../../shared/components/list-table/list-table.component';
import { ActivatedRoute } from '@angular/router';
import { ListExtraActionsDirective } from '../../../shared/directives/list-extra-actions.directive';

@Component({
  selector: 'app-other-student-list',
  imports: [TabsModule, ListTableComponent, CardModule],
  templateUrl: './other-student-list.component.html'
})
export class OtherStudentListComponent implements OnInit {
  readonly studentService = inject(StudentService);
  readonly route = inject(ActivatedRoute);

  extraActionsTemplate = contentChild(ListExtraActionsDirective<StudentNameResponse>);

  students = signal<StudentNameResponse[]>([]);
  studentsloading = signal(false);
  studentColumns: ColumnModel[] = [
    { field: 'fullName', header: 'Full Name', type: 'string' }
  ];
  studentLoaded = signal(false);

  ngOnInit() {
    const classId = this.route.snapshot.paramMap.get('classId') ?? '';
    
    this.studentsloading.set(true);
    this.studentService.getOtherStudentsInClass(classId).subscribe(students => {
      this.studentsloading.set(false);
      this.students.set(students.map(s => ({ fullName: s } as StudentNameResponse)));
    });
  }
}
