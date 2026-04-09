import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environment';
import { Student, StudentBase, StudentClassResponse, StudentCourseResponse } from '../models/student.model';
import { PaginatedResult } from '../models/paginated-result.mode';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class StudentService extends BaseService<Student> { 
  override readonly baseUrl = `${environment.apiUrl}/student`;

  getAll(name: string, email: string, phoneNumber: string, page: number, pageSize: number) {
    const params = new HttpParams()
      .set('name', name)
      .set('emailAddress', email)
      .set('phoneNumber', phoneNumber)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResult<StudentBase>>(this.baseUrl, { params });
  }

  getClassesByStudentId(studentId: string) {
    return this.http.get<StudentClassResponse[]>(`${this.baseUrl}/getClasses/${studentId}`);
  }

  getCoursesByStudentId(studentId: string) {
    return this.http.get<StudentCourseResponse[]>(`${this.baseUrl}/getCourses/${studentId}`);
  }

  getClasses() {
    return this.http.get<StudentClassResponse[]>(`${this.baseUrl}/getClasses`);
  }

  getCourses() {
    return this.http.get<StudentCourseResponse[]>(`${this.baseUrl}/getCourses`);
  }

  getOtherStudentsInClass(classId: string) {
    return this.http.get<string[]>(`${this.baseUrl}/getOtherStudents/${classId}`);
  }
}
