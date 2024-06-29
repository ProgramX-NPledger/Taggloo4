import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{

  http = inject(HttpClient)

  title = 'client';
  languages: any;

  ngOnInit(): void {
    this.http.get('http://localhost:5067/api/v4/languages').subscribe({
      next: response => {
        this.languages = response;
      },
      error: error => {
        console.log(error)
      },
      complete: () => {}
    })
  }

}
