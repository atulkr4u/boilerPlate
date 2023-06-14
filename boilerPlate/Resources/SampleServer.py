import json
from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import urlparse, parse_qs

attempt_count = 0


class SimpleHTTPRequestHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        global attempt_count
        parsed_url = urlparse(self.path)
        query_params = parse_qs(parsed_url.query)
        name = query_params.get('name', [''])[0]

        if attempt_count < 3:
            attempt_count += 1
            self.send_error(500, "Server Error")
        else:
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()

            response = {
                "message": f"Hello, {name}!"
            }
            response_json = json.dumps(response).encode()
            self.wfile.write(response_json)

        print(f"Received request: GET /?name={name}")
        print("Sending response:", response)


def run(server_class=HTTPServer, handler_class=SimpleHTTPRequestHandler, port=8000):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    print(f"Starting server on port {port}...")
    httpd.serve_forever()


if __name__ == '__main__':
    run()
