import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/Photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  unApprovedPhotos:Photo[]=[];

  constructor(private adminService:AdminService) { }

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval(){
    this.adminService.getPhotosForApproval().subscribe(next=>{
      this.unApprovedPhotos=next;
    })
}
  approvePhoto(photoId:number){
    this.adminService.approvePhoto(photoId).subscribe(()=>{
      this.unApprovedPhotos.splice(this.unApprovedPhotos.findIndex(p=>p.id==photoId),1);
    })
  }

  rejectPhoto(photoId:number){
    this.adminService.rejectPhoto(photoId).subscribe(()=>{
      this.unApprovedPhotos.splice(this.unApprovedPhotos.findIndex(p=>p.id==photoId),1);
    })
  }
}
