import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import './App.css';
import EpisodeList from './components/EpisodeList';
import EpisodeDetail from './components/EpisodeDetail';
import SearchResults from './components/SearchResults';

function App() {
  return (
    <Router>
      <div className="App">
        <main>
          
          <Routes>
            <Route path="/" element={<EpisodeList />} />
            <Route path="/episode/:id" element={<EpisodeDetail />} />
            <Route path="/search" element={<SearchResults />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
