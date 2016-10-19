/* This file implements the web worker */

// This event is fired when the web worker is started
onconnect = sendResponse("The worker has started");

// This event is fired when a message is received
onmessage = function (event) {
    sendResponse("Msg received: " + event.data);
}

// Sends a message to the main thread
function sendResponse(message) {
    postMessage(message);
}
