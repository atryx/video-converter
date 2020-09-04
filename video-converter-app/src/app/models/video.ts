import { Thumbnail } from './thumbnail';

export interface Video {
  id: number;
  filename: string;
  duration: string;
  bitRate: string;
  codecName: string;
  resolution: string;
  size: string;
  status: string;
  availableResolutions: Video[];
  thumbnails: Thumbnail[];
}
