import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any={}
  

  constructor(public authService:AuthService,private toastr:ToastrService,private router:Router) { }

  ngOnInit() {
    
  }

  login(){
    this.authService.login(this.model).subscribe({
      next:(response)=>{this.router.navigateByUrl('/members');}      
    });
  }

 

  logout(){
    this.authService.logout();
   
    this.router.navigateByUrl('/');
  }

}
