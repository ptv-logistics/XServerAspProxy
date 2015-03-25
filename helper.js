    // returns a layer group for xmap back- and foreground layers
    function getXMapBaseLayers(url, style, token, attribution) {
        var background = new L.TileLayer.WMS(url, {
            maxZoom: 19, minZoom: 0, opacity: 1.0,
            noWrap: true,
            layers: style? 'xmap-' + style + '-bg': 'xmap-ajaxbg',
            format: 'image/png', transparent: false,
            attribution: attribution
        });

        var foreground = new L.NonTiledLayer.WMS(url + (token? "?xtok=" + token : ''), {
            minZoom: 0, opacity: 1.0,
            layers: style ? 'xmap-' + style + '-fg' : 'xmap-ajaxfg',
            format: 'image/png', transparent: true,
            attribution: attribution,
        });

        return L.layerGroup([background, foreground]);
    }

// my dirty little wkt line string to geojson poly function
function isoToPoly(wkt) {
    x = replaceAll('LINESTRING', '', wkt);
    x = x.trim();
    x = replaceAll(', ', '],[', x);
    x = replaceAll(' ', ',', x);
    x = replaceAll('(', '[', x);
    x = replaceAll(')', ']', x);
    x = '[[' + x + ']]';
    return eval(x);
}

function fixClickPropagationSoItWorksForF___ingIE11(container) {
    container.onclick = L.DomEvent.stopPropagation;
    var inputTags = container.getElementsByTagName("input");
    for (var i = 0; i < inputTags.length; i++) {
        if (inputTags[i].type == "text")
            inputTags[i].onclick = L.DomEvent.stopPropagation;
        inputTags[i].onmousedown = inputTags[i].ondblclick = inputTags[i].onpointerdown = L.DomEvent.stopPropagation;
    }
}

function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function replaceAll(find, replace, str) {
    return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}

// runRequest executes a json request on PTV xServer internet,
// given the url endpoint, the token and the callbacks to be called
// upon completion. The error callback is parameterless, the success
// callback is called with the object returned by the server.
function runRequest(url, request, token, handleSuccess, handleError) {
    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(request),

        headers: {
            "Authorization": "Basic " + btoa("xtok:" + token),
            "Content-Type": "application/json"
        },

        success: function (data, status, xhr) {
            handleSuccess(data);
        },

        error: function (xhr, status, error) {
            handleError(xhr);
        }
    });
}

function lngFormatter(num) {
    var direction = (num < 0) ? 'W' : 'E';
    var formatted = Math.abs(L.Util.formatNum(num, 3)) + '&ordm; ' + direction;
    return formatted;
}

function latFormatter(num) {
    var direction = (num < 0) ? 'S' : 'N';
    var formatted = Math.abs(L.Util.formatNum(num, 3)) + '&ordm; ' + direction;
    return formatted;
}
