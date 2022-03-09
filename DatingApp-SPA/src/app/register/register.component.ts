import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder,FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router} from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister=new EventEmitter();
  
  registerForm!:FormGroup;
  maxDate!:Date;
  validationErrors:string[]=[];

  constructor(private authService:AuthService,private fb:FormBuilder,private route:Router) { }

  ngOnInit() {
    this.initializeForm();
    this.maxDate=new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }

  initializeForm(){
    this.registerForm=this.fb.group({
      username:['',Validators.required],
      gender:['male'],
      knownAs:['',Validators.required],
      dateOfBirth:['',Validators.required],
      city:['',Validators.required],
      country:['',Validators.required],
      password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword:['',[Validators.required,this.matchValues('password')]]
    });
    this.registerForm.controls['password'].valueChanges.subscribe(()=>{
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    })
  }

  matchValues(matchTo:string):ValidatorFn{
    return (control:AbstractControl)=>{
      return control?.value===control?.parent?.get(matchTo)?.value?null:{isMatching:true};
    }
  }

  register(){
    this.authService.register(this.registerForm.value).subscribe({
      next:()=>{this.route.navigateByUrl('/members');},
      error:(err)=>{
        this.validationErrors=err;
      }     
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
