import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/Member';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm')
  editForm!: NgForm;
  member!: Member;
  user!: User;
  @HostListener('window:beforeunload',['$event']) unloadNotification($event:any){
    if(this.editForm.dirty){
      $event.returnValue=true;
    }
  }

  constructor(private authService:AuthService,private membersService:MembersService,private toastr:ToastrService) {
   this.authService.currentUser$.pipe(take(1)).subscribe(res=>this.user=res);
   }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.membersService.getMember(this.user.username).subscribe(m=>this.member=m);
  }

  updateMember(){
    this.membersService.updateMember(this.member).subscribe(()=>{
      this.toastr.success('Profile updated successfully');
      this.editForm.reset(this.member);
    })
    
  }
}
