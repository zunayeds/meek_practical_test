import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environment';

@Injectable({ providedIn: 'root' })
export class BaseService<T> {
  protected readonly http = inject(HttpClient);
  protected readonly baseUrl = `${environment.apiUrl}/`;

  getById(id: string) {
    return this.http.get<T>(`${this.baseUrl}/${id}`);
  }

  create(request: any) {
    return this.http.post<T>(this.baseUrl, request);
  }
  
  update(id: string, request: T) {
    return this.http.put<T>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string) {
    return this.http.delete<T>(`${this.baseUrl}/${id}`);
  }
}