export interface Video {
  id: number;
  filename: string;
  duration: string;
  bitRate: string;
  codecName: string;
  resolution: string;
  size: string;
  status: string;
  availableResolutions: string[];
  thumbnails: string[];
}
