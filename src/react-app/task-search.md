# Task Search

### API
https://localhost:7001/api/Search/Embeddings?query={query}   - 语义搜索
https://localhost:7001/api/Search/Content?query={query}      - 精确搜索

```json
[
  {
    "episodeId": "string",
    "title": "string",
    "content": "string",
    "score": 0
  } 
]
```

## Frontend
在首页添加一个搜索框, 两个搜索按钮(语义，精确)，用户输入关键词后，跳转到一个新的页面，显示搜索结果。