import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environment';
import { Class, ClassBase } from '../models/class.model';
import { PaginatedResult } from '../models/paginated-result.mode';
import { BaseService } from './base.service';
import { AddRemoveStudentsRequest, CourseBase } from '../models/course.model';
import { StudentBase } from '../models/student.model';

@Injectable({ providedIn: 'root' })
export class ClassService extends BaseService<Class> {
  override readonly baseUrl = `${environment.apiUrl}/class`;

  getAll(name: string, page: number, pageSize: number) {
    const params = new HttpParams()
      .set('name', name)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResult<ClassBase>>(this.baseUrl, { params });
  }

  getStudents(classId: string) {
    return this.http.get<StudentBase[]>(`${this.baseUrl}/getStudents/${classId}`);
  }

  getCourses(classId: string) {
    return this.http.get<CourseBase[]>(`${this.baseUrl}/getCourses/${classId}`);
  }

  addRemoveStudents(classId: string, request: AddRemoveStudentsRequest) {
    return this.http.post(`${this.baseUrl}/addRemoveStudents/${classId}`, request);
  }
}
