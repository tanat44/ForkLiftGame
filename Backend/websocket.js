var WebSocket = require('ws');
var queryString = require('query-string');

function createServer (expressServer) {
    const wss = new WebSocket.Server({
        noServer: true,
        path: "",
      });
    
      expressServer.on("upgrade", (request, socket, head) => {
        wss.handleUpgrade(request, socket, head, (websocket) => {
          wss.emit("connection", websocket, request);
        });
      });
    
      wss.on("connection", (ws) =>{
        ws.on('message', (message, isBinary) => {
            console.log('WS message', message, isBinary)
            wss.clients.forEach(function each(client) {
                if (client.readyState === WebSocket.OPEN) {
                    client.send(message, { binary: isBinary });
                }
            });
        });

        ws.send(JSON.stringify({type: 'server', data: 'welcome new peer'}));
      }
    );
}

module.exports = { createServer } 