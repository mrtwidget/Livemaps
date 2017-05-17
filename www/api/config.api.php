<?php
// database connection 
$config["database"]["host"] = "localhost";
$config["database"]["username"] = "unturned";
$config["database"]["password"] = "password";
$config["database"]["database"] = "unturned";

// database tables
$config["database"]["tables"]["livemap_server"] = "livemap_server";
$config["database"]["tables"]["livemap_data"] = "livemap_data";
$config["database"]["tables"]["livemap_chat"] = "livemap_chat";

// database settings
$config["database"]["settings"]["livemap_chat_activity_duration"] = 15; // 5mins
?>