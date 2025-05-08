# Task NLP

## Description
需要添加一个API, 将subtitles发送给API service， 然后该服务返回Part of Speech Tags

# API service spec
Analyze Sentence
POST /analyze
Analyzes the provided text and returns part-of-speech information for each word
Request body:
{
    "text": "Your sentence here"
}
Response example:
{
    "text": "The quick brown fox jumps over the lazy dog.",
    "analysis": [
        {
            "word": "The",
            "tag": "DT",
            "type": "Determiner"
        },
        {
            "word": "quick",
            "tag": "JJ",
            "type": "Adjective"
        },
        // ... more words
    ]
}

## REF
https://github.com/johnLwith/py-nlp-test/blob/main/README.md