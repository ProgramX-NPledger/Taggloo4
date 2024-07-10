import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { LanguagesResult } from '../_models/languages-result';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LanguagesService {
  private http = inject(HttpClient);
  baseUrl = "http://localhost:5067/api/v4/";

  getAvailableLanguages() {
    return this.http.get<LanguagesResult>(this.baseUrl+'languages').subscribe({
      next: response => {
        return response.results;
      },
      error: error => {
        throw new Error(error);
      },
      complete: () => {}
      }
    );
  }

  getOtherLanguage(firstLanguageTag: string, allLanguageTags: string[]) {

  }
}
