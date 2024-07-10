import { Component } from '@angular/core';
import { LargeTranslateFormComponent } from '../large-translate-form/large-translate-form.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [LargeTranslateFormComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
