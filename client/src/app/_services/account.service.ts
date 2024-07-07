import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { map } from 'rxjs';
import { LoggedInUser } from '../_models/logged-in-user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = "http://localhost:5067/api/v4/";
  currentUser = signal<LoggedInUser | null>(null);
  login(userName: string, password: string) {
    return this.http.post<LoggedInUser>(this.baseUrl+'login', {
      UserName: userName,
      Password: password
    }).pipe(
      map(loggedInUser => {
        if (loggedInUser) {
          localStorage.setItem('user',JSON.stringify(loggedInUser));
          this.currentUser.set(loggedInUser);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set (null);
  }

}
