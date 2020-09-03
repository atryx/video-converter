import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
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
  selectedValue: string;

  formats: OutputFormat[] = [
    { value: 'Hd480', viewValue: 'HD 480p' },
    { value: 'Hd720', viewValue: 'HD 720p' },
    { value: 'Hd1080', viewValue: 'HD 1080p' },
  ];

  constructor(private videoService: VideoService) {}

  ngOnInit(): void {}

  onClick() {
    const fileInput = this.fileInput.nativeElement;
    fileInput.onchange = () => {
      this.file = fileInput.files;
    };
    fileInput.click();
  }

  onFileSelect(event) {
    console.log(event);
  }

  callVideoService() {
    this.fileInput.nativeElement.value = '';
    const formData = new FormData();
    formData.append('file', this.file);
    formData.append('outputFormat', this.selectedValue);

    this.videoService.convertToFormat(formData).subscribe(
      (video) => {
        console.log(video);
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
