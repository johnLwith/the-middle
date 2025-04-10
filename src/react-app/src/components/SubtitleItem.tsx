import React from 'react';
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

  return (
    <div
      className={`subtitle-item ${isActive ? 'active' : ''}`}
      onClick={() => onClick(startTime)}
    >
      <span className="subtitle-time">{formatTime(startTime)}</span>
      <span className="subtitle-text">{text}</span>
    </div>
  );
};

export default SubtitleItem; 