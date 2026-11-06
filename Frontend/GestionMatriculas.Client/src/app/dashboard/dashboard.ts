import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BaseChartDirective } from 'ng2-charts';
import { 
  ChartConfiguration, ChartData, ChartType, Chart, 
  CategoryScale, LinearScale, BarController, BarElement, 
  DoughnutController, ArcElement, Legend, Tooltip 
} from 'chart.js';

Chart.register(
  CategoryScale, LinearScale, BarController, BarElement, 
  DoughnutController, ArcElement, Legend, Tooltip
);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  
  totales = { totalEstudiantes: 0, totalColegios: 0, totalDocentes: 0 };
  edades = { entre3y7: 0, entre8y12: 0, mayores12: 0 };

  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { grid: { color: 'rgba(255,255,255,0.1)' }, ticks: { color: '#94a3b8' } },
      y: { grid: { color: 'rgba(255,255,255,0.1)' }, ticks: { color: '#94a3b8' } }
    },
    plugins: { legend: { labels: { color: '#e2e8f0' } } }
  };
  public barChartType: ChartType = 'bar';
  public barChartData: ChartData<'bar'> = {
    labels: [],
    datasets: [{ data: [], label: 'Estudiantes Matriculados', backgroundColor: '#38bdf8', borderRadius: 6 }]
  };

  public doughnutChartType: ChartType = 'doughnut';
  public doughnutChartData: ChartData<'doughnut'> = {
    labels: [],
    datasets: [{ data: [], backgroundColor: ['#818cf8', '#38bdf8'], borderWidth: 0 }]
  };
  public doughnutChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { position: 'bottom', labels: { color: '#e2e8f0', padding: 20 } } }
  };

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.cargarEstadisticas();
  }

  cargarEstadisticas() {
    // URL actualizada a Render
    this.http.get<any>('https://inovait-backend.onrender.com/api/Dashboard/estadisticas').subscribe({
      next: (data) => {
        console.log("Datos recibidos del Backend:", data);
        
        // Actualizamos los datos
        this.totales = { ...data.totales };
        this.edades = { ...data.edades };

        this.barChartData.labels = data.graficaGrados.map((g: any) => g.grado);
        this.barChartData.datasets[0].data = data.graficaGrados.map((g: any) => g.cantidad);
        this.barChartData = { ...this.barChartData }; 

        this.doughnutChartData.labels = data.graficaSectores.map((s: any) => s.sector);
        this.doughnutChartData.datasets[0].data = data.graficaSectores.map((s: any) => s.cantidad);
        this.doughnutChartData = { ...this.doughnutChartData };

        // FORZAMOS A ANGULAR A REDIBUJAR LA PANTALLA
        setTimeout(() => {
          this.cdr.detectChanges();
        }, 0);
      },
      error: (err) => {
        console.error('Error al cargar estadísticas:', err);
      }
    });
  }
}