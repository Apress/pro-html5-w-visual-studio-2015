/* This file contains functions used to 
   communicate with the web worker */

var mySharedWorker;

function createSharedWorker() {
    if (typeof (SharedWorker) !== "undefined") {
        var log = document.querySelector("#output");
        log.value += "Starting shared worker process... ";

        mySharedWorker = new SharedWorker("sharedWorker.js");

        log.value += "Adding listener... ";
        mySharedWorker.port.addEventListener("message", function (event) {
            log.value += event.data + "\n";
        }, false);

        mySharedWorker.port.start();

        log.value += "Done!\n";
    }
    else {
        alert("Your browser does not support shared web workers");
    }
}

function sendSharedWorkerMessage() {
    if (mySharedWorker !== null) {
        var log = document.querySelector("#output");
        log.value += "Sending message... ";

        var message = document.querySelector("#messageS");
        mySharedWorker.port.postMessage(message.value);

        log.value += "Done!\n";
    }
}

function closeSharedWorker() {
    if (mySharedWorker !== null) {
        var log = document.querySelector("#output");
        log.value += "Closing worker... ";

        mySharedWorker.port.terminate;
        mySharedWorker = null;

        log.value += "Done!\n";
    }
}
