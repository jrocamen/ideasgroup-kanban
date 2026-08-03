import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { KanbanColumn, KanbanTask } from '../models/kanban.models';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private colUrl = `${environment.apiUrl}/columns`;
  private taskUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) { }

  getColumns(projectId: string): Observable<KanbanColumn[]> {
    return this.http.get<KanbanColumn[]>(`${this.colUrl}/project/${projectId}`);
  }

  createColumn(column: Partial<KanbanColumn>): Observable<KanbanColumn> {
    return this.http.post<KanbanColumn>(this.colUrl, column);
  }

  updateColumn(id: string, column: Partial<KanbanColumn>): Observable<void> {
    return this.http.put<void>(`${this.colUrl}/${id}`, column);
  }

  deleteColumn(id: string): Observable<void> {
    return this.http.delete<void>(`${this.colUrl}/${id}`);
  }

  updateColumnOrder(orderData: any[]): Observable<void> {
    return this.http.put<void>(`${this.colUrl}/order`, orderData);
  }

  getTasks(projectId: string): Observable<KanbanTask[]> {
    return this.http.get<KanbanTask[]>(`${this.taskUrl}/project/${projectId}`);
  }

  createTask(task: Partial<KanbanTask>): Observable<KanbanTask> {
    return this.http.post<KanbanTask>(this.taskUrl, task);
  }

  updateTask(id: string, task: Partial<KanbanTask>): Observable<void> {
    return this.http.put<void>(`${this.taskUrl}/${id}`, task);
  }

  deleteTask(id: string): Observable<void> {
    return this.http.delete<void>(`${this.taskUrl}/${id}`);
  }

  moveTask(moveData: any): Observable<void> {
    return this.http.put<void>(`${this.taskUrl}/move`, moveData);
  }
}
