import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environment';
import { StudentBase } from '../models/student.model';
import { PaginatedResult } from '../models/paginated-result.mode';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/student`;

  getAll(name: string, email: string, phoneNumber: string, page: number, pageSize: number) {
    const params = new HttpParams()
      .set('name', name)
      .set('emailAddress', email)
      .set('phoneNumber', phoneNumber)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResult<StudentBase>>(this.base, { params });
  }
}
