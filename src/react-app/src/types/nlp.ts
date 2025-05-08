export interface WordAnalysis {
  word: string;
  tag: string;
  type: string;
}

export interface NlpAnalysisResponse {
  text: string;
  analysis: WordAnalysis[];
}

export const POS_COLORS = {
  // Nouns
  NN: '#FFEB3B',
  NNS: '#FFEB3B',
  NNP: '#FFEB3B',
  NNPS: '#FFEB3B',
  
  // Verbs
  VB: '#2196F3',
  VBD: '#2196F3',
  VBG: '#2196F3',
  VBN: '#2196F3',
  VBP: '#2196F3',
  VBZ: '#2196F3',
  
  // Adjectives
  JJ: '#4CAF50',
  JJR: '#4CAF50',
  JJS: '#4CAF50'
};