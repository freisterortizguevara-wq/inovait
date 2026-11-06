import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CrearEstudiante } from './crear-estudiante';

describe('CrearEstudiante', () => {
  let component: CrearEstudiante;
  let fixture: ComponentFixture<CrearEstudiante>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CrearEstudiante],
    }).compileComponents();

    fixture = TestBed.createComponent(CrearEstudiante);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
