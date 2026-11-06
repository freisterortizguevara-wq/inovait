import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-consultar-estudiantes',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './consultar-estudiantes.html',
  styleUrl: './consultar-estudiantes.css'
})
export class ConsultarEstudiantesComponent {
  filtro = {
    colegioId: 1,
    gradoId: 1,
    anio: 2026 // <-- Ajusta este año si es necesario
  };
    exportarExcel() {
    if (this.estudiantes.length === 0) {
      alert('No hay datos para exportar.');
      return;
    }
    
    // Creamos la hoja de cálculo
    const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.estudiantes);
    const workbook: XLSX.WorkBook = { Sheets: { 'Estudiantes': worksheet }, SheetNames: ['Estudiantes'] };
    
    // Generamos y descargamos el archivo
    XLSX.writeFile(workbook, 'Reporte_Estudiantes.xlsx');
  }
  estudiantes: any[] = [];
  buscado: boolean = false;

  constructor(private http: HttpClient) {}

  buscar() {
    const url = `/api/Estudiantes/filtrar?colegioId=${this.filtro.colegioId}&gradoId=${this.filtro.gradoId}&anio=${this.filtro.anio}`;
    
    this.http.get<any[]>(url).subscribe({
      next: (data) => {
        this.estudiantes = data;
        this.buscado = true;
      },
      error: (err) => {
        alert('Error al consultar: ' + (err.error?.error || err.message));
      }
    });
  }

  // --- FUNCIÓN DE ELIMINAR (Asegúrate de que esté aquí) ---
  eliminarEstudiante(id: number) {
    if (confirm('¿Estás seguro de que deseas eliminar este estudiante?')) {
      this.http.delete(`/api/Estudiantes/${id}`).subscribe({
        next: () => {
          alert('Estudiante eliminado con éxito.');
          this.buscar(); // Recargar la tabla
        },
        error: (err) => {
          alert('Error al eliminar: ' + (err.error?.error || err.message));
        }
      });
    }
  }

  // --- FUNCIÓN DE ACTUALIZAR (Asegúrate de que esté aquí) ---
  actualizarEstudiante(est: any) {
    const nuevoNombre = prompt('Ingrese el nuevo nombre:', est.nombreEstudiante);
    if (!nuevoNombre) return; 

    const nuevoGrupo = prompt('Ingrese el nuevo grupo:', est.grupo);
    if (!nuevoGrupo) return;

    const nuevoAnioStr = prompt('Ingrese el nuevo año:', est.anio.toString());
    const nuevoAnio = nuevoAnioStr ? parseInt(nuevoAnioStr) : est.anio;

    const dtoActualizacion = {
      nombre: nuevoNombre,
      fechaNacimiento: new Date().toISOString(), 
      colegioId: this.filtro.colegioId,
      gradoId: this.filtro.gradoId,
      anio: nuevoAnio,
      grupo: nuevoGrupo
    };

    this.http.put(`/api/Estudiantes/${est.estudianteId}`, dtoActualizacion).subscribe({
      next: () => {
        alert('Estudiante actualizado con éxito.');
        this.buscar();
      },
      error: (err) => {
        alert('Error al actualizar: ' + (err.error?.error || err.message));
      }
    });
  }
} // <-- ESTA LLAVE DEBE ESTAR AL FINAL DE TODO