import { Routes } from '@angular/router';
import { CrearEstudianteComponent } from './crear-estudiante/crear-estudiante';
import { ConsultarEstudiantesComponent } from './consultar-estudiantes/consultar-estudiantes';
import { DashboardComponent } from './dashboard/dashboard'; // <-- Nuevo

export const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent }, // <-- Nueva ruta
  { path: 'crear', component: CrearEstudianteComponent },
  { path: 'consultar', component: ConsultarEstudiantesComponent },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' }, // Ahora inicia en el dashboard
  { path: '**', redirectTo: '/dashboard' }
];