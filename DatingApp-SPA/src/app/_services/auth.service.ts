import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl='http://localhost:5000/api/auth/';
private currentUserSource=new ReplaySubject<User>(1);
currentUser$=this.currentUserSource.asObservable();

constructor(private http:HttpClient) { }

login(model:any){
  return this.http.post<User>(this.baseUrl+'login',model)
  .pipe(
    map((response:User)=>{
      const user=response;
      if(user){
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }
        
    })
  )
}

setCurrentUser(user:User){
  this.currentUserSource.next(user);
}

register(model:any){
  return this.http.post<User>(this.baseUrl+'register',model).pipe(
    map((user:User)=>{
      if (user) {
        localStorage.setItem('user',JSON.stringify(user));
        this.currentUserSource.next(user);
      }
    })
  );
}

logout(){
  localStorage.removeItem('user');
  this.currentUserSource.next(null!);
}
}
