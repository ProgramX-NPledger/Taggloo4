import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastModule } from 'primeng/toast';
import {MenuItem, MessageService } from 'primeng/api';
import { AvatarModule } from 'primeng/avatar';
import { MenuModule } from 'primeng/menu';
import { LoggedInUser } from '../_models/logged-in-user';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule,ToastModule,AvatarModule,MenuModule],
  providers: [MessageService],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})

export class NavComponent {
  accountService:AccountService=inject(AccountService);
  private messageService:MessageService=inject(MessageService);

  credentialsModel:any = {}

  loggedInMenuItems:MenuItem[] = [
    {
      label: 'Profile',
    },
    {
      label: 'Logout',
    }
  ];

  login() {
    this.accountService.login(this.credentialsModel.userName, this.credentialsModel.password).subscribe({
      next: response => {
        console.log(response);
        this.messageService.add({
          summary: 'Login successful',
          detail: 'Welcome, '+this.credentialsModel.userName,
          severity: 'success'
        })
      },
      error: error => {
        if (error.status===401) {
          this.messageService.add({
            summary: 'Login unsuccessful',
            detail: 'The usernmae/password provided was not recognised',
            severity: 'error'
          })
        } else {
          this.messageService.add({
            summary: 'Login error',
            detail: 'There was a problem attempting to log you in',
            severity: 'error'
          })

        }
        console.error(error);
      }
    })
  }

  logout() {
    this.accountService.logout();
  }
}
