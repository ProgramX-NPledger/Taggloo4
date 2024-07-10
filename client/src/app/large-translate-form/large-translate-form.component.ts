import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FloatLabelModule } from 'primeng/floatlabel';
import { DropdownModule } from 'primeng/dropdown';
import { LanguagesService } from '../_services/languages.service';
import { Language } from '../_models/language';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-large-translate-form',
  standalone: true,
  imports: [ FormsModule, FloatLabelModule, DropdownModule, ButtonModule ],
  templateUrl: './large-translate-form.component.html',
  styleUrl: './large-translate-form.component.css'
})
export class LargeTranslateFormComponent implements OnInit {
  languagesService: LanguagesService = inject(LanguagesService);
  ngOnInit(): void {
    this.populateDropDowns();
  }

  translationModel: any = {};
  languages: any[] = [];
  isLoading: boolean = false;

  private populateDropDowns() {
    this.isLoading=true;
    const allLanguages = this.languagesService.getAvailableLanguages();
console.log(allLanguages);
//this.languages=allLanguages;
    this.isLoading=false;
  }

  translate() {

  }
}
