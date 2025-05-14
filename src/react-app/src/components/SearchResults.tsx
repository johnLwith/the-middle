import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { api } from '../services/api';
import SearchBar from './SearchBar';
import './SearchResults.css';

interface SearchResult {
  episodeId: string;
  title: string;
  content: string;
  score: number;
}

const SearchResults: React.FC = () => {
  const [searchParams] = useSearchParams();
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const query = searchParams.get('q');
    const type = searchParams.get('type');

    if (!query || !type) {
      setError('Invalid search parameters');
      setLoading(false);
      return;
    }

    const fetchResults = async () => {
      try {
        const response = await fetch(
          `http://localhost:5001/api/Search/${type === 'semantic' ? 'embeddings' : 'content'}?query=${encodeURIComponent(query)}`
        );
        const data = await response.json();
        setResults(data);
      } catch (err) {
        setError('Failed to fetch search results');
      } finally {
        setLoading(false);
      }
    };

    fetchResults();
  }, [searchParams]);

  if (loading) {
    return <div className="search-results-loading">Loading...</div>;
  }

  if (error) {
    return <div className="search-results-error">{error}</div>;
  }

  if (results.length === 0) {
    return <div className="search-results-empty">No results found</div>;
  }

  return (
    <>
    <SearchBar />
    <div className="search-results">
      <h2>Search Results</h2>
      <div className="results-list">
        {results.map((result) => (
          <div key={result.episodeId} className="result-item">
            <h3><a href={"/episode/"+result.episodeId}>{result.title}</a></h3>
            <p>{result.content}</p>
            <div className="result-score">
              Relevance: {((1 - result.score) * 100).toFixed(2)}%
            </div>
          </div>
        ))}
      </div>
    </div>
    </>
  );
};

export default SearchResults;