import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Episode.css';

interface EpisodeProps {
  id: string;
  title: string;
  description: string;
  season: number;
  episodeNumber: number;
  airDate: string;
  imageUrl?: string;
}

const Episode: React.FC<EpisodeProps> = ({
  id,
  title,
  description,
  season,
  episodeNumber,
  airDate,
  imageUrl
}) => {
  const navigate = useNavigate();

  const handleClick = () => {
    navigate(`/episode/${id}`);
  };

  return (
    <div className="episode-card" onClick={handleClick}>
      {imageUrl && (
        <div className="episode-image">
          <img src={imageUrl} alt={title} />
        </div>
      )}
      <div className="episode-content">
        <h3 className="episode-title">{title}</h3>
        <div className="episode-meta">
          <span>Season {season}</span>
          <span>Episode {episodeNumber}</span>
          <span>{airDate}</span>
        </div>
        <p className="episode-description">{description}</p>
      </div>
    </div>
  );
};

export default Episode; 