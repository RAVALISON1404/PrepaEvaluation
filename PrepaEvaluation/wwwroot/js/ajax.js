function Ajax(method, url, data) {
    return new Promise((resolve, reject) => {
        const request = new XMLHttpRequest();
        request.open(method, url);
        request.onload = function () {
            if (request.status >= 200 && request.status < 300) {
                resolve(JSON.parse(request.responseText));
            } else {
                reject({
                    status: request.status,
                    statusText: request.statusText
                });
            }
        };
        request.onerror = function () {
            reject({
                status: request.status,
                statusText: request.statusText
            });
        };
        if (data) {
            request.setRequestHeader('Content-Type', 'application/json');
            request.send(JSON.stringify(data)); 
        } else {
            request.send();
        }
    });
}