import React, { useState } from 'react';
import { api } from '../services/api';
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

  return (
    <div className={`subtitle-item ${isActive ? 'active' : ''}`}>
      <span className="subtitle-time">{formatTime(startTime)}</span>
      <div className="subtitle-text-container">
        <div 
          className="subtitle-text"
        >
          {text.split(' ').map((word, index) => (
            <span 
              key={index}
              className={` ${addedWords.has(word) ? 'added-word' : 'word'} ` }
              onClick={() => { setcurrentWord(word); setShowTooltip(true)}}
              onMouseLeave={() => setTimeout(()=>{ setShowTooltip(false)}, 2000)}
            >
              {word}{' '}
            </span>
          ))}
        </div>
        {showTooltip && (
          <div className="tooltip">
            <button onClick={handleTranslate} disabled={isTranslating}>
              {isTranslating ? 'Translating...' : 'Translate'}
            </button>
            {translation && <div className="translation-result">{translation}</div>}
            <button onClick={handleAddToWordBook}>Add to Word Book</button>
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
    </div>
  );
};

export default SubtitleItem;