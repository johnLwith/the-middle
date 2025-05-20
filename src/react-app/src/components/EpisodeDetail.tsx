import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api, Episode, Subtitle } from '../services/api';
import SubtitleItem from './SubtitleItem';
import './EpisodeDetail.css';

interface Segment {
  segments: string;
  description: string;
}

const POS_COLORS = {
  NOUN: '#FFEB3B',  // Updated to match nlp.ts
  VERB: '#2196F3',
  ADJ: '#4CAF50'
};

const PosColorGuide: React.FC = () => (
  <div className="pos-color-samples">
    <h5>Parts of Speech Color Guide</h5>
    <div className="sample-items">
      {Object.entries(POS_COLORS).map(([pos, color]) => (
        <div key={pos} className="sample-item" style={{ backgroundColor: color }}>
          <span>{pos}</span>
        </div>
      ))}
    </div>
  </div>
);

const EpisodeDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const audioRef = useRef<HTMLAudioElement>(null);
  const [episode, setEpisode] = useState<Episode | null>(null);
  const [subtitles, setSubtitles] = useState<Subtitle[]>([]);
  const [currentSubtitle, setCurrentSubtitle] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [segments, setSegments] = useState<Segment[]>([]);
  const [loadingSegments, setLoadingSegments] = useState(false);
  const [groupedSubtitles, setGroupedSubtitles] = useState<{[key: string]: Subtitle[]}>({});


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
      <PosColorGuide />
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

        <div className="segment-section">
          <button 
            className="segment-button"
            onClick={async () => {
              try {
                setLoadingSegments(true);
                const segmentData = await api.episodes.getSegments(id!);
                setSegments(segmentData);
              } catch (error) {
                console.error('Error fetching segments:', error);
              } finally {
                setLoadingSegments(false);
              }
            }}
            disabled={loadingSegments}
          >
            {loadingSegments ? 'Loading Segments...' : 'Get Segments'}
          </button>

          {segments.length > 0 && (
            <div className="segments-list">
              {segments.map((segment, index) => {
                const [start, end] = segment.segments.split('-').map(Number);
                const segmentSubtitles = subtitles.slice(start - 1, end);
                return (
                  <div key={index} className="segment-item">
                    <div className="segment-header">
                      <div className="segment-range">Subtitles {segment.segments}</div>
                      <div className="segment-description">{segment.description}</div>
                    </div>
                    <div className="segment-subtitles">
                      {segmentSubtitles.map((sub, subIndex) => (
                        <SubtitleItem
                          key={subIndex}
                          episodeId={id!}
                          subtitleId={sub.id}
                          startTime={sub.startTime}
                          endTime={sub.endTime}
                          text={sub.text}
                          isActive={sub.text === currentSubtitle}
                          onClick={handleSubtitleClick}
                        />
                      ))}
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
        </div>

        <div className="subtitles">
          <div className="all-subtitles">
            {subtitles.map((sub, index) => (
              <SubtitleItem
                key={index}
                episodeId={id!}
                subtitleId={sub.id}
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
  );
};

export default EpisodeDetail;