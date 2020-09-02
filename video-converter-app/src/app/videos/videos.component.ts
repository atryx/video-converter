import { Component, OnInit } from '@angular/core';
import { Video } from '../models/video';
import { VIDEOS } from '../mock-videos';

@Component({
  selector: 'app-videos',
  templateUrl: './videos.component.html',
  styleUrls: ['./videos.component.css'],
})
export class VideosComponent implements OnInit {
  video: Video = {
    id: 1,
    filename: 'fish_eats.mp4',
    duration: '00:10:00',
    bitRate: '48k',
    codecName: 'h264',
    resolution: '4k',
    status: 'FileUploaded',
    availableResolutions: ['Hd720', 'Hd480'],
    thumbnails: ['1', '2'],
  };
  videos = VIDEOS;

  selectedVideo: Video;
  onSelect(video: Video): void {
    this.selectedVideo = video;
  }

  constructor() {}

  ngOnInit(): void {}
}
