import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProjectService } from './project.service';
import { environment } from '../../../environments/environment';

describe('ProjectService', () => {
  let service: ProjectService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProjectService]
    });
    service = TestBed.inject(ProjectService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch projects with pagination', () => {
    const mockResponse = { items: [{ id: '1', name: 'Proj 1' }], totalCount: 1 };
    
    service.getProjects('test', 2, 5).subscribe(res => {
      expect(res.items.length).toBe(1);
      expect(res.totalCount).toBe(1);
      expect(res.items[0].name).toBe('Proj 1');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/projects?page=2&size=5&searchTerm=test`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch a single project by id', () => {
    const mockProject = { id: '1', name: 'Proj 1' };
    
    service.getProject('1').subscribe(res => {
      expect(res.name).toBe('Proj 1');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/projects/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockProject);
  });

  it('should create a project', () => {
    const mockPayload = { name: 'New Proj' };
    const mockResponse = { id: '2', name: 'New Proj' };
    
    service.createProject(mockPayload).subscribe(res => {
      expect(res.id).toBe('2');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/projects`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockPayload);
    req.flush(mockResponse);
  });

  it('should delete a project', () => {
    service.deleteProject('1').subscribe(res => {
      expect(res).toBeNull();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/projects/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
