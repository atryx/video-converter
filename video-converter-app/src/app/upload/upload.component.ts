import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { OutputFormat } from '../models/outputFormat';
import { VideoService } from '../video.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css'],
})
export class UploadComponent implements OnInit {
  @ViewChild('fileInput', { static: false }) fileInput: ElementRef;
  file;

  constructor(private videoService: VideoService, private router: Router) {}

  ngOnInit(): void {}

  onClick() {
    const fileInput = this.fileInput.nativeElement;
    fileInput.onchange = () => {
      this.file = fileInput.files[0];
    };
    fileInput.click();
  }
  deselectFile() {
    this.file = '';
  }

  callVideoService() {
    const formData = new FormData();
    formData.append('UploadedFile', this.file);

    this.videoService.uploadFile(formData).subscribe(
      (video) => {
        this.router.navigate(['video', video.id]);
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
