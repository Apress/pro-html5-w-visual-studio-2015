/* This file contains functions used to 
   communicate with the web worker */

var myWorker;

function createWorker() {
    if (typeof (Worker) !== "undefined") {
        var log = document.querySelector("#output");
        log.value += "Starting worker process... ";

        myWorker = new Worker("worker.js");

        log.value += "Adding listener... ";
        myWorker.onmessage = function (event) {
            log.value += event.data + "\n";
        }

        log.value += "Done!\n";
    }
    else {
        alert("Your browser does not support web workers");
    }
}

function sendWorkerMessage() {
    if (myWorker !== null) {
        var log = document.querySelector("#output");
        log.value += "Sending message... ";

        var message = document.querySelector("#message");
        myWorker.postMessage(message.value);

        log.value += "Done!\n";
    }
}

function closeWorker() {
    if (myWorker !== null) {
        var log = document.querySelector("#output");
        log.value += "Closing worker... ";

        myWorker.terminate;
        myWorker = null;

        log.value += "Done!\n";
    }
}
