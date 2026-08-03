import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Subject } from 'rxjs';
import { KanbanColumn, KanbanTask } from '../models/kanban.models';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection | undefined;

  public taskCreated$ = new Subject<KanbanTask>();
  public taskUpdated$ = new Subject<{id: string, dto: any}>();
  public taskDeleted$ = new Subject<string>();
  public taskMoved$ = new Subject<any>();

  public columnCreated$ = new Subject<KanbanColumn>();
  public columnUpdated$ = new Subject<{id: string, dto: any}>();
  public columnDeleted$ = new Subject<string>();
  public columnOrderUpdated$ = new Subject<any[]>();

  public activeUsers$ = new BehaviorSubject<string[]>([]);

  constructor() { }

  public startConnection() {
    const token = localStorage.getItem('token');
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl, {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));

    this.addListeners();
  }

  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  public subscribeToProject(projectId: string, userName: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SubscribeToProject', projectId, userName);
    } else if (this.hubConnection) {
      // If not connected yet, wait for it
      const interval = setInterval(() => {
        if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
          this.hubConnection.invoke('SubscribeToProject', projectId, userName);
          clearInterval(interval);
        }
      }, 500);
    }
  }

  public unsubscribeFromProject(projectId: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('UnsubscribeFromProject', projectId);
    }
  }

  private addListeners() {
    if (!this.hubConnection) return;

    this.hubConnection.on('TaskCreated', (task: KanbanTask) => this.taskCreated$.next(task));
    this.hubConnection.on('TaskUpdated', (data) => this.taskUpdated$.next(data));
    this.hubConnection.on('TaskDeleted', (id: string) => this.taskDeleted$.next(id));
    this.hubConnection.on('TaskMoved', (data) => this.taskMoved$.next(data));

    this.hubConnection.on('ColumnCreated', (col: KanbanColumn) => this.columnCreated$.next(col));
    this.hubConnection.on('ColumnUpdated', (data) => this.columnUpdated$.next(data));
    this.hubConnection.on('ColumnDeleted', (id: string) => this.columnDeleted$.next(id));
    this.hubConnection.on('ColumnOrderUpdated', (data) => this.columnOrderUpdated$.next(data));

    this.hubConnection.on('ActiveUsersUpdated', (users: string[]) => this.activeUsers$.next(users));
  }
}
