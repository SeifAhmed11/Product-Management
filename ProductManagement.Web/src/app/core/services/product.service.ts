import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly baseUrl = 'http://localhost:5000/api/products';

  constructor(private http: HttpClient) { }

  getAll(search: string, page: number, pageSize: number, includeDeleted: boolean): Observable<{ total: number, items: any[] }>{
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('includeDeleted', includeDeleted);
    if (search) params = params.set('search', search);
    return this.http.get<{ total: number, items: any[] }>(this.baseUrl, { params });
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`);
  }

  create(product: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, product);
  }

  update(id: number, product: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, product);
  }

  softDelete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
