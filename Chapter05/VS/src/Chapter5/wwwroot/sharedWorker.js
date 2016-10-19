/* This file implements the shared web worker */
var clients = 0;

onconnect = function (event) {
    var port = event.ports[0];
    clients++;

    /* Attach the event listener */
    port.addEventListener("message", function (event) {
        sendResponse(event.target, "Msg received: " + event.data);
    }, false);

    port.start();

    sendResponse(port, "You are client # " + clients + "\n");
}

function sendResponse(senderPort, message) {
    senderPort.postMessage(message);
}
