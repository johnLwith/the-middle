# Task Analyze

## Backend
后端将返回一个JSON格式的结果，包含每个单词的词性标签。

### API
https://localhost:7001/api/Episodes/tm11/analyze

```json
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
```

## Frontend
前端将接收后端返回的结果，并将各个页面的词性标签显示在页面上。
将词性分类，用户可在设置中开启 “语法高亮”，单词按词性显示不同颜色（名词 #FFEB3B、动词 #2196F3、形容词 #4CAF50），鼠标悬停显示详细词性标签及词干（如 “ran→run（动词）”）；

```txt
The API uses the following POS tags:

CC: Coordinating conjunction
CD: Cardinal number
DT: Determiner
EX: Existential there
FW: Foreign word
IN: Preposition/subordinating conjunction
JJ: Adjective
JJR: Adjective, comparative
JJS: Adjective, superlative
LS: List item marker
MD: Modal
NN: Noun, singular
NNS: Noun, plural
NNP: Proper noun, singular
NNPS: Proper noun, plural
PDT: Predeterminer
POS: Possessive ending
PRP: Personal pronoun
PRP$: Possessive pronoun
RB: Adverb
RBR: Adverb, comparative
RBS: Adverb, superlative
RP: Particle
TO: to
UH: Interjection
VB: Verb, base form
VBD: Verb, past tense
VBG: Verb, gerund/present participle
VBN: Verb, past participle
VBP: Verb, non-3rd person singular present
VBZ: Verb, 3rd person singular present
WDT: Wh-determiner
WP: Wh-pronoun
WP$: Possessive wh-pronoun
WRB: Wh-adverb
```