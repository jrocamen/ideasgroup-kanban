import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Project } from '../models/kanban.models';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = `${environment.apiUrl}/projects`;

  constructor(private http: HttpClient) { }

  getProjects(searchTerm?: string, page: number = 1, size: number = 10): Observable<{items: Project[], totalCount: number}> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('size', size.toString());
      
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    return this.http.get<{items: Project[], totalCount: number}>(this.apiUrl, { params });
  }

  getProject(id: string): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${id}`);
  }

  createProject(project: Partial<Project>): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, project);
  }

  updateProject(id: string, project: Partial<Project>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, project);
  }

  deleteProject(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  exportProject(id: string, format: string, priority?: string | null, assigneeId?: string | null): Observable<Blob> {
    let url = `${this.apiUrl}/${id}/export?format=${format}`;
    if (priority) url += `&priority=${priority}`;
    if (assigneeId) url += `&assigneeId=${assigneeId}`;

    return this.http.get(url, { responseType: 'blob' });
  }
}
