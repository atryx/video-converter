import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VideoService } from '../video.service';
import { Video } from '../models/video';
import { OutputFormat } from '../models/outputFormat';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'app-video-detail',
  templateUrl: './video-detail.component.html',
  styleUrls: ['./video-detail.component.css'],
})
export class VideoDetailComponent implements OnInit {
  id;
  selectedValue: string;
  video: Video;
  isLoading = true;

  formats: OutputFormat[] = [
    { value: 'Hd480', viewValue: 'HD 480p' },
    { value: 'Hd720', viewValue: 'HD 720p' },
    { value: 'Hd1080', viewValue: 'HD 1080p' },
  ];
  constructor(
    private _Activatedroute: ActivatedRoute,
    private videoService: VideoService
  ) {}

  ngOnInit(): void {
    this._Activatedroute.paramMap.subscribe((params) => {
      this.id = params.get('id');
      this.getVideos(this.id);
    });
  }

  getVideos(id: number) {
    this.videoService.getVideoById(id).subscribe(
      (video) => {
        this.video = video;
        this.isLoading = false;
        console.log(this.video);
      },
      (error) => {
        this.isLoading = false;
        console.log(error);
      }
    );
  }

  generateHLS() {
    console.log('generate HLS');
  }

  getThumbnails() {
    console.log('getting thumbnails');
  }

  convertToOutput() {
    console.log('gconvert to output');
  }

  download(filename) {
    this.isLoading = true;
    console.log('download clicked', filename);
    this.videoService.downloadFile(filename).subscribe((response) => {
      console.log(response);
      this.isLoading = false;
      let blob: any = new Blob([response.fileContents], {
        type: response.contentType,
      });
      // const url = window.URL.createObjectURL(blob);
      fileSaver.saveAs(blob, response.fileName);
    }),
      (error) => console.log('Error downloading the file'),
      () => console.info('File downloaded successfully');
  }
}
