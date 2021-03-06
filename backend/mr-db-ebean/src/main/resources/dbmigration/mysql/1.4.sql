-- apply changes
create table fh_app_strategy (
  id                            varchar(40) not null,
  fk_app_id                     varchar(40) not null,
  when_archived                 datetime(6),
  strategy_name                 varchar(255) not null,
  strategy                      json not null,
  fk_person_who_changed         varchar(40) not null,
  version                       bigint not null,
  when_updated                  datetime(6) not null,
  when_created                  datetime(6) not null,
  constraint idx_app_strategies unique (fk_app_id,strategy_name),
  constraint pk_fh_app_strategy primary key (id)
);

create table fh_strat_for_feature (
  id                            varchar(40) not null,
  fk_fv_id                      varchar(40) not null,
  fk_rs_id                      varchar(40) not null,
  enabled                       tinyint(1) default 0 not null,
  value                         longtext,
  constraint idx_feature_strat unique (fk_fv_id,fk_rs_id),
  constraint pk_fh_strat_for_feature primary key (id)
);

create index ix_fh_app_strategy_fk_app_id on fh_app_strategy (fk_app_id);
alter table fh_app_strategy add constraint fk_fh_app_strategy_fk_app_id foreign key (fk_app_id) references fh_application (id) on delete restrict on update restrict;

create index ix_fh_app_strategy_fk_person_who_changed on fh_app_strategy (fk_person_who_changed);
alter table fh_app_strategy add constraint fk_fh_app_strategy_fk_person_who_changed foreign key (fk_person_who_changed) references fh_person (id) on delete restrict on update restrict;

create index ix_fh_strat_for_feature_fk_fv_id on fh_strat_for_feature (fk_fv_id);
alter table fh_strat_for_feature add constraint fk_fh_strat_for_feature_fk_fv_id foreign key (fk_fv_id) references fh_env_feature_strategy (id) on delete restrict on update restrict;

create index ix_fh_strat_for_feature_fk_rs_id on fh_strat_for_feature (fk_rs_id);
alter table fh_strat_for_feature add constraint fk_fh_strat_for_feature_fk_rs_id foreign key (fk_rs_id) references fh_app_strategy (id) on delete restrict on update restrict;

