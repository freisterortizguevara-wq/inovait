import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-crear-estudiante',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './crear-estudiante.html',
  styleUrl: './crear-estudiante.css'
})
export class CrearEstudianteComponent {
  estudiante = {
    nombre: '',
    fechaNacimiento: '',
    colegioId: 1, 
    gradoId: 1,   
    anio: new Date().getFullYear(),
    grupo: ''
  };

  mensaje: string = '';

  constructor(private http: HttpClient) {}

  guardar() {
    // 1. Formatear la fecha a YYYY-MM-DD
    let fechaFormateada = this.estudiante.fechaNacimiento;
    if (fechaFormateada) {
      const partes = fechaFormateada.split('/');
      if (partes.length === 3) {
        fechaFormateada = `${partes[2]}-${partes[1]}-${partes[0]}`;
      }
    }

    // 2. Crear una copia del objeto con la fecha corregida
    const payload = {
      ...this.estudiante,
      fechaNacimiento: fechaFormateada
    };

    // 3. Petición a Render
    this.http.post('https://inovait-backend.onrender.com/api/Estudiantes', payload)
      .subscribe({
        next: (res: any) => {
          this.mensaje = '¡Estudiante guardado con éxito!';
          this.estudiante.nombre = '';
          this.estudiante.grupo = '';
        },
        error: (err) => {
          this.mensaje = 'Error al guardar: ' + (err.error?.error || err.message);
        }
      });
  }
}