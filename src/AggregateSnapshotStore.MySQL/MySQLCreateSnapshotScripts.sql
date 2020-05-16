CREATE TABLE `AggregateSnapshot` (
    `Sequence`              BIGINT(20)        NOT NULL AUTO_INCREMENT,
    `AggregateRootTypeName` NVARCHAR (256)    NOT NULL,
    `AggregateRootId`       NVARCHAR (36)     NOT NULL,
    `Version`               INT(11)           NOT NULL,
    `Data`                  BLOB(65535)		  NOT NULL,
    PRIMARY KEY(`Sequence`),
    UNIQUE KEY `ix_AggregateRootId` (`AggregateRootId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
