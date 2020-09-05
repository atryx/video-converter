import { Thumbnail } from './thumbnail';
import { HLSFile } from './hlsFile';

export interface Video {
  id: number;
  filename: string;
  fileDirectory: string;
  duration: string;
  bitRate: string;
  codecName: string;
  resolution: string;
  size: string;
  status: string;
  availableResolutions: Video[];
  thumbnails: Thumbnail[];
  hlsFiles: HLSFile[];
}
