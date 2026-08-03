import { Routes } from '@angular/router';
import { AppLayoutComponent } from './layout/app.layout/app.layout.component';
import { LoginComponent } from './features/auth/login/login.component';
import { ProjectListComponent } from './features/projects/project-list/project-list.component';
import { KanbanBoardComponent } from './features/board/kanban-board/kanban-board.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        component: AppLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            { path: '', redirectTo: 'projects', pathMatch: 'full' },
            { path: 'projects', component: ProjectListComponent },
            { path: 'board/:projectId', component: KanbanBoardComponent }
        ]
    },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: 'projects' }
];
