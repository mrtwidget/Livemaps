ALTER TABLE livemap_server 
ADD COLUMN enable_livemap_status TINYINT(1) DEFAULT 1 AFTER refresh_interval,
ADD COLUMN enable_livemap TINYINT(1) DEFAULT 1 AFTER refresh_interval;