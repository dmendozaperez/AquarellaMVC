var request = new XMLHttpRequest();
request.open('POST', '/ArticuloStock/UpdateStats', false);
request.send(null);

if (request.status == 200) {
    postMessage(request.responseText);  
}
