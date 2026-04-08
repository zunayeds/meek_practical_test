import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environment';
import { ClassBase } from '../models/class.model';
import { PaginatedResult } from '../models/paginated-result.mode';
import { BaseService } from './base.service';

@Injectable({ providedIn: 'root' })
export class ClassService extends BaseService<ClassBase> {
  override readonly baseUrl = `${environment.apiUrl}/class`;

  getAll(name: string, page: number, pageSize: number) {
    const params = new HttpParams()
      .set('name', name)
      .set('page', page)
      .set('pageSize', pageSize);
    return this.http.get<PaginatedResult<ClassBase>>(this.baseUrl, { params });
  }
}
