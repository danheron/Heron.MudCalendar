window.dataLayer = window.dataLayer || [];
window.gtag = window.gtag || function () { dataLayer.push(arguments); };
gtag("js", new Date());
var GoogleAnalyticsInterop;
(function (GoogleAnalyticsInterop) {
    function configure(trackingId, globalConfigObject, debug) {
        if (debug === void 0) { debug = false; }
        this.debug = debug;
        this.globalConfigObject = globalConfigObject;
        var script = document.createElement("script");
        script.async = true;
        script.src = "https://www.googletagmanager.com/gtag/js?id=" + trackingId;
        document.head.appendChild(script);
        var configObject = {};
        configObject.send_page_view = false;
        Object.assign(configObject, globalConfigObject);
        gtag("config", trackingId, configObject);
        if (this.debug) {
            console.log("[GTAG][" + trackingId + "] Configured!");
        }
    }
    GoogleAnalyticsInterop.configure = configure;
    function navigate(trackingId, href) {
        var configObject = {};
        configObject.page_location = href;
        Object.assign(configObject, this.globalConfigObject);
        gtag("config", trackingId, configObject);
        if (this.debug) {
            console.log("[GTAG][" + trackingId + "] Navigated: '" + href + "'");
        }
    }
    GoogleAnalyticsInterop.navigate = navigate;
    function trackEvent(eventName, eventData, globalEventData) {
        Object.assign(eventData, globalEventData);
        gtag("event", eventName, eventData);
        if (this.debug) {
            console.log("[GTAG][Event triggered]: " + eventName);
        }
    }
    GoogleAnalyticsInterop.trackEvent = trackEvent;
})(GoogleAnalyticsInterop || (GoogleAnalyticsInterop = {}));
