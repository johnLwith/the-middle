import axios from 'axios';

const API_BASE_URL = 'http://localhost:5001/api';

export interface Episode {
  id: string;
  seasonNumber: number;
  episodeNumber: number;
  title: string;
  description: string;
  subtitlePath?: string;
  audioPath?: string;
}

export interface Subtitle {
  startTime: string;
  endTime: string;
  text: string;
}

export const api = {
  episodes: {
    getAll: async (): Promise<Episode[]> => {
      const response = await axios.get(`${API_BASE_URL}/Episodes`);
      return response.data;
    },
    getById: async (id: string): Promise<Episode> => {
      const response = await axios.get(`${API_BASE_URL}/Episodes/${id}`);
      return response.data;
    },
    getSubtitles: async (id: string): Promise<Subtitle[]> => {
      const response = await axios.get(`${API_BASE_URL}/Episodes/${id}/subtitles`);
      return response.data;
    }
  }
}; 