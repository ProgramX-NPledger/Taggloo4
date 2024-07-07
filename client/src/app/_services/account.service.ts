import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = "http://localhost:5067/api/v4/";

  login(userName: string, password: string) {
    return this.http.post(this.baseUrl+'login', {
      UserName: userName,
      Password: password
    });

  }

}
