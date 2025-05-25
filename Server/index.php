<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *'); // Allow CORS for testing
header('Access-Control-Allow-Methods: GET, POST');
header('Access-Control-Allow-Headers: Content-Type, Authorization');

// Get the request method and path
$method = $_SERVER['REQUEST_METHOD'];
$path = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);

// Route requests
switch ($path) {
    case '/api/get-data':
        if ($method === 'GET') {
            $response = [
                'status' => 'success',
                'data' => [
                    'id' => 1,
                    'name' => 'Sample Data',
                    'timestamp' => date('c')
                ]
            ];
            echo json_encode($response);
        } else {
            http_response_code(405);
            echo json_encode(['error' => 'Method not allowed']);
        }
        break;

    case '/api/post-data':
        if ($method === 'POST') {
            $input = json_decode(file_get_contents('php://input'), true);
            if (json_last_error() !== JSON_ERROR_NONE) {
                http_response_code(400);
                echo json_encode(['error' => 'Invalid JSON']);
                return;
            }
            if (!isset($input['name']) || !isset($input['value'])) {
                http_response_code(400);
                echo json_encode(['error' => 'Missing name or value']);
                return;
            }
            $response = [
                'status' => 'success',
                'received' => $input,
                'message' => 'Data processed successfully'
            ];
            echo json_encode($response);
        } else {
            http_response_code(405);
            echo json_encode(['error' => 'Method not allowed']);
        }
        break;

    case '/':
    case '/index.php':
        if ($method === 'GET') {
            echo json_encode(['message' => 'Welcome to the LimonadoEnt API']);
        } else {
            http_response_code(405);
            echo json_encode(['error' => 'Method not allowed']);
        }
        break;

    default:
        http_response_code(404);
        echo json_encode(['error' => 'Endpoint not found']);
        break;
}
?>