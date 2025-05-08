CREATE TABLE public.episodes (
	id varchar(20) NOT NULL,
	season_number int4 NOT NULL,
	episode_number int4 NOT NULL,
	title varchar(255) NULL,
	description text NOT NULL,
	subtitle_path varchar(255) NULL,
	audio_path varchar(255) NULL,
	CONSTRAINT episodes_pk PRIMARY KEY (id)
);

CREATE TABLE public."episodes_embeddings" (
    "id" SERIAL PRIMARY KEY,
    "episodes_id" VARCHAR(20) NOT NULL,
    "content" TEXT,
    "embedding" VECTOR,
    CONSTRAINT episodes_embeddings_episodes_id_fkey FOREIGN KEY (episodes_id) 
        REFERENCES public.episodes(id)
);

CREATE TABLE public.words (
	id serial4 NOT NULL,
	word varchar(255) NOT NULL,
	episode_id varchar(20) NOT NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
	CONSTRAINT words_pkey PRIMARY KEY (id)
);


-- public.words foreign keys

ALTER TABLE public.words ADD CONSTRAINT fk_words_episodes FOREIGN KEY (episode_id) REFERENCES public.episodes(id) ON DELETE CASCADE ON UPDATE CASCADE;