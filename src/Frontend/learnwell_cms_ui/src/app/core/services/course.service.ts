import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environment';
import { CourseBase } from '../models/course.model';
import { PaginatedResult } from '../models/paginated-result.mode';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class CourseService extends BaseService<CourseBase> {
  override readonly baseUrl = `${environment.apiUrl}/course`;

  getAll(name: string, page: number, pageSize: number) {
    const params = new HttpParams()
      .set('name', name)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResult<CourseBase>>(this.baseUrl, { params });
  }
}
