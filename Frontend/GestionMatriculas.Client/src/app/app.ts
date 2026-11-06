import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router'; // <-- Importante: Importar RouterLink

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink], // <-- Importante: Agregarlos aquí
  templateUrl: './app.html',           // Ojo: ahora se llama app.html
  styleUrl: './app.css'                // Ojo: ahora se llama app.css
})
export class App {
  title = 'GestionMatriculas.Client';
}