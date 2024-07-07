import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule,ToastModule],
  providers: [MessageService],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})

export class NavComponent {
  private accountService:AccountService=inject(AccountService);
  private messageService:MessageService=inject(MessageService);

  credentialsModel:any = {}
  currentUser: any|null = null;

  login() {
    this.accountService.login(this.credentialsModel.userName, this.credentialsModel.password).subscribe({
      next: response => {
        console.log(response);
        this.currentUser=response;
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
}
