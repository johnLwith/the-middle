import React, { useState } from 'react';
import { api } from '../services/api';
import './SubtitleItem.css';

interface SubtitleItemProps {
  startTime: string;
  endTime: string;
  text: string;
  isActive?: boolean;
  onClick: (startTime: string) => void;
}

const SubtitleItem: React.FC<SubtitleItemProps> = ({
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

  const handleAddToWordBook = () => {
    // TODO: Implement add to word book functionality
    console.log('Add to word book:', currentWord);
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
              className="word"
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