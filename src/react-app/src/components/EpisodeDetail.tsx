import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api, Episode, Subtitle } from '../services/api';
import SubtitleItem from './SubtitleItem';
import './EpisodeDetail.css';

const EpisodeDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const audioRef = useRef<HTMLAudioElement>(null);
  const [episode, setEpisode] = useState<Episode | null>(null);
  const [subtitles, setSubtitles] = useState<Subtitle[]>([]);
  const [currentSubtitle, setCurrentSubtitle] = useState<string>('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchEpisodeData = async () => {
      try {
        const episodeData = await api.episodes.getById(id!);
        setEpisode(episodeData);
        
        const subtitleData = await api.episodes.getSubtitles(id!);
        setSubtitles(subtitleData);
        
        setLoading(false);
      } catch (error) {
        console.error('Error fetching episode data:', error);
        setLoading(false);
      }
    };

    fetchEpisodeData();
  }, [id]);

  const formatTime = (timeString: string) => {
    const [hours, minutes, seconds] = timeString.split(':');
    return `${hours}:${minutes}:${seconds.split('.')[0]}`;
  };

  const timeToSeconds = (timeString: string) => {
    const [hours, minutes, seconds] = timeString.split(':').map(Number);
    return hours * 3600 + minutes * 60 + seconds;
  };

  const handleTimeUpdate = () => {
    if (!audioRef.current) return;

    const currentTime = audioRef.current.currentTime;
    const subtitle = subtitles.find(sub => 
      currentTime >= timeToSeconds(sub.startTime) && 
      currentTime <= timeToSeconds(sub.endTime)
    );
    
    setCurrentSubtitle(subtitle?.text || '');
  };

  const handleSubtitleClick = (startTime: string) => {
    if (!audioRef.current) return;
    audioRef.current.currentTime = timeToSeconds(startTime);
    audioRef.current.play();
  };

  if (loading) {
    return <div>Loading episode...</div>;
  }

  if (!episode) {
    return <div>Episode not found</div>;
  }

  if (!episode.audioPath) {
    return <div>Audio not available for this episode</div>;
  }

  return (
    <div className="episode-detail">
      <div className="container">
        <h1>{episode.title || `Episode ${episode.episodeNumber}`}</h1>
        
        <div className="player">
          <audio
            ref={audioRef}
            controls
            src={episode.audioPath}
            onTimeUpdate={handleTimeUpdate}
          >
            Your browser does not support the audio element.
          </audio>
        </div>

        <div className="current-subtitle">
          {currentSubtitle}
        </div>

        <div className="subtitles">
          <div className="all-subtitles">
            {subtitles.map((sub, index) => (
              <SubtitleItem
                key={index}
                startTime={sub.startTime}
                endTime={sub.endTime}
                text={sub.text}
                isActive={sub.text === currentSubtitle}
                onClick={handleSubtitleClick}
              />
            ))}
          </div>
        </div>

        <button onClick={() => navigate('/')} className="back-button">
          Back to Episodes
        </button>
      </div>
    </div>
  );
};

export default EpisodeDetail; 