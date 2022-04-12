import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map, Observable, take } from 'rxjs';
import { AuthService } from '../_services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private authService:AuthService,private toastr:ToastrService){}
  canActivate(): Observable<boolean> {
    return this.authService.currentUser$.pipe(
      map(user=>{
        if(user.roles.includes('Admin') || user.roles.includes('Moderator'))
        return true;
        this.toastr.error("You can't access this area");
        return false;
      })
    )
  }
  
}
