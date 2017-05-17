<?php
/**
 * Livemap API
 *
 * This API queries a mysql database containing associated livemap tables and returns a
 * JSON encoded result based on the `server_id` value passed by `$_GET[livemap]`
 * @author Nexis <mrtwidget@gmail.com>
 * @package Livemap/API
 * @version 0.2.0
 */
class API {
    /* MySQL Connection */
    public $mysql;

    /* Configuration Settings */
    public $config;

    function __construct($server_id = null, $filter = null) {
        // require config.api.php
        $config_filename = __DIR__ . "/config.api.php";
        if (file_exists($config_filename)) {
            require($config_filename);
            $this->config = $config;
        } else {
            die("FATAL ERROR: No configurtion file was found!");
        }

        // if `server_id` and `filter` are empty
        if ($server_id == null && $filter == null) {
            // return 405 Method Not Allowed
            http_response_code(405);
        } else {
            // set up mysql connection
            $conn = $config["database"];
            self::connect_database($conn["host"],$conn["username"],$conn["password"],$conn["database"]);

            // process and return encoded request
            $request = self::process_request($server_id, $filter);
            echo json_encode($request);
        }
    }

    /**
     * Connect to MySQL Database
     *
     * This function connects to the configured MySQL database via `PDO`. Database connection credentials are 
     * configured in the `config.api.php` file.
     * @param string $host          Database host address
     * @param string $username      Database username
     * @param string $password      Database password
     * @param string $database      Database name
     * @uses object $this->mysql    MySQL database connection
     */
    public function connect_database($host, $username, $password, $database) {
        try {
            $DSN = 'mysql:host=' . $host . ';dbname=' . $database;
            $this->mysql = new PDO( $DSN, $username, $password );
            $this->mysql->setAttribute( PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION ); 
        } catch(PDOException $ex) {
            die("FATAL ERROR: Could not connect to the database! Check your config.api.php settings and then reload this page.");
        }
    }

    /**
     * Process API Request
     *
     * This function queries the database by filtering results based on the `server_id` identifier passed by `$_GET[livemap]`,
     * and returns a combined array of all associated livemap tables [`livemap_server`,`livemap_data`,`livemap_chat`]
     * @param string $server_id     ServerID associated with table column `server_id`
     * @param string $filter        Request filter to return specific data
     * @return array $result        Returned array of table result queries combining `livemap_server`, `livemap_data`, and `livemap_chat`
     */
    public function process_request($server_id = null, $filter = null) {
        // database tables
        $livemap_server = $this->config["database"]["tables"]["livemap_server"];
        $livemap_data = $this->config["database"]["tables"]["livemap_data"];
        $livemap_chat = $this->config["database"]["tables"]["livemap_chat"];

        // database settings
        $livemap_chat_activity_duration = $this->config["database"]["settings"]["livemap_chat_activity_duration"] ;

        // database queries
        $livemap_init_query = "SELECT * FROM $livemap_server WHERE last_refresh > DATE_SUB(NOW(), INTERVAL 30 SECOND)";
        $livemap_server_query = "SELECT * FROM $livemap_server WHERE last_refresh > DATE_SUB(NOW(), INTERVAL 30 SECOND) AND server_id = :server_id";
        $livemap_data_query = "SELECT * FROM $livemap_data WHERE (last_refresh > last_disconnect OR last_disconnect IS NULL) AND last_refresh > DATE_SUB(NOW(), INTERVAL 30 SECOND) AND server_id = :server_id";
        $livemap_chat_query = "SELECT livemap_chat.server_id, livemap_chat.steam_id, livemap_chat.message, livemap_chat.timestamp, livemap_data.character_name, livemap_data.steam_avatar_medium, livemap_data.is_admin FROM $livemap_chat INNER JOIN livemap_data ON livemap_chat.steam_id = livemap_data.CSteamID WHERE timestamp > DATE_SUB(NOW(), INTERVAL $livemap_chat_activity_duration MINUTE) AND livemap_chat.server_id = :server_id ORDER BY timestamp DESC";

        // init() load query
        if ($server_id == null && $filter == $livemap_server) {
            $query = $this->mysql->query($livemap_init_query);
            return $query->fetchAll(PDO::FETCH_ASSOC);
        } else {
            // loop through each table and collect data matching passed `server_id`
            foreach ($this->config["database"]["tables"] as $table) {            
                // filter results if $filter is set
                if ($filter == null || $filter == $table) {
                    // input sanitization
                    $query = $this->mysql->prepare(${$table . "_query"});
                    $query->bindValue(':server_id', $server_id, PDO::PARAM_STR);
                    $query->execute();

                    // if result is empty return null
                    if ($query->rowCount() > 0) {
                        $result[$table] = $query->fetchAll(PDO::FETCH_ASSOC);
                    } else {
                        $result[$table] = null;
                    }
                }
            }
            // if `livemap_server` is empty return null
            return isset($result[$livemap_server]) ? $result : null;
        }
    }
}

/* GET Request */
$livemap = !empty($_GET["livemap"]) ? $_GET["livemap"] : null;
$filter = !empty($_GET["filter"]) ? $_GET["filter"] : null;

/* Initiate API */
$api = new API($livemap, $filter);
?>