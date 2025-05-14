import React, { useEffect, useState } from 'react';
import { api, Episode } from '../services/api';
import EpisodeComponent from './Episode';
import SearchBar from './SearchBar';
import './EpisodeList.css';

const EpisodeList: React.FC = () => {
  const [episodes, setEpisodes] = useState<Episode[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchEpisodes = async () => {
      try {
        const data = await api.episodes.getAll();
        setEpisodes(data);
        setLoading(false);
      } catch (err) {
        setError('Failed to fetch episodes');
        setLoading(false);
      }
    };

    fetchEpisodes();
  }, []);

  if (loading) {
    return <div>Loading episodes...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <>
    <SearchBar />
    <div className="episode-list">
      {episodes.map((episode) => (
        <EpisodeComponent
          key={episode.id}
          id={episode.id!}
          title={episode.title || `Episode ${episode.episodeNumber}`}
          description={episode.description}
          season={episode.seasonNumber}
          episodeNumber={episode.episodeNumber}
          airDate="N/A" // You might want to add this to your backend model
        />
      ))}
    </div>
    </>

  );
};

export default EpisodeList; 