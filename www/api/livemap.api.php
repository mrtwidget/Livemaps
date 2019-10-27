<?php
    $json = file_get_contents("php://input");
    $file = "data.json";
    file_put_contents($file,$json);
?>