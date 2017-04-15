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
    public $mysql;
    public $config;

    function __construct($server_id = null) {
        // require config
        require(__DIR__ . "/config.api.php");
        $this->config = $config;

        // set up mysql connection
        $conn = $config["database"];
        self::connect_database($conn["host"],$conn["username"],$conn["password"],$conn["database"]);

        // process and return request
        if ($server_id != null) {
            $request = self::return_server_status($server_id);
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
     * Return Server Status
     *
     * This function queries the database by filtering results based on the `server_id` identifier passed by `$_GET[livemap]`,
     * and returns a combined array of all associated livemap tables [`livemap_servers`,`livemap_data`,`livemap_chat`]
     * @param string $id        ServerID associated with table column `server_id`
     * @return array $result    Returned array of table result queries combining `livemap_servers`, `livemap_data`, and `livemap_chat`
     */
    public function return_server_status($server_id) {
        $livemap_servers = $this->config["database"]["tables"]["livemap_servers"];
        $livemap_data = $this->config["database"]["tables"]["livemap_data"];
        $livemap_chat = $this->config["database"]["tables"]["livemap_chat"];

        // sterilize input and query server status
        $query_server_status = $this->mysql->prepare("SELECT * FROM $livemap_servers WHERE server_id = :server_id");
        $query_server_status->bindValue(':server_id', $server_id, PDO::PARAM_STR);
        $query_server_status->execute();
        $server_status = $query_server_status->fetch(PDO::FETCH_ASSOC);

        // if server status is not empty, generate a result
        if ($server_status != false) {
            // query online players
            $query_player_data = $this->mysql->query("SELECT * FROM $livemap_data WHERE last_refresh <> last_disconnect");
            $player_data = $query_player_data->fetchAll(PDO::FETCH_ASSOC);

            // query last 5 minutes of world chat
            $query_world_chat = $this->mysql->query("SELECT * FROM $livemap_chat WHERE timestamp > DATE_SUB(NOW(), INTERVAL 5 MINUTE)");
            $world_chat = $query_world_chat->fetchAll(PDO::FETCH_ASSOC);

            // combine the arrays
            $result["status"] = $server_status;
            $result["players"] = $player_data;
            $result["world_chat"] = $world_chat;
        }

        // return result
        return $result;
    }
}

/**
 * Initiate API
 * this block enforces a `server_id` be passed to initiate the API or else
 * it will return a http response code of 400 (Bad Request)
 */
if (isset($_GET["livemap"])) {
    $api = new API($_GET["livemap"]);
} else {
    http_response_code(400); // bad request
}
?>