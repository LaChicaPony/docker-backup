﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DockerBackup.App.Initializers;

public static class QuartzDbInitializer
{
    public static void Initialize(DatabaseFacade db)
    {
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_FIRED_TRIGGERS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_PAUSED_TRIGGER_GRPS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_SCHEDULER_STATE;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_LOCKS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_SIMPROP_TRIGGERS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_SIMPLE_TRIGGERS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_CRON_TRIGGERS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_BLOB_TRIGGERS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_TRIGGERS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_JOB_DETAILS;");
        db.ExecuteSqlRaw("DROP TABLE IF EXISTS QRTZ_CALENDARS;");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_JOB_DETAILS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tJOB_NAME NVARCHAR(150) NOT NULL,\r\n    JOB_GROUP NVARCHAR(150) NOT NULL,\r\n    DESCRIPTION NVARCHAR(250) NULL,\r\n    JOB_CLASS_NAME   NVARCHAR(250) NOT NULL,\r\n    IS_DURABLE BIT NOT NULL,\r\n    IS_NONCONCURRENT BIT NOT NULL,\r\n    IS_UPDATE_DATA BIT  NOT NULL,\r\n\tREQUESTS_RECOVERY BIT NOT NULL,\r\n    JOB_DATA BLOB NULL,\r\n    PRIMARY KEY (SCHED_NAME,JOB_NAME,JOB_GROUP)\r\n);\r\n");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_TRIGGERS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tTRIGGER_NAME NVARCHAR(150) NOT NULL,\r\n    TRIGGER_GROUP NVARCHAR(150) NOT NULL,\r\n    JOB_NAME NVARCHAR(150) NOT NULL,\r\n    JOB_GROUP NVARCHAR(150) NOT NULL,\r\n    DESCRIPTION NVARCHAR(250) NULL,\r\n    NEXT_FIRE_TIME BIGINT NULL,\r\n    PREV_FIRE_TIME BIGINT NULL,\r\n    PRIORITY INTEGER NULL,\r\n    TRIGGER_STATE NVARCHAR(16) NOT NULL,\r\n    TRIGGER_TYPE NVARCHAR(8) NOT NULL,\r\n    START_TIME BIGINT NOT NULL,\r\n    END_TIME BIGINT NULL,\r\n    CALENDAR_NAME NVARCHAR(200) NULL,\r\n    MISFIRE_INSTR INTEGER NULL,\r\n    JOB_DATA BLOB NULL,\r\n    PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),\r\n    FOREIGN KEY (SCHED_NAME,JOB_NAME,JOB_GROUP)\r\n        REFERENCES QRTZ_JOB_DETAILS(SCHED_NAME,JOB_NAME,JOB_GROUP)\r\n);");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_SIMPLE_TRIGGERS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tTRIGGER_NAME NVARCHAR(150) NOT NULL,\r\n    TRIGGER_GROUP NVARCHAR(150) NOT NULL,\r\n    REPEAT_COUNT BIGINT NOT NULL,\r\n    REPEAT_INTERVAL BIGINT NOT NULL,\r\n    TIMES_TRIGGERED BIGINT NOT NULL,\r\n    PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),\r\n    FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)\r\n        REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP) ON DELETE CASCADE\r\n);");
        db.ExecuteSqlRaw("CREATE TRIGGER DELETE_SIMPLE_TRIGGER DELETE ON QRTZ_TRIGGERS\r\nBEGIN\r\n\tDELETE FROM QRTZ_SIMPLE_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP;\r\nEND\r\n;");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_SIMPROP_TRIGGERS \r\n  (\r\n    SCHED_NAME NVARCHAR (120) NOT NULL ,\r\n    TRIGGER_NAME NVARCHAR (150) NOT NULL ,\r\n    TRIGGER_GROUP NVARCHAR (150) NOT NULL ,\r\n    STR_PROP_1 NVARCHAR (512) NULL,\r\n    STR_PROP_2 NVARCHAR (512) NULL,\r\n    STR_PROP_3 NVARCHAR (512) NULL,\r\n    INT_PROP_1 INT NULL,\r\n    INT_PROP_2 INT NULL,\r\n    LONG_PROP_1 BIGINT NULL,\r\n    LONG_PROP_2 BIGINT NULL,\r\n    DEC_PROP_1 NUMERIC NULL,\r\n    DEC_PROP_2 NUMERIC NULL,\r\n    BOOL_PROP_1 BIT NULL,\r\n    BOOL_PROP_2 BIT NULL,\r\n    TIME_ZONE_ID NVARCHAR(80) NULL,\r\n\tPRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),\r\n\tFOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)\r\n        REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP) ON DELETE CASCADE\r\n);");
        db.ExecuteSqlRaw("CREATE TRIGGER DELETE_SIMPROP_TRIGGER DELETE ON QRTZ_TRIGGERS\r\nBEGIN\r\n\tDELETE FROM QRTZ_SIMPROP_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP;\r\nEND\r\n;");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_CRON_TRIGGERS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tTRIGGER_NAME NVARCHAR(150) NOT NULL,\r\n    TRIGGER_GROUP NVARCHAR(150) NOT NULL,\r\n    CRON_EXPRESSION NVARCHAR(250) NOT NULL,\r\n    TIME_ZONE_ID NVARCHAR(80),\r\n    PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),\r\n    FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)\r\n        REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP) ON DELETE CASCADE\r\n);");
        db.ExecuteSqlRaw("CREATE TRIGGER DELETE_CRON_TRIGGER DELETE ON QRTZ_TRIGGERS\r\nBEGIN\r\n\tDELETE FROM QRTZ_CRON_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP;\r\nEND\r\n;");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_BLOB_TRIGGERS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tTRIGGER_NAME NVARCHAR(150) NOT NULL,\r\n    TRIGGER_GROUP NVARCHAR(150) NOT NULL,\r\n    BLOB_DATA BLOB NULL,\r\n    PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),\r\n    FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)\r\n        REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP) ON DELETE CASCADE\r\n);");
        db.ExecuteSqlRaw("CREATE TRIGGER DELETE_BLOB_TRIGGER DELETE ON QRTZ_TRIGGERS\r\nBEGIN\r\n\tDELETE FROM QRTZ_BLOB_TRIGGERS WHERE SCHED_NAME=OLD.SCHED_NAME AND TRIGGER_NAME=OLD.TRIGGER_NAME AND TRIGGER_GROUP=OLD.TRIGGER_GROUP;\r\nEND\r\n;");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_CALENDARS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tCALENDAR_NAME  NVARCHAR(200) NOT NULL,\r\n    CALENDAR BLOB NOT NULL,\r\n    PRIMARY KEY (SCHED_NAME,CALENDAR_NAME)\r\n);");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_PAUSED_TRIGGER_GRPS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tTRIGGER_GROUP NVARCHAR(150) NOT NULL, \r\n    PRIMARY KEY (SCHED_NAME,TRIGGER_GROUP)\r\n);");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_FIRED_TRIGGERS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tENTRY_ID NVARCHAR(140) NOT NULL,\r\n    TRIGGER_NAME NVARCHAR(150) NOT NULL,\r\n    TRIGGER_GROUP NVARCHAR(150) NOT NULL,\r\n    INSTANCE_NAME NVARCHAR(200) NOT NULL,\r\n    FIRED_TIME BIGINT NOT NULL,\r\n    SCHED_TIME BIGINT NOT NULL,\r\n\tPRIORITY INTEGER NOT NULL,\r\n    STATE NVARCHAR(16) NOT NULL,\r\n    JOB_NAME NVARCHAR(150) NULL,\r\n    JOB_GROUP NVARCHAR(150) NULL,\r\n    IS_NONCONCURRENT BIT NULL,\r\n    REQUESTS_RECOVERY BIT NULL,\r\n    PRIMARY KEY (SCHED_NAME,ENTRY_ID)\r\n);");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_SCHEDULER_STATE\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tINSTANCE_NAME NVARCHAR(200) NOT NULL,\r\n    LAST_CHECKIN_TIME BIGINT NOT NULL,\r\n    CHECKIN_INTERVAL BIGINT NOT NULL,\r\n    PRIMARY KEY (SCHED_NAME,INSTANCE_NAME)\r\n);");
        db.ExecuteSqlRaw("CREATE TABLE QRTZ_LOCKS\r\n  (\r\n    SCHED_NAME NVARCHAR(120) NOT NULL,\r\n\tLOCK_NAME  NVARCHAR(40) NOT NULL, \r\n    PRIMARY KEY (SCHED_NAME,LOCK_NAME)\r\n);");
    }
}