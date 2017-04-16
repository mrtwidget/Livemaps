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
        // require config
        require(__DIR__ . "/config.api.php");
        $this->config = $config;

        // set up mysql connection
        $conn = $config["database"];
        self::connect_database($conn["host"],$conn["username"],$conn["password"],$conn["database"]);

        // process and return request
        if ($server_id != null) {
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
            die("Fatal Error! Could not connect to the database. Check your config.api.php settings and try again.");
        }
    }

    /**
     * Process Request
     *
     * This function queries the database by filtering results based on the `server_id` identifier passed by `$_GET[livemap]`,
     * and returns a combined array of all associated livemap tables [`livemap_server`,`livemap_data`,`livemap_chat`]
     * @param string $id        ServerID associated with table column `server_id`
     * @return array $result    Returned array of table result queries combining `livemap_server`, `livemap_data`, and `livemap_chat`
     */
    public function process_request($server_id, $filter = null) {
        // tables
        $livemap_server = $this->config["database"]["tables"]["livemap_server"];
        $livemap_data = $this->config["database"]["tables"]["livemap_data"];
        $livemap_chat = $this->config["database"]["tables"]["livemap_chat"];

        // settings
        $livemap_chat_activity_duration = $this->config["database"]["settings"]["livemap_chat_activity_duration"] ;

        // queries
        $livemap_server_query = "SELECT * FROM $livemap_server WHERE server_id = :server_id";
        $livemap_data_query = "SELECT * FROM $livemap_data WHERE (last_refresh > last_disconnect OR last_disconnect IS NULL) AND last_refresh > DATE_SUB(NOW(), INTERVAL 30 SECOND) AND server_id = :server_id";
        $livemap_chat_query = "SELECT * FROM $livemap_chat WHERE timestamp > DATE_SUB(NOW(), INTERVAL $livemap_chat_activity_duration MINUTE) AND server_id = :server_id";

        // loop through each table
        foreach ($this->config["database"]["tables"] as $table) {
            // filter results if $filter is set
            if ($filter == null || $filter == $table) {
                $query = $this->mysql->prepare(${$table . "_query"});
                $query->bindValue(':server_id', $server_id, PDO::PARAM_STR);
                $query->execute();
                $result[$table] = $query->fetchAll(PDO::FETCH_ASSOC);
            }
        }
        return $result;
    }
}

/**
 * Initiate API
 * this block enforces a `server_id` be passed to initiate the API or else
 * it will return a http response code of 400 (Bad Request)
 */
if (isset($_GET["livemap"])) {
    $api = new API($_GET["livemap"], (isset($_GET["filter"]) ? $_GET["filter"] : null));
} else {
    http_response_code(400); // bad request
}
?>