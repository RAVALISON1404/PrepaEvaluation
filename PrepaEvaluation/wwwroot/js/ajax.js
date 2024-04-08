function Ajax(method, url, data) {
    return new Promise((resolve, reject) => {
        const request = new XMLHttpRequest();
        request.open(method, url);
        request.setRequestHeader('Content-Type', 'application/json'); // Déplacez ceci ici pour tous les cas

        request.onload = function () {
            if (request.status >= 200 && request.status < 300) {
                resolve(JSON.parse(request.responseText)); // Parsez la réponse JSON
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
            request.send(JSON.stringify(data)); // Envoyez les données en tant que chaîne JSON
        } else {
            request.send();
        }
    });
}
