CREATE TABLE public.episodes (
	id varchar(20) NULL,
	season_number int4 NOT NULL,
	episode_number int4 NOT NULL,
	title varchar(255) NULL,
	description text NOT NULL,
	subtitle_path varchar(255) NULL,
	audio_path varchar(255) NULL
);