# Task Embedding

## Description

使用 Ollama 的mxbai-embed-large模型将英文字幕转换为向量，存储至 PostgreSQL+pgvector，支持 “语义相似性搜索”（如查找与 “家庭责任” 主题相关的所有台词）；

## Table

CREATE TABLE public."episodes_embeddings" (
    "id" SERIAL PRIMARY KEY,
    "episodes_id" VARCHAR(20) NOT NULL,
    "content" TEXT,
    "embedding" VECTOR,
    CONSTRAINT episodes_embeddings_episodes_id_fkey FOREIGN KEY (episodes_id) 
        REFERENCES public.episodes(id)
);

name:
episodes_embeddings

columns:
- episode_id
- embedding

## API

- /embeddings - POST
- /embeddings - GET

## Implementation

- 实现 embedding 存储
    1. 从EPISODES表中读取所有的字幕, 每一行字幕作为一个字符串
    2. 调用ollama的mxbai-embed-large模型，将字幕转换为向量
    3. 将向量存储至episodes_embeddings表中

- 通过关键字实现 embedding 搜索
    1. 从episodes_embeddings表中读取所有的向量

REF:
