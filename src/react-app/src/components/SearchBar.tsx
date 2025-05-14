import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './SearchBar.css';

const SearchBar: React.FC = () => {
  const [query, setQuery] = useState('');
  const navigate = useNavigate();

  const handleSearch = (type: 'semantic' | 'exact') => {
    if (query.trim()) {
      navigate(`/search?q=${encodeURIComponent(query)}&type=${type}`);
    }
  };

  return (
    <div className="search-bar">
      <input
        type="text"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Enter keywords to search..."
        className="search-input"
      />
      <div className="search-buttons">
        <button
          onClick={() => handleSearch('semantic')}
          className="search-button semantic"
        >
          Semantic Search
        </button>
        <button
          onClick={() => handleSearch('exact')}
          className="search-button exact"
        >
          Exact Search
        </button>
      </div>
    </div>
  );
};

export default SearchBar;