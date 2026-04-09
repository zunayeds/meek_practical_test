import { Component, inject, OnInit, signal } from '@angular/core';

import { CourseService } from '../../../../core/services/course.service';
import { AssignmentLayoutComponent } from '../../../../layouts/assignment-layout/assignment-layout.component';
import { PickListItem } from '../../../../core/models/pick-list.mode';
import { map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { StudentService } from '../../../../core/services/student.service';

@Component({
  selector: 'app-course-assign-students',
  imports: [AssignmentLayoutComponent],
  templateUrl: './course-assign-students.component.html'
})
export class CourseAssignStudentsComponent implements OnInit {
  private readonly courseService = inject(CourseService);
  private readonly studentService = inject(StudentService);
  private readonly route = inject(ActivatedRoute);

  entityName = signal('');

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id') ?? '';
    this.courseService.getById(id).subscribe(course => this.entityName.set(course.name));
  }

  loadAssignedStudents = (courseId: string) =>
    this.courseService.getStudents(courseId).pipe(
      map(items => items.map(c => ({ id: c.studentId, label: c.firstName + ' ' + c.lastName } as PickListItem)))
    );

  loadStudentSourcePage = (page: number) =>
    this.studentService.getAll('', '', '', page, 10).pipe(
      map(res => res.records.map(c => ({ id: c.studentId, label: c.firstName + ' ' + c.lastName } as PickListItem)))
    );

  saveStudentAssignments = (courseId: string, addIds: string[], removeIds: string[]) =>
    this.courseService.addRemoveStudents(courseId, { addStudentIds: addIds, removeStudentIds: removeIds });
}
