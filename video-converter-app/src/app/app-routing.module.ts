import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VideosComponent } from './videos/videos.component';
import { VideoDetailComponent } from './video-detail/video-detail.component';
import { UploadComponent } from './upload/upload.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

const routes: Routes = [
  { path: 'videos', component: VideosComponent },
  { path: 'uploads', component: UploadComponent },
  { path: 'video/:id', component: VideoDetailComponent },
  { path: '', redirectTo: '/videos', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
