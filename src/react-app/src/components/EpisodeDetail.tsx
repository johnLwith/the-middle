import React, { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api, Episode, Subtitle } from '../services/api';
import SubtitleItem from './SubtitleItem';
import './EpisodeDetail.css';

const POS_COLORS = {
  NOUN: '#4CAF50',
  VERB: '#2196F3',
  ADJ: '#FF9800',
  ADV: '#9C27B0',
  PRON: '#607D8B',
  DET: '#795548',
  ADP: '#009688',
  CONJ: '#673AB7',
  NUM: '#E91E63',
  PRT: '#00BCD4',
  X: '#9E9E9E',
};

const EpisodeDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const audioRef = useRef<HTMLAudioElement>(null);
  const [episode, setEpisode] = useState<Episode | null>(null);
  const [subtitles, setSubtitles] = useState<Subtitle[]>([]);
  const [currentSubtitle, setCurrentSubtitle] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [showAnalysis, setShowAnalysis] = useState(false);
  const [analysisData, setAnalysisData] = useState<any[]>([]);

  useEffect(() => {
    const fetchEpisodeData = async () => {
      try {
        const episodeData = await api.episodes.getById(id!);
        setEpisode(episodeData);
        
        const subtitleData = await api.episodes.getSubtitles(id!);
        const analysisData = await api.episodes.analyzeSubtitle(id!);
        
        const subtitlesWithAnalysis = subtitleData.map(subtitle => ({
          startTime: subtitle.startTime,
          endTime: subtitle.endTime,
          text: subtitle.text
        }));
        
        setSubtitles(subtitlesWithAnalysis);
        setAnalysisData(analysisData.analysis || []);
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
          <button 
            onClick={() => setShowAnalysis(!showAnalysis)} 
            className="analyze-toggle"
          >
            {showAnalysis ? 'Hide Analysis' : 'Show Analysis'}
          </button>
        </div>

        <div className="current-subtitle">
          {showAnalysis && currentSubtitle ? (
            <div className="analysis-container">
              <div className="subtitle-text">
                {currentSubtitle.split(' ').map((word, i) => {
                  const analysis = analysisData.find(item => item.word === word);
                  console.log('Analysis for word:', word, analysis);
                  const color = analysis ? POS_COLORS[analysis.tag as keyof typeof POS_COLORS] : 'inherit';
                  return (
                    <span 
                      key={i} 
                      style={{ color }}
                    >
                      {word}
                    </span>
                  );
                })}
              </div>
              <div className="analysis-details">
                {analysisData
                  .filter(item => currentSubtitle.includes(item.word))
                  .map((item, i) => (
                    <div key={i} className="analysis-item">
                      <div className="analysis-label">{item.word}:</div>
                      <div className="analysis-value">{item.type} ({item.tag})</div>
                    </div>
                  ))
                }
              </div>
            </div>
          ) : currentSubtitle}
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