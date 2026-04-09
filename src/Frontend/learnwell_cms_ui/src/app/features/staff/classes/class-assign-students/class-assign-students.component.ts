import { Component, inject, OnInit, signal } from '@angular/core';

import { ClassService } from '../../../../core/services/class.service';
import { AssignmentLayoutComponent } from '../../../../layouts/assignment-layout/assignment-layout.component';
import { PickListItem } from '../../../../core/models/pick-list.mode';
import { map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { StudentService } from '../../../../core/services/student.service';

@Component({
  selector: 'app-class-assign-students',
  imports: [AssignmentLayoutComponent],
  templateUrl: './class-assign-students.component.html'
})
export class ClassAssignStudentsComponent implements OnInit {
  private readonly classService = inject(ClassService);
  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);

  entityName = signal('');

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.classService.getById(id).subscribe(classItem => this.entityName.set(classItem.name));
  }

  loadAssignedStudents = (classId: string) =>
    this.classService.getStudents(classId).pipe(
      map(items => items.map(c => ({ id: c.studentId, label: c.firstName + ' ' + c.lastName } as PickListItem)))
    );

  loadStudentSourcePage = (page: number) =>
    this.studentService.getAll('', '', '', page, 10).pipe(
      map(res => res.records.map(c => ({ id: c.studentId, label: c.firstName + ' ' + c.lastName } as PickListItem)))
    );

  saveStudentAssignments = (classId: string, addIds: string[], removeIds: string[]) =>
    this.classService.addRemoveStudents(classId, { addStudentIds: addIds, removeStudentIds: removeIds });
}
