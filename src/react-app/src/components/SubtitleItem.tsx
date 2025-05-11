import React, { useState } from 'react';
import { api } from '../services/api';
import { POS_COLORS } from '../types/nlp';
import './SubtitleItem.css';

interface SubtitleItemProps {
  episodeId: string;
  startTime: string;
  endTime: string;
  text: string;
  isActive?: boolean;
  onClick: (startTime: string) => void;
}

const SubtitleItem: React.FC<SubtitleItemProps> = ({
  episodeId,
  startTime,
  endTime,
  text,
  isActive = false,
  onClick
}) => {
  const formatTime = (timeString: string) => {
    const [hours, minutes, seconds] = timeString.split(':');
    return `${hours}:${minutes}:${seconds.split('.')[0]}`;
  };

  const [showTooltip, setShowTooltip] = useState(false);
  const [currentWord, setcurrentWord] = useState("");
  const [translation, setTranslation] = useState("");
  const [isTranslating, setIsTranslating] = useState(false);
  const [addedWords, setAddedWords] = useState<Set<string>>(new Set());
  const [successMessage, setSuccessMessage] = useState<string>("");
  const [analysis, setAnalysis] = useState<{[key: string]: { tag: string, type: string }}>({});
  const [isAnalyzing, setIsAnalyzing] = useState(false);

  const handleTranslate = async () => {
    try {
      setIsTranslating(true);
      const response = await api.translate.getTranslation(currentWord);
      setTranslation(response.translation);
    } catch (error) {
      console.error('Translation error:', error);
      setTranslation('Translation failed');
    } finally {
      setIsTranslating(false);
    }
  };

  const handleAddToWordBook = async () => {
    try {
      await api.wordbook.addWord(currentWord, episodeId);
      setAddedWords(prev => new Set(prev).add(currentWord));
      setSuccessMessage(`"${currentWord}" added to wordbook`);
      setTimeout(() => setSuccessMessage(""), 2000);
    } catch (error) {
      console.error('Failed to add word to wordbook:', error);
    }
  };

  const handleAnalyze = async (text: string) => {
    try {
      setIsAnalyzing(true);
      const response = await api.episodes.analyzeSubtitle(text);
      const analysisMap = response.analysis.reduce((acc, item) => {
        const word = item.word.toLowerCase().replace(/[.,!?"']/g, '');
        acc[word] = { tag: item.tag, type: item.type };
        return acc;
      }, {} as {[key: string]: { tag: string, type: string }});
      setAnalysis(analysisMap);
    } catch (error) {
      console.error('Analysis error:', error);
    } finally {
      setIsAnalyzing(false);
    }
  };


  return (
    <div className={`subtitle-item ${isActive ? 'active' : ''}`}>
      <span className="subtitle-time">{formatTime(startTime)}</span>
      <div className="subtitle-text-container">
        <div 
          className="subtitle-text"
        >
          {text.split(' ').map((word, index) => {
            const normalizedWord = word.toLowerCase().replace(/[.,!?"']/g, '');
            const wordStyle = analysis[normalizedWord] ? {
              backgroundColor: POS_COLORS[analysis[normalizedWord].tag as keyof typeof POS_COLORS]
            } : {};
            return (
              <span 
                key={index}
                className={` ${addedWords.has(word) ? 'added-word' : 'word'} `}
                style={wordStyle}
                onClick={() => { setcurrentWord(word); setShowTooltip(true)}}
                onMouseLeave={() => setTimeout(()=>{ setShowTooltip(false)}, 2000)}
              >
                {word}{' '}
              </span>
            );
          })}
        </div>
        {showTooltip && (
          <div className="tooltip">
            <button onClick={handleTranslate} disabled={isTranslating}>
              {isTranslating ? 'Translating...' : 'Translate'}
            </button>
            {translation && <div className="translation-result">{translation}</div>}
            <button onClick={handleAddToWordBook}>Add to Word Book</button>
            {analysis[currentWord] && (
              <div className="pos-info">
                {analysis[currentWord].type} ({analysis[currentWord].tag})
              </div>
            )}
            {successMessage && <div className="success-message">{successMessage}</div>}
          </div>
        )}
      </div>
      <button 
        className="play-button"
        onClick={() => onClick(startTime)}
        title="Play from this subtitle"
      >
        ▶
      </button>
      <button
        className="analyze-button"
        onClick={()=>handleAnalyze(text)}
        disabled={isAnalyzing}
        title="Analyze parts of speech"
      >
        {isAnalyzing ? '...' : '🔍'}
      </button>
    </div>
  );
};

export default SubtitleItem;