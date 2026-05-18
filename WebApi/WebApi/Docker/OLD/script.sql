CREATE TABLE public.usertask (
	id int GENERATED ALWAYS AS IDENTITY NOT NULL,
	title varchar NOT NULL,
	description varchar NULL,
	status varchar NULL,
	dtcreate date NULL,
	dtupdate date NULL,
	CONSTRAINT usertask_pk PRIMARY KEY (id)
);
