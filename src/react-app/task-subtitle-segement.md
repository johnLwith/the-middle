# Task - Subtitle Segment

## Backend
后端将返回一个JSON格式的结果

### API
/api/Episodes/segement?id={episodeId}

```json
  [
  {
    "segments": "1-20",
    "description": "Brick has trouble relating to his teacher with odd results."
  }
]
```

## Frontend

在EpisodeDetail.tsx添加一个按钮，点击后调用API， 然后根据返回结果更新UI。
segments为Subtitle的id，description为Subtitle的summary.