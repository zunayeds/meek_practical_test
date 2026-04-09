import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environment';
import { Course, CourseBase } from '../models/course.model';
import { PaginatedResult } from '../models/paginated-result.mode';
import { BaseService } from './base.service';
import { ClassBase } from '../models/class.model';
import { StudentBase } from '../models/student.model';

@Injectable({ providedIn: 'root' })
export class CourseService extends BaseService<Course> {
  override readonly baseUrl = `${environment.apiUrl}/course`;

  getAll(name: string, page: number, pageSize: number) {
    const params = new HttpParams()
      .set('name', name)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResult<CourseBase>>(this.baseUrl, { params });
  }

  getClasses(courseId: string) {
    return this.http.get<ClassBase[]>(`${this.baseUrl}/getClasses/${courseId}`);
  }

  getStudents(courseId: string) {
    return this.http.get<StudentBase[]>(`${this.baseUrl}/getStudents/${courseId}`);
  }
}
