import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../core/models/kanban.models';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CalendarModule } from 'primeng/calendar';
import { TagModule } from 'primeng/tag';
import { DropdownModule } from 'primeng/dropdown';
import { PaginatorModule } from 'primeng/paginator';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ButtonModule, 
    RippleModule, 
    InputTextModule, 
    DialogModule, 
    InputTextareaModule, 
    CalendarModule,
    TagModule,
    DropdownModule,
    PaginatorModule
  ],
  templateUrl: './project-list.component.html'
})
export class ProjectListComponent implements OnInit {
  projects: Project[] = [];
  totalRecords: number = 0;
  loading: boolean = false;
  
  // Pagination State
  currentSearchTerm: string = '';
  currentPage: number = 1;
  pageSize: number = 10;
  
  projectDialog: boolean = false;
  project: Partial<Project> = {};
  startDate: Date = new Date();
  expectedEndDate: Date = new Date();
  submitted: boolean = false;
  isEditMode: boolean = false;

  states = [
    { label: 'No Iniciado', value: 'NotStarted' },
    { label: 'En Progreso', value: 'Active' },
    { label: 'Completado', value: 'Completed' }
  ];

  constructor(private projectService: ProjectService, private router: Router) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects() {
    this.loading = true;
    this.projectService.getProjects(this.currentSearchTerm, this.currentPage, this.pageSize).subscribe(res => {
      this.projects = res.items;
      this.totalRecords = res.totalCount;
      this.loading = false;
    });
  }

  onPageChange(event: any) {
    this.currentPage = (event.first / event.rows) + 1;
    this.pageSize = event.rows;
    this.loadProjects();
  }

  onSearch(event: Event) {
    const term = (event.target as HTMLInputElement).value;
    this.currentSearchTerm = term;
    this.currentPage = 1; // Reset to first page when searching
    this.loadProjects();
  }

  openBoard(projectId: string) {
    this.router.navigate(['/board', projectId]);
  }

  openNew() {
    this.project = { state: 'NotStarted' };
    this.startDate = new Date();
    const nextMonth = new Date();
    nextMonth.setMonth(nextMonth.getMonth() + 1);
    this.expectedEndDate = nextMonth;
    
    this.submitted = false;
    this.isEditMode = false;
    this.projectDialog = true;
  }

  editProject(p: Project, event: Event) {
    event.stopPropagation();
    this.project = { ...p };
    this.startDate = new Date(p.startDate);
    this.expectedEndDate = new Date(p.expectedEndDate);
    this.submitted = false;
    this.isEditMode = true;
    this.projectDialog = true;
  }
  hideDialog() {
    this.projectDialog = false;
    this.submitted = false;
  }

  saveProject() {
    this.submitted = true;

    if (this.project.name?.trim()) {
      const payload = {
        name: this.project.name,
        description: this.project.description || '',
        startDate: this.startDate.toISOString(),
        expectedEndDate: this.expectedEndDate.toISOString(),
        state: this.project.state
      };

      if (this.isEditMode && this.project.id) {
        this.projectService.updateProject(this.project.id, payload).subscribe({
          next: () => {
            this.loadProjects();
            this.projectDialog = false;
            this.project = {};
          }
        });
      } else {
        this.projectService.createProject(payload).subscribe({
          next: (res) => {
            this.projects.push(res);
            this.projectDialog = false;
            this.project = {};
          }
        });
      }
    }
  }

  deleteProject(id: string, event: Event) {
    event.stopPropagation();
    if (confirm('¿Estás seguro de eliminar este proyecto y todo su contenido?')) {
        this.projectService.deleteProject(id).subscribe({
            next: () => {
                this.loadProjects();
            },
            error: (err) => {
                alert('No se pudo eliminar el proyecto. ' + (err.error?.message || ''));
            }
        });
    }
  }
}
