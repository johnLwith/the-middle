import axios from 'axios';
import { NlpAnalysisResponse } from '../types/nlp';

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
  id: number;
  startTime: string;
  endTime: string;
  text: string;
  analysis?: NlpAnalysisResponse;
}

export interface TranslationResponse {
  translation: string;
}

export const api = {
  translate: {
    getTranslation: async (word: string): Promise<TranslationResponse> => {
      const response = await axios.get(`${API_BASE_URL}/Translate?word=${word}`);
      return response.data;
    }
  },
  wordbook: {
    addWord: async (word: string, episodeId: string): Promise<void> => {
      await axios.post(`${API_BASE_URL}/Wordbook`, { word, episodeId });
    }
  },
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
    },
    analyzeSubtitle: async (text: string): Promise<NlpAnalysisResponse> => {
      const response = await axios.get(`${API_BASE_URL}/Episodes/analyze?text=${text}`);
      return response.data;
    }
  }
};