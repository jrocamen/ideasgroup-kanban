import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BoardService } from '../../../core/services/board.service';
import { ProjectService } from '../../../core/services/project.service';
import { SignalrService } from '../../../core/services/signalr.service';
import { AuthService } from '../../../core/services/auth.service';
import { KanbanColumn, KanbanTask } from '../../../core/models/kanban.models';
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Subscription } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { AvatarGroupModule } from 'primeng/avatargroup';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DragDropModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputTextareaModule,
    DropdownModule,
    TagModule,
    AvatarModule,
    AvatarGroupModule,
    TooltipModule
  ],
  templateUrl: './kanban-board.component.html'
})
export class KanbanBoardComponent implements OnInit, OnDestroy {
  projectId: string = '';
  projectName: string = 'Tablero Kanban';
  columns: KanbanColumn[] = [];
  
  columnDialog = false;
  newColumnName = '';

  filterSearchText: string = '';
  filterPriority: string | null = null;
  filterAssignee: string | null = null;
  users: any[] = [];
  activeUsers: string[] = [];

  taskDialog = false;
  isEditTaskMode = false;
  newTask: Partial<KanbanTask> = {};
  selectedColumnId: string = '';
  
  priorities = [
    { label: 'Baja', value: 'Low' },
    { label: 'Media', value: 'Medium' },
    { label: 'Alta', value: 'High' }
  ];

  private subs = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private boardService: BoardService,
    private projectService: ProjectService,
    private signalrService: SignalrService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') || '';
    if (!this.projectId) {
      this.router.navigate(['/projects']);
      return;
    }

    this.projectService.getProject(this.projectId).subscribe(project => {
        if (project) {
            this.projectName = project.name;
        }
    });

    this.authService.getUsers().subscribe(users => {
        this.users = users;
    });

    this.loadBoard();
    this.signalrService.startConnection();
    this.setupSignalRListeners();

    const currentUser = this.authService.currentUserValue;
    if (currentUser) {
        this.signalrService.subscribeToProject(this.projectId, currentUser.name);
    }
  }

  ngOnDestroy(): void {
    this.signalrService.unsubscribeFromProject(this.projectId);
    this.subs.unsubscribe();
    this.signalrService.stopConnection();
  }

  loadBoard() {
    this.boardService.getColumns(this.projectId).subscribe(cols => {
      this.columns = cols;
      this.boardService.getTasks(this.projectId).subscribe(tasks => {
        this.columns.forEach(col => {
          col.tasks = tasks.filter(t => t.columnId === col.id).sort((a, b) => a.order - b.order);
        });
      });
    });
  }

  setupSignalRListeners() {
    this.subs.add(this.signalrService.taskCreated$.subscribe(task => {
      const col = this.columns.find(c => c.id === task.columnId);
      if (col && col.tasks && !col.tasks.find(t => t.id === task.id)) {
        col.tasks.push(task);
      }
    }));

    this.subs.add(this.signalrService.taskMoved$.subscribe(data => {
      // Re-fetch board or handle locally to avoid race conditions 
      // For a robust implementation, usually re-fetching or applying local patches is needed
      this.loadBoard();
    }));

    this.subs.add(this.signalrService.taskUpdated$.subscribe(data => {
      this.loadBoard();
    }));

    this.subs.add(this.signalrService.columnCreated$.subscribe(col => {
      if (col.projectId === this.projectId && !this.columns.find(c => c.id === col.id)) {
        col.tasks = [];
        this.columns.push(col);
      }
    }));
    
    this.subs.add(this.signalrService.columnDeleted$.subscribe(id => {
      this.columns = this.columns.filter(c => c.id !== id);
    }));
    
    this.subs.add(this.signalrService.taskDeleted$.subscribe(id => {
       this.columns.forEach(col => {
           if(col.tasks) col.tasks = col.tasks.filter(t => t.id !== id);
       });
    }));

    this.subs.add(this.signalrService.activeUsers$.subscribe(users => {
        this.activeUsers = users;
    }));
  }

  goBack() {
    this.router.navigate(['/projects']);
  }

  openNewColumnDialog() {
    this.newColumnName = '';
    this.columnDialog = true;
  }

  saveColumn() {
    if (this.newColumnName.trim()) {
      this.boardService.createColumn({ name: this.newColumnName, projectId: this.projectId })
        .subscribe({
          next: () => {
            this.columnDialog = false;
          },
          error: (err) => {
            console.error('Error creating column', err);
            alert('Error al crear columna: ' + (err.error?.message || err.message));
          }
        });
    }
  }

  deleteColumn(id: string) {
    this.boardService.deleteColumn(id).subscribe({
        next: () => {
            // SignalR handles the visual removal or we can do it locally too
        },
        error: (err) => {
            alert(err.error?.message || 'Cannot delete column');
        }
    });
  }

  deleteTask(id: string) {
    this.boardService.deleteTask(id).subscribe({
        next: () => {
            // SignalR handles the visual removal
        },
        error: (err) => {
            alert(err.error?.message || 'Error al eliminar tarea');
        }
    });
  }

  openNewTaskDialog(col: KanbanColumn) {
    this.selectedColumnId = col.id;
    this.newTask = { priority: 'Medium' };
    this.isEditTaskMode = false;
    this.taskDialog = true;
  }

  editTask(task: KanbanTask, event: Event) {
    event.stopPropagation();
    this.newTask = { ...task };
    this.selectedColumnId = task.columnId;
    this.isEditTaskMode = true;
    this.taskDialog = true;
  }

  saveTask() {
    const user = this.authService.currentUserValue;
    if (this.newTask.title?.trim() && user) {
      const payload = {
        title: this.newTask.title,
        description: this.newTask.description || '',
        priority: this.newTask.priority,
        columnId: this.selectedColumnId,
        assigneeId: user.id
      };

      if (this.isEditTaskMode && this.newTask.id) {
        this.boardService.updateTask(this.newTask.id, payload).subscribe({
          next: () => {
            this.loadBoard(); // Force local refresh immediately
            this.taskDialog = false;
          },
          error: (err) => {
            alert('Error al actualizar tarea: ' + (err.error?.message || err.message));
          }
        });
      } else {
        this.boardService.createTask(payload).subscribe({
          next: () => {
            this.taskDialog = false;
          },
          error: (err) => {
            alert('Error al crear tarea: ' + (err.error?.message || err.message));
          }
        });
      }
    }
  }

  drop(event: CdkDragDrop<KanbanTask[]>, newColumnId: string) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
    }

    const task = event.container.data[event.currentIndex];
    
    // Call API to save movement
    this.boardService.moveTask({
        taskId: task.id,
        newColumnId: newColumnId,
        newOrder: event.currentIndex
    }).subscribe();
  }

  getPrioritySeverity(priority: string): 'success' | 'secondary' | 'info' | 'warning' | 'danger' | 'contrast' | undefined {
      switch(priority) {
          case 'High': return 'danger';
          case 'Medium': return 'info';
          case 'Low': return 'success';
          default: return 'info';
      }
  }

  getPriorityLabel(priority: string): string {
      switch(priority) {
          case 'High': return 'Alta';
          case 'Medium': return 'Media';
          case 'Low': return 'Baja';
          default: return priority;
      }
  }

  matchesFilter(task: KanbanTask): boolean {
    if (this.filterPriority && task.priority !== this.filterPriority) return false;
    if (this.filterAssignee && task.assigneeId !== this.filterAssignee) return false;
    
    if (this.filterSearchText) {
        const lowerSearch = this.filterSearchText.toLowerCase();
        const matchesTitle = task.title.toLowerCase().includes(lowerSearch);
        const matchesDesc = (task.description || '').toLowerCase().includes(lowerSearch);
        if (!matchesTitle && !matchesDesc) return false;
    }
    
    return true;
  }

  exportReport(format: string) {
    this.projectService.exportProject(this.projectId, format, this.filterPriority, this.filterAssignee).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Reporte_${format}.${format === 'excel' ? 'xlsx' : 'pdf'}`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    });
  }
}
