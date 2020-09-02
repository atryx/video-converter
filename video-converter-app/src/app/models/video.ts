export interface Video {
  id: number;
  filename: string;
  duration: string;
  bitRate: string;
  codecName: string;
  resolution: string;
  status: string;
  availableResolutions: string[];
  thumbnails: string[];
}
