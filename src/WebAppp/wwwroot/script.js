let currentSubtitles = [];
let currentEpisode = 's7e01';

async function loadEpisodes() {
    try {
        const response = await fetch('/api/media/episodes');
        if (!response.ok) throw new Error('Failed to load episodes');
        const episodes = await response.json();
        
        const select = document.getElementById('episode-select');
        select.innerHTML = episodes.map(episode => 
            `<option value="${episode}" ${episode === currentEpisode ? 'selected' : ''}>
                Episode ${episode.replace('s', 'Season ').replace('e', ' Episode ')}
            </option>`
        ).join('');
        
    } catch (error) {
        console.error('Error loading episodes:', error);
    }
}

async function loadSubtitles() {
    try {
        const response = await fetch(`/api/media/subtitles/${currentEpisode}`);
        if (!response.ok) throw new Error('Failed to load subtitles');
        currentSubtitles = await response.json();
        displayAllSubtitles();
    } catch (error) {
        console.error('Error loading subtitles:', error);
    }
}

function formatTime(timeString) {
    const [hours, minutes, seconds] = timeString.split(':');
    return `${hours}:${minutes}:${seconds.split('.')[0]}`;
}

function timeToSeconds(timeString) {
    const [hours, minutes, seconds] = timeString.split(':').map(Number);
    return hours * 3600 + minutes * 60 + seconds;
}

function displayAllSubtitles() {
    const allSubtitlesDiv = document.getElementById('all-subtitles');
    allSubtitlesDiv.innerHTML = currentSubtitles.map(sub => `
        <div class="subtitle-line" data-start="${sub.startTime}" data-end="${sub.endTime}">
            <span class="subtitle-time">${formatTime(sub.startTime)}</span>
            <span class="subtitle-text">${sub.text}</span>
        </div>
    `).join('');

    document.querySelectorAll('.subtitle-time').forEach(line => {
        line.addEventListener('click', (e) => {
            const parentLine = e.target.parentElement;
            const startTime = timeToSeconds(parentLine.dataset.start);
            const audioPlayer = document.getElementById('audio-player');
            audioPlayer.currentTime = startTime;
            audioPlayer.play();
        });
    });
}

function updateSubtitleDisplay(currentTime) {
    const subtitle = currentSubtitles.find(sub => 
        currentTime >= timeToSeconds(sub.startTime) && 
        currentTime <= timeToSeconds(sub.endTime)
    );
    
    const subtitleDisplay = document.getElementById('subtitle-display');
    subtitleDisplay.textContent = subtitle ? subtitle.text : '';

    document.querySelectorAll('.subtitle-line').forEach(line => {
        const start = timeToSeconds(line.dataset.start);
        const end = timeToSeconds(line.dataset.end);
        const wasActive = line.classList.contains('active');
        const isActive = currentTime >= start && currentTime <= end;
        
        line.classList.toggle('active', isActive);
        
        if (!wasActive && isActive) {
            line.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    });
}

async function changeEpisode(episode) {
    currentEpisode = episode;
    const audioPlayer = document.getElementById('audio-player');
    
    audioPlayer.pause();
    audioPlayer.currentTime = 0;
    
    await loadSubtitles();
    audioPlayer.src = `/api/media/audio/${currentEpisode}`;
}

document.addEventListener('DOMContentLoaded', async () => {
    const audioPlayer = document.getElementById('audio-player');
    const episodeSelect = document.getElementById('episode-select');
    
    await loadEpisodes();
    await loadSubtitles();
    
    audioPlayer.src = `/api/media/audio/${currentEpisode}`;
    
    audioPlayer.addEventListener('timeupdate', () => {
        updateSubtitleDisplay(audioPlayer.currentTime);
    });
    
    episodeSelect.addEventListener('change', (e) => {
        changeEpisode(e.target.value);
    });
});