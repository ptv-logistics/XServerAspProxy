# XServerAspProxy
#### Relay PTV xServer Requests in your ASP.NET Middleware

This can be useful when accessing xServer behind a firewall, allow cross-domain and cross-scheme calls, 
add some load-balancing or fail-over, tile-caching, etc. 
You can also use your own authorization or inject xServer credentials inside the middleware.
For example, you can add your xserver internet token and don't have to expose it in JavaScript.
The web-client can still use xServer in exactly same way as it does for a direct access.

The JavaScript sample-code is based on https://github.com/ptv-logistics/PoiLocator, but all calls go
through the XServerProxy.ashx rather than calling xServer internet directly. 

For more samples with xServer in combination with ASP.NET, you should also take a look at 
https://github.com/ptv-logistics/WebFormsMap or https://github.com/ptv-logistics/SpatialTutorial.

To use this code with xServer internet, you need an xServer internet subscription. 
Go to http://xserver.ptvgroup.com/en-uk/products/ptv-xserver-internet/test/ to get a trial token.

## How the sample works
This is a javascript/ASP.NET sample that loads some data from an ASP page and does some geographic visualizations
and operations on it. For map-display and geographic operations PTV xServer is used. Unlike the
other xServer/JavaScript samples, the xServer isn't accessed directly from the client, but through the
middleware that hosts the web page.

## To use this code in your own project
1. Add the the XServerProxy handler code to your ASP.NET project
2. Add a redirection rule in your [Web.config](https://github.com/ptv-logistics/XServerAspProxy/blob/master/Web.config#L14-17)
3. Adapt the code for your xServer infrasctructure. I've added sample for 
[xServer on-premise](https://github.com/ptv-logistics/XServerAspProxy/blob/master/XServerProxy.ashx.cs#L74-93)
and [xServer internet](https://github.com/ptv-logistics/XServerAspProxy/blob/master/XServerProxy.ashx.cs#L62-73)
3. Make your xServer internet requests with the Proxy url, like *http://localhost:53758/XServerProxy/WMS/WMS* or
*http://localhost:53758/XServerProxy/xlocate/rs/XLocate/findAddressByText*.
