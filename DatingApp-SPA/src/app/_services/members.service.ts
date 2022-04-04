import { HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/Member';
import { User } from '../_models/user';
import { UserParams } from '../_models/UserParams';
import { AuthService } from './auth.service';
import { getPaginatedResult, getPaginationHeaders } from './PaginationHelpers';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl=environment.apiUrl;
  members:Member[]=[];
  memberCache=new Map();
  user!: User;
  userParams!: UserParams;
 
  constructor(private http:HttpClient,private authService:AuthService) {
    this.authService.currentUser$.pipe(take(1)).subscribe(u=>{
      this.user=u;
      this.userParams=new UserParams(u);
    });
   }

   getUserParams(){
     return this.userParams;
   }

   setUserParams(params:UserParams){
     this.userParams=params;
   }

   resetUserParams(){
     this.userParams=new UserParams(this.user);
     return this.userParams;
   }

  getMembers(userParams:UserParams){
    var response=this.memberCache.get(Object.values( userParams).join('-'));

    if (response) {
      return of(response);
    }
    
    let params=getPaginationHeaders(userParams.pageNumber,userParams.pageSize);

    params=params.append('minAge',userParams.minAge.toString());
    params=params.append('maxAge',userParams.maxAge.toString());
    params=params.append('gender',userParams.gender);
    params=params.append('orderBy',userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl+'users',params,this.http)
    .pipe(map(x=>{
      this.memberCache.set(Object.values( userParams).join('-'),x);
      return x;
    }));
  }

  addLike(username:string){
    return this.http.post(this.baseUrl+'likes/'+username,{});
  }
  
  getLikes(predicate:string,pageNumber:number,pageSize:number){
    let params=getPaginationHeaders(pageNumber,pageSize);
    params=params.append('predicate',predicate);

    return getPaginatedResult<Partial<Member[]>>(this.baseUrl+'likes',params,this.http);
  }

  getMember(username:string){
    const member=[...this.memberCache.values()]
    .reduce((arr,elem)=>arr.concat(elem.result),[])
    .find((x:Member)=>x.username==username);

    if (member) {
      return of(member);
    }
    
    return this.http.get<Member>(this.baseUrl+'users/'+username);
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl+'users',member).pipe(
      map(()=>{
        const index=this.members.indexOf(member);
        this.members[index]=member;
      })
    );
  }

  setMainPhoto(photoId:number){
    return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId,{});
  }

  deletePhoto(photoId:number){
    return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
  }

 
}
