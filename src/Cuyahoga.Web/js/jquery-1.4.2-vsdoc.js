(function (window, undefined) {
    var jQuery = function (selector, context) {
        /// <summary>1: Accepts a string containing a CSS selector which is then used to match a set of elements.
        /// 1.1 - $(selector, context) 
        /// 1.2 - $(element) 
        /// 1.3 - $(elementArray) 
        /// 1.4 - $(jQuery object) 
        /// 1.5 - $()
        /// 2: Creates DOM elements on the fly from the provided string of raw HTML.
        /// 2.1 - $(html, ownerDocument) 
        /// 2.2 - $(html, props)
        /// 3: Binds a function to be executed when the DOM has finished loading.
        /// 3.1 - $(callback)</summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A string containing a selector expression</param>
        /// <param name="context" type="jQuery">A DOM Element, Document, or jQuery to use as context</param>

        // The jQuery object is actually just the init constructor 'enhanced'
        return new jQuery.fn.init(selector, context);
    };
    function access(elems, key, value, exec, fn, pass) {
        var length = elems.length;   // Setting many attributes
        if (typeof key === "object") { for (var k in key) { access(elems, k, key[k], exec, fn, value); } return elems; }   // Setting one attribute
        if (value !== undefined) {       // Optionally, function values get executed if exec is true
            exec = !pass && exec && jQuery.isFunction(value); for (var i = 0; i < length; i++) { fn(elems[i], key, exec ? value.call(elems[i], i, fn(elems[i], key)) : value, pass); } return elems;
        }   // Getting an attribute
        return length ? fn(elems[0], key) : undefined;
    }
    jQuery.Event = function (src) {

        // Allow instantiation without the 'new' keyword
        if (!this.preventDefault) {
            return new jQuery.Event(src);
        }

        // Event object
        if (src && src.type) {
            this.originalEvent = src;
            this.type = src.type;
            // Event type
        } else {
            this.type = src;
        }

        // timeStamp is buggy for some events on Firefox(#3843)
        // So we won't rely on the native value
        this.timeStamp = now();

        // Mark it as fixed
        this[expando] = true;
    };
    jQuery.active =
0;
    jQuery.ajax = function (origSettings) {
        /// <summary>Perform an asynchronous HTTP (Ajax) request.
        /// </summary>
        /// <returns type="XMLHttpRequest" />
        /// <param name="origSettings" type="Object">             A set of key/value pairs that configure the Ajax request. All options are optional. A default can be set for any option with $.ajaxSetup().           </param>

        var s = jQuery.extend(true, {}, jQuery.ajaxSettings, origSettings);

        var jsonp, status, data,
			callbackContext = origSettings && origSettings.context || s,
			type = s.type.toUpperCase();

        // convert data if not already a string
        if (s.data && s.processData && typeof s.data !== "string") {
            s.data = jQuery.param(s.data, s.traditional);
        }

        // Handle JSONP Parameter Callbacks
        if (s.dataType === "jsonp") {
            if (type === "GET") {
                if (!jsre.test(s.url)) {
                    s.url += (rquery.test(s.url) ? "&" : "?") + (s.jsonp || "callback") + "=?";
                }
            } else if (!s.data || !jsre.test(s.data)) {
                s.data = (s.data ? s.data + "&" : "") + (s.jsonp || "callback") + "=?";
            }
            s.dataType = "json";
        }

        // Build temporary JSONP function
        if (s.dataType === "json" && (s.data && jsre.test(s.data) || jsre.test(s.url))) {
            jsonp = s.jsonpCallback || ("jsonp" + jsc++);

            // Replace the =? sequence both in the query string and the data
            if (s.data) {
                s.data = (s.data + "").replace(jsre, "=" + jsonp + "$1");
            }

            s.url = s.url.replace(jsre, "=" + jsonp + "$1");

            // We need to make sure
            // that a JSONP style response is executed properly
            s.dataType = "script";

            // Handle JSONP-style loading
            window[jsonp] = window[jsonp] || function (tmp) {
                data = tmp;
                success();
                complete();
                // Garbage collect
                window[jsonp] = undefined;

                try {
                    delete window[jsonp];
                } catch (e) { }

                if (head) {
                    head.removeChild(script);
                }
            };
        }

        if (s.dataType === "script" && s.cache === null) {
            s.cache = false;
        }

        if (s.cache === false && type === "GET") {
            var ts = now();

            // try replacing _= if it is there
            var ret = s.url.replace(rts, "$1_=" + ts + "$2");

            // if nothing was replaced, add timestamp to the end
            s.url = ret + ((ret === s.url) ? (rquery.test(s.url) ? "&" : "?") + "_=" + ts : "");
        }

        // If data is available, append data to url for get requests
        if (s.data && type === "GET") {
            s.url += (rquery.test(s.url) ? "&" : "?") + s.data;
        }

        // Watch for a new set of requests
        if (s.global && !jQuery.active++) {
            jQuery.event.trigger("ajaxStart");
        }

        // Matches an absolute URL, and saves the domain
        var parts = rurl.exec(s.url),
			remote = parts && (parts[1] && parts[1] !== location.protocol || parts[2] !== location.host);

        // If we're requesting a remote document
        // and trying to load JSON or Script with a GET
        if (s.dataType === "script" && type === "GET" && remote) {
            var head = document.getElementsByTagName("head")[0] || document.documentElement;
            var script = document.createElement("script");
            script.src = s.url;
            if (s.scriptCharset) {
                script.charset = s.scriptCharset;
            }

            // Handle Script loading
            if (!jsonp) {
                var done = false;

                // Attach handlers for all browsers
                script.onload = script.onreadystatechange = function () {
                    if (!done && (!this.readyState ||
							this.readyState === "loaded" || this.readyState === "complete")) {
                        done = true;
                        success();
                        complete();

                        // Handle memory leak in IE
                        script.onload = script.onreadystatechange = null;
                        if (head && script.parentNode) {
                            head.removeChild(script);
                        }
                    }
                };
            }

            // Use insertBefore instead of appendChild  to circumvent an IE6 bug.
            // This arises when a base node is used (#2709 and #4378).
            head.insertBefore(script, head.firstChild);

            // We handle everything using the script element injection
            return undefined;
        }

        var requestDone = false;

        // Create the request object
        var xhr = s.xhr();

        if (!xhr) {
            return;
        }

        // Open the socket
        // Passing null username, generates a login popup on Opera (#2865)
        if (s.username) {
            xhr.open(type, s.url, s.async, s.username, s.password);
        } else {
            xhr.open(type, s.url, s.async);
        }

        // Need an extra try/catch for cross domain requests in Firefox 3
        try {
            // Set the correct header, if data is being sent
            if (s.data || origSettings && origSettings.contentType) {
                xhr.setRequestHeader("Content-Type", s.contentType);
            }

            // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
            if (s.ifModified) {
                if (jQuery.lastModified[s.url]) {
                    xhr.setRequestHeader("If-Modified-Since", jQuery.lastModified[s.url]);
                }

                if (jQuery.etag[s.url]) {
                    xhr.setRequestHeader("If-None-Match", jQuery.etag[s.url]);
                }
            }

            // Set header so the called script knows that it's an XMLHttpRequest
            // Only send the header if it's not a remote XHR
            if (!remote) {
                xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            }

            // Set the Accepts header for the server, depending on the dataType
            xhr.setRequestHeader("Accept", s.dataType && s.accepts[s.dataType] ?
				s.accepts[s.dataType] + ", */*" :
				s.accepts._default);
        } catch (e) { }

        // Allow custom headers/mimetypes and early abort
        if (s.beforeSend && s.beforeSend.call(callbackContext, xhr, s) === false) {
            // Handle the global AJAX counter
            if (s.global && ! --jQuery.active) {
                jQuery.event.trigger("ajaxStop");
            }

            // close opended socket
            xhr.abort();
            return false;
        }

        if (s.global) {
            trigger("ajaxSend", [xhr, s]);
        }

        // Wait for a response to come back
        var onreadystatechange = xhr.onreadystatechange = function (isTimeout) {
            // The request was aborted
            if (!xhr || xhr.readyState === 0 || isTimeout === "abort") {
                // Opera doesn't call onreadystatechange before this point
                // so we simulate the call
                if (!requestDone) {
                    complete();
                }

                requestDone = true;
                if (xhr) {
                    xhr.onreadystatechange = jQuery.noop;
                }

                // The transfer is complete and the data is available, or the request timed out
            } else if (!requestDone && xhr && (xhr.readyState === 4 || isTimeout === "timeout")) {
                requestDone = true;
                xhr.onreadystatechange = jQuery.noop;

                status = isTimeout === "timeout" ?
					"timeout" :
					!jQuery.httpSuccess(xhr) ?
						"error" :
						s.ifModified && jQuery.httpNotModified(xhr, s.url) ?
							"notmodified" :
							"success";

                var errMsg;

                if (status === "success") {
                    // Watch for, and catch, XML document parse errors
                    try {
                        // process the data (runs the xml through httpData regardless of callback)
                        data = jQuery.httpData(xhr, s.dataType, s);
                    } catch (err) {
                        status = "parsererror";
                        errMsg = err;
                    }
                }

                // Make sure that the request was successful or notmodified
                if (status === "success" || status === "notmodified") {
                    // JSONP handles its own success callback
                    if (!jsonp) {
                        success();
                    }
                } else {
                    jQuery.handleError(s, xhr, status, errMsg);
                }

                // Fire the complete handlers
                complete();

                if (isTimeout === "timeout") {
                    xhr.abort();
                }

                // Stop memory leaks
                if (s.async) {
                    xhr = null;
                }
            }
        };

        // Override the abort handler, if we can (IE doesn't allow it, but that's OK)
        // Opera doesn't fire onreadystatechange at all on abort
        try {
            var oldAbort = xhr.abort;
            xhr.abort = function () {
                if (xhr) {
                    oldAbort.call(xhr);
                }

                onreadystatechange("abort");
            };
        } catch (e) { }

        // Timeout checker
        if (s.async && s.timeout > 0) {
            setTimeout(function () {
                // Check to see if the request is still happening
                if (xhr && !requestDone) {
                    onreadystatechange("timeout");
                }
            }, s.timeout);
        }

        // Send the data
        try {
            xhr.send(type === "POST" || type === "PUT" || type === "DELETE" ? s.data : null);
        } catch (e) {
            jQuery.handleError(s, xhr, null, e);
            // Fire the complete handlers
            complete();
        }

        // firefox 1.5 doesn't fire statechange for sync requests
        if (!s.async) {
            onreadystatechange();
        }

        function success() {
            // If a local callback was specified, fire it and pass it the data
            if (s.success) {
                s.success.call(callbackContext, data, status, xhr);
            }

            // Fire the global callback
            if (s.global) {
                trigger("ajaxSuccess", [xhr, s]);
            }
        }

        function complete() {
            // Process result
            if (s.complete) {
                s.complete.call(callbackContext, xhr, status);
            }

            // The request was completed
            if (s.global) {
                trigger("ajaxComplete", [xhr, s]);
            }

            // Handle the global AJAX counter
            if (s.global && ! --jQuery.active) {
                jQuery.event.trigger("ajaxStop");
            }
        }

        function trigger(type, args) {
            (s.context ? jQuery(s.context) : jQuery.event).trigger(type, args);
        }

        // return XMLHttpRequest to allow aborting the request etc.
        return xhr;
    };
    jQuery.ajaxSetup = function (settings) {
        /// <summary>Set default values for future Ajax requests.
        /// </summary>/// <param name="settings" type="Object">A set of key/value pairs that configure the default Ajax request. All options are optional. </param>

        jQuery.extend(jQuery.ajaxSettings, settings);
    };
    jQuery.attr = function (elem, name, value, pass) {

        // don't set attributes on text and comment nodes
        if (!elem || elem.nodeType === 3 || elem.nodeType === 8) {
            return undefined;
        }

        if (pass && name in jQuery.attrFn) {
            return jQuery(elem)[name](value);
        }

        var notxml = elem.nodeType !== 1 || !jQuery.isXMLDoc(elem),
        // Whether we are setting (or getting)
			set = value !== undefined;

        // Try to normalize/fix the name
        name = notxml && jQuery.props[name] || name;

        // Only do all the following if this is a node (faster for style)
        if (elem.nodeType === 1) {
            // These attributes require special treatment
            var special = rspecialurl.test(name);

            // Safari mis-reports the default selected property of an option
            // Accessing the parent's selectedIndex property fixes it
            if (name === "selected" && !jQuery.support.optSelected) {
                var parent = elem.parentNode;
                if (parent) {
                    parent.selectedIndex;

                    // Make sure that it also works with optgroups, see #5701
                    if (parent.parentNode) {
                        parent.parentNode.selectedIndex;
                    }
                }
            }

            // If applicable, access the attribute via the DOM 0 way
            if (name in elem && notxml && !special) {
                if (set) {
                    // We can't allow the type property to be changed (since it causes problems in IE)
                    if (name === "type" && rtype.test(elem.nodeName) && elem.parentNode) {
                        jQuery.error("type property can't be changed");
                    }

                    elem[name] = value;
                }

                // browsers index elements by id/name on forms, give priority to attributes.
                if (jQuery.nodeName(elem, "form") && elem.getAttributeNode(name)) {
                    return elem.getAttributeNode(name).nodeValue;
                }

                // elem.tabIndex doesn't always return the correct value when it hasn't been explicitly set
                // http://fluidproject.org/blog/2008/01/09/getting-setting-and-removing-tabindex-values-with-javascript/
                if (name === "tabIndex") {
                    var attributeNode = elem.getAttributeNode("tabIndex");

                    return attributeNode && attributeNode.specified ?
						attributeNode.value :
						rfocusable.test(elem.nodeName) || rclickable.test(elem.nodeName) && elem.href ?
							0 :
							undefined;
                }

                return elem[name];
            }

            if (!jQuery.support.style && notxml && name === "style") {
                if (set) {
                    elem.style.cssText = "" + value;
                }

                return elem.style.cssText;
            }

            if (set) {
                // convert the value to a string (all browsers do this but IE) see #1070
                elem.setAttribute(name, "" + value);
            }

            var attr = !jQuery.support.hrefNormalized && notxml && special ?
            // Some attributes require a special call on IE
					elem.getAttribute(name, 2) :
					elem.getAttribute(name);

            // Non-existent attributes return null, we normalize to undefined
            return attr === null ? undefined : attr;
        }

        // elem is actually elem.style ... set the style
        // Using attr for specific style information is now deprecated. Use style instead.
        return jQuery.style(elem, name, value);
    };
    jQuery.bindReady = function () {

        if (readyBound) {
            return;
        }

        readyBound = true;

        // Catch cases where $(document).ready() is called after the
        // browser event has already occurred.
        if (document.readyState === "complete") {
            return jQuery.ready();
        }

        // Mozilla, Opera and webkit nightlies currently support this event
        if (document.addEventListener) {
            // Use the handy event callback
            document.addEventListener("DOMContentLoaded", DOMContentLoaded, false);

            // A fallback to window.onload, that will always work
            window.addEventListener("load", jQuery.ready, false);

            // If IE event model is used
        } else if (document.attachEvent) {
            // ensure firing before onload,
            // maybe late but safe also for iframes
            document.attachEvent("onreadystatechange", DOMContentLoaded);

            // A fallback to window.onload, that will always work
            window.attachEvent("onload", jQuery.ready);

            // If IE and not a frame
            // continually check to see if the document is ready
            var toplevel = false;

            try {
                toplevel = window.frameElement == null;
            } catch (e) { }

            if (document.documentElement.doScroll && toplevel) {
                doScrollCheck();
            }
        }
    };
    jQuery.boxModel =
true;
    jQuery.clean = function (elems, context, fragment, scripts) {

        context = context || document;

        // !context.createElement fails in IE with an error but returns typeof 'object'
        if (typeof context.createElement === "undefined") {
            context = context.ownerDocument || context[0] && context[0].ownerDocument || document;
        }

        var ret = [];

        for (var i = 0, elem; (elem = elems[i]) != null; i++) {
            if (typeof elem === "number") {
                elem += "";
            }

            if (!elem) {
                continue;
            }

            // Convert html string into DOM nodes
            if (typeof elem === "string" && !rhtml.test(elem)) {
                elem = context.createTextNode(elem);

            } else if (typeof elem === "string") {
                // Fix "XHTML"-style tags in all browsers
                elem = elem.replace(rxhtmlTag, fcloseTag);

                // Trim whitespace, otherwise indexOf won't work as expected
                var tag = (rtagName.exec(elem) || ["", ""])[1].toLowerCase(),
					wrap = wrapMap[tag] || wrapMap._default,
					depth = wrap[0],
					div = context.createElement("div");

                // Go to html and back, then peel off extra wrappers
                div.innerHTML = wrap[1] + elem + wrap[2];

                // Move to the right depth
                while (depth--) {
                    div = div.lastChild;
                }

                // Remove IE's autoinserted <tbody> from table fragments
                if (!jQuery.support.tbody) {

                    // String was a <table>, *may* have spurious <tbody>
                    var hasBody = rtbody.test(elem),
						tbody = tag === "table" && !hasBody ?
							div.firstChild && div.firstChild.childNodes :

                    // String was a bare <thead> or <tfoot>
							wrap[1] === "<table>" && !hasBody ?
								div.childNodes :
								[];

                    for (var j = tbody.length - 1; j >= 0; --j) {
                        if (jQuery.nodeName(tbody[j], "tbody") && !tbody[j].childNodes.length) {
                            tbody[j].parentNode.removeChild(tbody[j]);
                        }
                    }

                }

                // IE completely kills leading whitespace when innerHTML is used
                if (!jQuery.support.leadingWhitespace && rleadingWhitespace.test(elem)) {
                    div.insertBefore(context.createTextNode(rleadingWhitespace.exec(elem)[0]), div.firstChild);
                }

                elem = div.childNodes;
            }

            if (elem.nodeType) {
                ret.push(elem);
            } else {
                ret = jQuery.merge(ret, elem);
            }
        }

        if (fragment) {
            for (var i = 0; ret[i]; i++) {
                if (scripts && jQuery.nodeName(ret[i], "script") && (!ret[i].type || ret[i].type.toLowerCase() === "text/javascript")) {
                    scripts.push(ret[i].parentNode ? ret[i].parentNode.removeChild(ret[i]) : ret[i]);

                } else {
                    if (ret[i].nodeType === 1) {
                        ret.splice.apply(ret, [i + 1, 0].concat(jQuery.makeArray(ret[i].getElementsByTagName("script"))));
                    }
                    fragment.appendChild(ret[i]);
                }
            }
        }

        return ret;
    };
    jQuery.cleanData = function (elems) {

        var data, id, cache = jQuery.cache,
			special = jQuery.event.special,
			deleteExpando = jQuery.support.deleteExpando;

        for (var i = 0, elem; (elem = elems[i]) != null; i++) {
            id = elem[jQuery.expando];

            if (id) {
                data = cache[id];

                if (data.events) {
                    for (var type in data.events) {
                        if (special[type]) {
                            jQuery.event.remove(elem, type);

                        } else {
                            removeEvent(elem, type, data.handle);
                        }
                    }
                }

                if (deleteExpando) {
                    delete elem[jQuery.expando];

                } else if (elem.removeAttribute) {
                    elem.removeAttribute(jQuery.expando);
                }

                delete cache[id];
            }
        }
    };
    jQuery.contains = function (a, b) {
        /// <summary>Check to see if a DOM node is within another DOM node.
        /// </summary>
        /// <returns type="Boolean" />
        /// <param name="a" domElement="true">The DOM element that may contain the other element.</param>
        /// <param name="b" domElement="true">The DOM node that may be contained by the other element.</param>

        return a !== b && (a.contains ? a.contains(b) : true);
    };
    jQuery.css = function (elem, name, force, extra) {

        if (name === "width" || name === "height") {
            var val, props = cssShow, which = name === "width" ? cssWidth : cssHeight;

            function getWH() {
                val = name === "width" ? elem.offsetWidth : elem.offsetHeight;

                if (extra === "border") {
                    return;
                }

                jQuery.each(which, function () {
                    if (!extra) {
                        val -= parseFloat(jQuery.curCSS(elem, "padding" + this, true)) || 0;
                    }

                    if (extra === "margin") {
                        val += parseFloat(jQuery.curCSS(elem, "margin" + this, true)) || 0;
                    } else {
                        val -= parseFloat(jQuery.curCSS(elem, "border" + this + "Width", true)) || 0;
                    }
                });
            }

            if (elem.offsetWidth !== 0) {
                getWH();
            } else {
                jQuery.swap(elem, props, getWH);
            }

            return Math.max(0, Math.round(val));
        }

        return jQuery.curCSS(elem, name, force);
    };
    jQuery.curCSS = function (elem, name, force) {

        var ret, style = elem.style, filter;

        // IE uses filters for opacity
        if (!jQuery.support.opacity && name === "opacity" && elem.currentStyle) {
            ret = ropacity.test(elem.currentStyle.filter || "") ?
				(parseFloat(RegExp.$1) / 100) + "" :
				"";

            return ret === "" ?
				"1" :
				ret;
        }

        // Make sure we're using the right name for getting the float value
        if (rfloat.test(name)) {
            name = styleFloat;
        }

        if (!force && style && style[name]) {
            ret = style[name];

        } else if (getComputedStyle) {

            // Only "float" is needed here
            if (rfloat.test(name)) {
                name = "float";
            }

            name = name.replace(rupper, "-$1").toLowerCase();

            var defaultView = elem.ownerDocument.defaultView;

            if (!defaultView) {
                return null;
            }

            var computedStyle = defaultView.getComputedStyle(elem, null);

            if (computedStyle) {
                ret = computedStyle.getPropertyValue(name);
            }

            // We should always get a number back from opacity
            if (name === "opacity" && ret === "") {
                ret = "1";
            }

        } else if (elem.currentStyle) {
            var camelCase = name.replace(rdashAlpha, fcamelCase);

            ret = elem.currentStyle[name] || elem.currentStyle[camelCase];

            // From the awesome hack by Dean Edwards
            // http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

            // If we're not dealing with a regular pixel number
            // but a number that has a weird ending, we need to convert it to pixels
            if (!rnumpx.test(ret) && rnum.test(ret)) {
                // Remember the original values
                var left = style.left, rsLeft = elem.runtimeStyle.left;

                // Put in the new values to get a computed value out
                elem.runtimeStyle.left = elem.currentStyle.left;
                style.left = camelCase === "fontSize" ? "1em" : (ret || 0);
                ret = style.pixelLeft + "px";

                // Revert the changed values
                style.left = left;
                elem.runtimeStyle.left = rsLeft;
            }
        }

        return ret;
    };
    jQuery.data = function (elem, name, data) {
        /// <summary>1: Store arbitrary data associated with the specified element.
        /// 1.1 - jQuery.data(element, key, value)
        /// 2: 
        ///         Returns value at named data store for the element, as set by jQuery.data(element, name, value), or the full data store for the element.
        ///       
        /// 2.1 - jQuery.data(element, key) 
        /// 2.2 - jQuery.data(element)</summary>
        /// <returns type="jQuery" />
        /// <param name="elem" domElement="true">The DOM element to associate with the data.</param>
        /// <param name="name" type="String">A string naming the piece of data to set.</param>
        /// <param name="data" type="Object">The new data value.</param>

        if (elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()]) {
            return;
        }

        elem = elem == window ?
			windowData :
			elem;

        var id = elem[expando], cache = jQuery.cache, thisCache;

        if (!id && typeof name === "string" && data === undefined) {
            return null;
        }

        // Compute a unique ID for the element
        if (!id) {
            id = ++uuid;
        }

        // Avoid generating a new cache unless none exists and we
        // want to manipulate it.
        if (typeof name === "object") {
            elem[expando] = id;
            thisCache = cache[id] = jQuery.extend(true, {}, name);

        } else if (!cache[id]) {
            elem[expando] = id;
            cache[id] = {};
        }

        thisCache = cache[id];

        // Prevent overriding the named cache with undefined values
        if (data !== undefined) {
            thisCache[name] = data;
        }

        return typeof name === "string" ? thisCache[name] : thisCache;
    };
    jQuery.dequeue = function (elem, type) {
        /// <summary>Execute the next function on the queue for the matched element.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="elem" domElement="true">A DOM element from which to remove and execute a queued function.</param>
        /// <param name="type" type="String">             A string containing the name of the queue. Defaults to fx, the standard effects queue.           </param>

        type = type || "fx";

        var queue = jQuery.queue(elem, type), fn = queue.shift();

        // If the fx queue is dequeued, always remove the progress sentinel
        if (fn === "inprogress") {
            fn = queue.shift();
        }

        if (fn) {
            // Add a progress sentinel to prevent the fx queue from being
            // automatically dequeued
            if (type === "fx") {
                queue.unshift("inprogress");
            }

            fn.call(elem, function () {
                jQuery.dequeue(elem, type);
            });
        }
    };
    jQuery.dir = function (elem, dir, until) {

        var matched = [], cur = elem[dir];
        while (cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !jQuery(cur).is(until))) {
            if (cur.nodeType === 1) {
                matched.push(cur);
            }
            cur = cur[dir];
        }
        return matched;
    };
    jQuery.each = function (object, callback, args) {
        /// <summary>
        ///         A generic iterator function, which can be used to seamlessly iterate over both objects and arrays. Arrays and array-like objects with a length property (such as a function's arguments object) are iterated by numeric index, from 0 to length-1. Other objects are iterated via their named properties.
        ///       
        /// </summary>
        /// <returns type="Object" />
        /// <param name="object" type="Object">The object or array to iterate over.</param>
        /// <param name="callback" type="Function">The function that will be executed on every object.</param>

        var name, i = 0,
			length = object.length,
			isObj = length === undefined || jQuery.isFunction(object);

        if (args) {
            if (isObj) {
                for (name in object) {
                    if (callback.apply(object[name], args) === false) {
                        break;
                    }
                }
            } else {
                for (; i < length; ) {
                    if (callback.apply(object[i++], args) === false) {
                        break;
                    }
                }
            }

            // A special, fast, case for the most common use of each
        } else {
            if (isObj) {
                for (name in object) {
                    if (callback.call(object[name], name, object[name]) === false) {
                        break;
                    }
                }
            } else {
                for (var value = object[0];
					i < length && callback.call(value, i, value) !== false; value = object[++i]) { }
            }
        }

        return object;
    };
    jQuery.error = function (msg) {
        /// <summary>Takes a string and throws an exception containing it.
        /// </summary>/// <param name="msg" type="String">The message to send out.</param>

        throw msg;
    };
    jQuery.extend = function () {
        /// <summary>Merge the contents of two or more objects together into the first object.
        /// 1 - jQuery.extend(target, object1, objectN) 
        /// 2 - jQuery.extend(deep, target, object1, objectN)</summary>
        /// <returns type="Object" />
        /// <param name="" type="Boolean">If true, the merge becomes recursive (aka. deep copy).</param>
        /// <param name="{name}" type="Object">The object to extend. It will receive the new properties.</param>
        /// <param name="{name}" type="Object">An object containing additional properties to merge in.</param>
        /// <param name="{name}" type="Object">Additional objects containing properties to merge in.</param>

        // copy reference to target object
        var target = arguments[0] || {}, i = 1, length = arguments.length, deep = false, options, name, src, copy;

        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !jQuery.isFunction(target)) {
            target = {};
        }

        // extend jQuery itself if only one argument is passed
        if (length === i) {
            target = this;
            --i;
        }

        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null) {
                // Extend the base object
                for (name in options) {
                    src = target[name];
                    copy = options[name];

                    // Prevent never-ending loop
                    if (target === copy) {
                        continue;
                    }

                    // Recurse if we're merging object literal values or arrays
                    if (deep && copy && (jQuery.isPlainObject(copy) || jQuery.isArray(copy))) {
                        var clone = src && (jQuery.isPlainObject(src) || jQuery.isArray(src)) ? src
						: jQuery.isArray(copy) ? [] : {};

                        // Never move original objects, clone them
                        target[name] = jQuery.extend(deep, clone, copy);

                        // Don't bring in undefined values
                    } else if (copy !== undefined) {
                        target[name] = copy;
                    }
                }
            }
        }

        // Return the modified object
        return target;
    };
    jQuery.filter = function (expr, elems, not) {

        if (not) {
            expr = ":not(" + expr + ")";
        }

        return jQuery.find.matches(expr, elems);
    };
    jQuery.find = function (query, context, extra, seed) {

        context = context || document;

        // Only use querySelectorAll on non-XML documents
        // (ID selectors don't work in non-HTML documents)
        if (!seed && context.nodeType === 9 && !isXML(context)) {
            try {
                return makeArray(context.querySelectorAll(query), extra);
            } catch (e) { }
        }

        return oldSizzle(query, context, extra, seed);
    };
    jQuery.fx = function (elem, options, prop) {

        this.options = options;
        this.elem = elem;
        this.prop = prop;

        if (!options.orig) {
            options.orig = {};
        }
    };
    jQuery.get = function (url, data, callback, type) {
        /// <summary>Load data from the server using a HTTP GET request.
        /// </summary>
        /// <returns type="XMLHttpRequest" />
        /// <param name="url" type="String">A string containing the URL to which the request is sent.</param>
        /// <param name="data" type="String">A map or string that is sent to the server with the request.</param>
        /// <param name="callback" type="Function">A callback function that is executed if the request succeeds.</param>
        /// <param name="type" type="String">The type of data expected from the server.</param>

        // shift arguments if data argument was omited
        if (jQuery.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = null;
        }

        return jQuery.ajax({
            type: "GET",
            url: url,
            data: data,
            success: callback,
            dataType: type
        });
    };
    jQuery.getJSON = function (url, data, callback) {
        /// <summary>Load JSON-encoded data from the server using a GET HTTP request.
        /// </summary>
        /// <returns type="XMLHttpRequest" />
        /// <param name="url" type="String">A string containing the URL to which the request is sent.</param>
        /// <param name="data" type="Object">A map or string that is sent to the server with the request.</param>
        /// <param name="callback" type="Function">A callback function that is executed if the request succeeds.</param>

        return jQuery.get(url, data, callback, "json");
    };
    jQuery.getScript = function (url, callback) {
        /// <summary>Load a JavaScript file from the server using a GET HTTP request, then execute it.
        /// </summary>
        /// <returns type="XMLHttpRequest" />
        /// <param name="url" type="String">A string containing the URL to which the request is sent.</param>
        /// <param name="callback" type="Function">A callback function that is executed if the request succeeds.</param>

        return jQuery.get(url, null, callback, "script");
    };
    jQuery.globalEval = function (data) {
        /// <summary>Execute some JavaScript code globally.
        /// </summary>/// <param name="data" type="String">The JavaScript code to execute.</param>

        if (data && rnotwhite.test(data)) {
            // Inspired by code by Andrea Giammarchi
            // http://webreflection.blogspot.com/2007/08/global-scope-evaluation-and-dom.html
            var head = document.getElementsByTagName("head")[0] || document.documentElement,
				script = document.createElement("script");

            script.type = "text/javascript";

            if (jQuery.support.scriptEval) {
                script.appendChild(document.createTextNode(data));
            } else {
                script.text = data;
            }

            // Use insertBefore instead of appendChild to circumvent an IE6 bug.
            // This arises when a base node is used (#2709).
            head.insertBefore(script, head.firstChild);
            head.removeChild(script);
        }
    };
    jQuery.grep = function (elems, callback, inv) {
        /// <summary>Finds the elements of an array which satisfy a filter function. The original array is not affected.
        /// </summary>
        /// <returns type="Array" />
        /// <param name="elems" type="Array">The array to search through.</param>
        /// <param name="callback" type="Function">             The function to process each item against.  The first argument to the function is the item, and the second argument is the index.  The function should return a Boolean value.  this will be the global window object.           </param>
        /// <param name="inv" type="Boolean">If "invert" is false, or not provided, then the function returns an array consisting of all elements for which "callback" returns true.  If "invert" is true, then the function returns an array consisting of all elements for which "callback" returns false.</param>

        var ret = [];

        // Go through the array, only saving the items
        // that pass the validator function
        for (var i = 0, length = elems.length; i < length; i++) {
            if (!inv !== !callback(elems[i], i)) {
                ret.push(elems[i]);
            }
        }

        return ret;
    };
    jQuery.guid =
1;
    jQuery.handleError = function (s, xhr, status, e) {

        // If a local callback was specified, fire it
        if (s.error) {
            s.error.call(s.context || s, xhr, status, e);
        }

        // Fire the global callback
        if (s.global) {
            (s.context ? jQuery(s.context) : jQuery.event).trigger("ajaxError", [xhr, s, e]);
        }
    };
    jQuery.httpData = function (xhr, type, s) {

        var ct = xhr.getResponseHeader("content-type") || "",
			xml = type === "xml" || !type && ct.indexOf("xml") >= 0,
			data = xml ? xhr.responseXML : xhr.responseText;

        if (xml && data.documentElement.nodeName === "parsererror") {
            jQuery.error("parsererror");
        }

        // Allow a pre-filtering function to sanitize the response
        // s is checked to keep backwards compatibility
        if (s && s.dataFilter) {
            data = s.dataFilter(data, type);
        }

        // The filter can actually parse the response
        if (typeof data === "string") {
            // Get the JavaScript object, if JSON is used.
            if (type === "json" || !type && ct.indexOf("json") >= 0) {
                data = jQuery.parseJSON(data);

                // If the type is "script", eval it in global context
            } else if (type === "script" || !type && ct.indexOf("javascript") >= 0) {
                jQuery.globalEval(data);
            }
        }

        return data;
    };
    jQuery.httpNotModified = function (xhr, url) {

        var lastModified = xhr.getResponseHeader("Last-Modified"),
			etag = xhr.getResponseHeader("Etag");

        if (lastModified) {
            jQuery.lastModified[url] = lastModified;
        }

        if (etag) {
            jQuery.etag[url] = etag;
        }

        // Opera returns 0 when status is 304
        return xhr.status === 304 || xhr.status === 0;
    };
    jQuery.httpSuccess = function (xhr) {

        try {
            // IE error sometimes returns 1223 when it should be 204 so treat it as success, see #1450
            return !xhr.status && location.protocol === "file:" ||
            // Opera returns 0 when status is 304
				(xhr.status >= 200 && xhr.status < 300) ||
				xhr.status === 304 || xhr.status === 1223 || xhr.status === 0;
        } catch (e) { }

        return false;
    };
    jQuery.inArray = function (elem, array) {
        /// <summary>Search for a specified value within an array and return its index (or -1 if not found).
        /// </summary>
        /// <returns type="Number" />
        /// <param name="elem" type="Object">The value to search for.</param>
        /// <param name="array" type="Array">An array through which to search.</param>

        if (array.indexOf) {
            return array.indexOf(elem);
        }

        for (var i = 0, length = array.length; i < length; i++) {
            if (array[i] === elem) {
                return i;
            }
        }

        return -1;
    };
    jQuery.isArray = function (obj) {
        /// <summary>Determine whether the argument is an array.
        /// </summary>
        /// <returns type="boolean" />
        /// <param name="obj" type="Object">Object to test whether or not it is an array.</param>

        return toString.call(obj) === "[object Array]";
    };
    jQuery.isEmptyObject = function (obj) {
        /// <summary>Check to see if an object is empty (contains no properties).
        /// </summary>
        /// <returns type="Boolean" />
        /// <param name="obj" type="Object">The object that will be checked to see if it's empty.</param>

        for (var name in obj) {
            return false;
        }
        return true;
    };
    jQuery.isFunction = function (obj) {
        /// <summary>Determine if the argument passed is a Javascript function object. 
        /// </summary>
        /// <returns type="boolean" />
        /// <param name="obj" type="Object">Object to test whether or not it is a function.</param>

        return toString.call(obj) === "[object Function]";
    };
    jQuery.isPlainObject = function (obj) {
        /// <summary>Check to see if an object is a plain object (created using "{}" or "new Object").
        /// </summary>
        /// <returns type="Boolean" />
        /// <param name="obj" type="Object">The object that will be checked to see if it's a plain object.</param>

        // Must be an Object.
        // Because of IE, we also have to check the presence of the constructor property.
        // Make sure that DOM nodes and window objects don't pass through, as well
        if (!obj || toString.call(obj) !== "[object Object]" || obj.nodeType || obj.setInterval) {
            return false;
        }

        // Not own constructor property must be Object
        if (obj.constructor
			&& !hasOwnProperty.call(obj, "constructor")
			&& !hasOwnProperty.call(obj.constructor.prototype, "isPrototypeOf")) {
            return false;
        }

        // Own properties are enumerated firstly, so to speed up,
        // if last one is own, then all properties are own.

        var key;
        for (key in obj) { }

        return key === undefined || hasOwnProperty.call(obj, key);
    };
    jQuery.isReady =
true;
    jQuery.isXMLDoc = function (elem) {
        /// <summary>Check to see if a DOM node is within an XML document (or is an XML document).
        /// </summary>
        /// <returns type="Boolean" />
        /// <param name="elem" domElement="true">The DOM node that will be checked to see if it's in an XML document.</param>

        // documentElement is verified for cases where it doesn't yet exist
        // (such as loading iframes in IE - #4833) 
        var documentElement = (elem ? elem.ownerDocument || elem : 0).documentElement;
        return documentElement ? documentElement.nodeName !== "HTML" : false;
    };
    jQuery.makeArray = function (array, results) {
        /// <summary>Convert an array-like object into a true JavaScript array.
        /// </summary>
        /// <returns type="Array" />
        /// <param name="array" type="Object">Any object to turn into a native Array.</param>

        var ret = results || [];

        if (array != null) {
            // The window, strings (and functions) also have 'length'
            // The extra typeof function check is to prevent crashes
            // in Safari 2 (See: #3039)
            if (array.length == null || typeof array === "string" || jQuery.isFunction(array) || (typeof array !== "function" && array.setInterval)) {
                push.call(ret, array);
            } else {
                jQuery.merge(ret, array);
            }
        }

        return ret;
    };
    jQuery.map = function (elems, callback, arg) {
        /// <summary>Translate all items in an array or array-like object to another array of items.
        /// </summary>
        /// <returns type="Array" />
        /// <param name="elems" type="Array">The Array to translate.</param>
        /// <param name="callback" type="Function">             The function to process each item against.  The first argument to the function is the list item, the second argument is the index in array The function can return any value.  this will be the global window object.           </param>

        var ret = [], value;

        // Go through the array, translating each of the items to their
        // new value (or values).
        for (var i = 0, length = elems.length; i < length; i++) {
            value = callback(elems[i], i, arg);

            if (value != null) {
                ret[ret.length] = value;
            }
        }

        return ret.concat.apply([], ret);
    };
    jQuery.merge = function (first, second) {
        /// <summary>Merge the contents of two arrays together into the first array. 
        /// </summary>
        /// <returns type="Array" />
        /// <param name="first" type="Array">The first array to merge, the elements of second added.</param>
        /// <param name="second" type="Array">The second array to merge into the first, unaltered.</param>

        var i = first.length, j = 0;

        if (typeof second.length === "number") {
            for (var l = second.length; j < l; j++) {
                first[i++] = second[j];
            }

        } else {
            while (second[j] !== undefined) {
                first[i++] = second[j++];
            }
        }

        first.length = i;

        return first;
    };
    jQuery.noConflict = function (deep) {
        /// <summary>
        ///         Relinquish jQuery's control of the $ variable.
        ///       
        /// </summary>
        /// <returns type="Object" />
        /// <param name="deep" type="Boolean">A Boolean indicating whether to remove all jQuery variables from the global scope (including jQuery itself).</param>

        window.$ = _$;

        if (deep) {
            window.jQuery = _jQuery;
        }

        return jQuery;
    };
    jQuery.nodeName = function (elem, name) {

        return elem.nodeName && elem.nodeName.toUpperCase() === name.toUpperCase();
    };
    jQuery.noop = function () {
        /// <summary>An empty function.
        /// </summary>
        /// <returns type="Function" />
    };
    jQuery.nth = function (cur, result, dir, elem) {

        result = result || 1;
        var num = 0;

        for (; cur; cur = cur[dir]) {
            if (cur.nodeType === 1 && ++num === result) {
                break;
            }
        }

        return cur;
    };
    jQuery.param = function (a, traditional) {
        /// <summary>Create a serialized representation of an array or object, suitable for use in a URL query string or Ajax request. 
        /// 1 - jQuery.param(obj) 
        /// 2 - jQuery.param(obj, traditional)</summary>
        /// <returns type="String" />
        /// <param name="a" type="Object">An array or object to serialize.</param>
        /// <param name="traditional" type="Boolean">A Boolean indicating whether to perform a traditional "shallow" serialization.</param>

        var s = [];

        // Set traditional to true for jQuery <= 1.3.2 behavior.
        if (traditional === undefined) {
            traditional = jQuery.ajaxSettings.traditional;
        }

        // If an array was passed in, assume that it is an array of form elements.
        if (jQuery.isArray(a) || a.jquery) {
            // Serialize the form elements
            jQuery.each(a, function () {
                add(this.name, this.value);
            });

        } else {
            // If traditional, encode the "old" way (the way 1.3.2 or older
            // did it), otherwise encode params recursively.
            for (var prefix in a) {
                buildParams(prefix, a[prefix]);
            }
        }

        // Return the resulting serialization
        return s.join("&").replace(r20, "+");

        function buildParams(prefix, obj) {
            if (jQuery.isArray(obj)) {
                // Serialize array item.
                jQuery.each(obj, function (i, v) {
                    if (traditional || /\[\]$/.test(prefix)) {
                        // Treat each array item as a scalar.
                        add(prefix, v);
                    } else {
                        // If array item is non-scalar (array or object), encode its
                        // numeric index to resolve deserialization ambiguity issues.
                        // Note that rack (as of 1.0.0) can't currently deserialize
                        // nested arrays properly, and attempting to do so may cause
                        // a server error. Possible fixes are to modify rack's
                        // deserialization algorithm or to provide an option or flag
                        // to force array serialization to be shallow.
                        buildParams(prefix + "[" + (typeof v === "object" || jQuery.isArray(v) ? i : "") + "]", v);
                    }
                });

            } else if (!traditional && obj != null && typeof obj === "object") {
                // Serialize object item.
                jQuery.each(obj, function (k, v) {
                    buildParams(prefix + "[" + k + "]", v);
                });

            } else {
                // Serialize scalar item.
                add(prefix, obj);
            }
        }

        function add(key, value) {
            // If value is a function, invoke it and return its value
            value = jQuery.isFunction(value) ? value() : value;
            s[s.length] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
        }
    };
    jQuery.parseJSON = function (data) {
        /// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.
        /// </summary>
        /// <returns type="Object" />
        /// <param name="data" type="String">The JSON string to parse.</param>

        if (typeof data !== "string" || !data) {
            return null;
        }

        // Make sure leading/trailing whitespace is removed (IE can't handle it)
        data = jQuery.trim(data);

        // Make sure the incoming data is actual JSON
        // Logic borrowed from http://json.org/json2.js
        if (/^[\],:{}\s]*$/.test(data.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@")
			.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]")
			.replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) {

            // Try to use the native JSON parser first
            return window.JSON && window.JSON.parse ?
				window.JSON.parse(data) :
				(new Function("return " + data))();

        } else {
            jQuery.error("Invalid JSON: " + data);
        }
    };
    jQuery.post = function (url, data, callback, type) {
        /// <summary>Load data from the server using a HTTP POST request.
        /// </summary>
        /// <returns type="XMLHttpRequest" />
        /// <param name="url" type="String">A string containing the URL to which the request is sent.</param>
        /// <param name="data" type="String">A map or string that is sent to the server with the request.</param>
        /// <param name="callback" type="Function">A callback function that is executed if the request succeeds.</param>
        /// <param name="type" type="String">The type of data expected from the server.</param>

        // shift arguments if data argument was omited
        if (jQuery.isFunction(data)) {
            type = type || callback;
            callback = data;
            data = {};
        }

        return jQuery.ajax({
            type: "POST",
            url: url,
            data: data,
            success: callback,
            dataType: type
        });
    };
    jQuery.proxy = function (fn, proxy, thisObject) {
        /// <summary>Takes a function and returns a new one that will always have a particular context.
        /// 1 - jQuery.proxy(function, context) 
        /// 2 - jQuery.proxy(context, name)</summary>
        /// <returns type="Function" />
        /// <param name="fn" type="Function">The function whose context will be changed.</param>
        /// <param name="proxy" type="Object">The object to which the context (`this`) of the function should be set.</param>

        if (arguments.length === 2) {
            if (typeof proxy === "string") {
                thisObject = fn;
                fn = thisObject[proxy];
                proxy = undefined;

            } else if (proxy && !jQuery.isFunction(proxy)) {
                thisObject = proxy;
                proxy = undefined;
            }
        }

        if (!proxy && fn) {
            proxy = function () {
                return fn.apply(thisObject || this, arguments);
            };
        }

        // Set the guid of unique handler to the same of original handler, so it can be removed
        if (fn) {
            proxy.guid = fn.guid = fn.guid || proxy.guid || jQuery.guid++;
        }

        // So proxy can be declared as an argument
        return proxy;
    };
    jQuery.queue = function (elem, type, data) {
        /// <summary>1: Show the queue of functions to be executed on the matched element.
        /// 1.1 - jQuery.queue(element, queueName)
        /// 2: Manipulate the queue of functions to be executed on the matched element.
        /// 2.1 - jQuery.queue(element, queueName, newQueue) 
        /// 2.2 - jQuery.queue(element, queueName, callback())</summary>
        /// <returns type="jQuery" />
        /// <param name="elem" domElement="true">A DOM element where the array of queued functions is attached.</param>
        /// <param name="type" type="String">             A string containing the name of the queue. Defaults to fx, the standard effects queue.           </param>
        /// <param name="data" type="Array">An array of functions to replace the current queue contents.</param>

        if (!elem) {
            return;
        }

        type = (type || "fx") + "queue";
        var q = jQuery.data(elem, type);

        // Speed up dequeue by getting out quickly if this is just a lookup
        if (!data) {
            return q || [];
        }

        if (!q || jQuery.isArray(data)) {
            q = jQuery.data(elem, type, jQuery.makeArray(data));

        } else {
            q.push(data);
        }

        return q;
    };
    jQuery.ready = function () {

        // Make sure that the DOM is not already loaded
        if (!jQuery.isReady) {
            // Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
            if (!document.body) {
                return setTimeout(jQuery.ready, 13);
            }

            // Remember that the DOM is ready
            jQuery.isReady = true;

            // If there are functions bound, to execute
            if (readyList) {
                // Execute all of them
                var fn, i = 0;
                while ((fn = readyList[i++])) {
                    fn.call(document, jQuery);
                }

                // Reset the list of functions
                readyList = null;
            }

            // Trigger any bound ready events
            if (jQuery.fn.triggerHandler) {
                jQuery(document).triggerHandler("ready");
            }
        }
    };
    jQuery.removeData = function (elem, name) {
        /// <summary>Remove a previously-stored piece of data.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="elem" domElement="true">A DOM element from which to remove data.</param>
        /// <param name="name" type="String">A string naming the piece of data to remove.</param>

        if (elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()]) {
            return;
        }

        elem = elem == window ?
			windowData :
			elem;

        var id = elem[expando], cache = jQuery.cache, thisCache = cache[id];

        // If we want to remove a specific section of the element's data
        if (name) {
            if (thisCache) {
                // Remove the section of cache data
                delete thisCache[name];

                // If we've removed all the data, remove the element's cache
                if (jQuery.isEmptyObject(thisCache)) {
                    jQuery.removeData(elem);
                }
            }

            // Otherwise, we want to remove all of the element's data
        } else {
            if (jQuery.support.deleteExpando) {
                delete elem[jQuery.expando];

            } else if (elem.removeAttribute) {
                elem.removeAttribute(jQuery.expando);
            }

            // Completely remove the data cache
            delete cache[id];
        }
    };
    jQuery.sibling = function (n, elem) {

        var r = [];

        for (; n; n = n.nextSibling) {
            if (n.nodeType === 1 && n !== elem) {
                r.push(n);
            }
        }

        return r;
    };
    jQuery.speed = function (speed, easing, fn) {

        var opt = speed && typeof speed === "object" ? speed : {
            complete: fn || !fn && easing ||
				jQuery.isFunction(speed) && speed,
            duration: speed,
            easing: fn && easing || easing && !jQuery.isFunction(easing) && easing
        };

        opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
			jQuery.fx.speeds[opt.duration] || jQuery.fx.speeds._default;

        // Queueing
        opt.old = opt.complete;
        opt.complete = function () {
            if (opt.queue !== false) {
                jQuery(this).dequeue();
            }
            if (jQuery.isFunction(opt.old)) {
                opt.old.call(this);
            }
        };

        return opt;
    };
    jQuery.style = function (elem, name, value) {

        // don't set styles on text and comment nodes
        if (!elem || elem.nodeType === 3 || elem.nodeType === 8) {
            return undefined;
        }

        // ignore negative width and height values #1599
        if ((name === "width" || name === "height") && parseFloat(value) < 0) {
            value = undefined;
        }

        var style = elem.style || elem, set = value !== undefined;

        // IE uses filters for opacity
        if (!jQuery.support.opacity && name === "opacity") {
            if (set) {
                // IE has trouble with opacity if it does not have layout
                // Force it by setting the zoom level
                style.zoom = 1;

                // Set the alpha filter to set the opacity
                var opacity = parseInt(value, 10) + "" === "NaN" ? "" : "alpha(opacity=" + value * 100 + ")";
                var filter = style.filter || jQuery.curCSS(elem, "filter") || "";
                style.filter = ralpha.test(filter) ? filter.replace(ralpha, opacity) : opacity;
            }

            return style.filter && style.filter.indexOf("opacity=") >= 0 ?
				(parseFloat(ropacity.exec(style.filter)[1]) / 100) + "" :
				"";
        }

        // Make sure we're using the right name for getting the float value
        if (rfloat.test(name)) {
            name = styleFloat;
        }

        name = name.replace(rdashAlpha, fcamelCase);

        if (set) {
            style[name] = value;
        }

        return style[name];
    };
    jQuery.swap = function (elem, options, callback) {

        var old = {};

        // Remember the old values, and insert the new ones
        for (var name in options) {
            old[name] = elem.style[name];
            elem.style[name] = options[name];
        }

        callback.call(elem);

        // Revert the old values
        for (var name in options) {
            elem.style[name] = old[name];
        }
    };
    jQuery.text = function getText(elems) {

        var ret = "", elem;

        for (var i = 0; elems[i]; i++) {
            elem = elems[i];

            // Get the text from text nodes and CDATA nodes
            if (elem.nodeType === 3 || elem.nodeType === 4) {
                ret += elem.nodeValue;

                // Traverse everything else, except comment nodes
            } else if (elem.nodeType !== 8) {
                ret += getText(elem.childNodes);
            }
        }

        return ret;
    };
    jQuery.trim = function (text) {
        /// <summary>Remove the whitespace from the beginning and end of a string.
        /// </summary>
        /// <returns type="String" />
        /// <param name="text" type="String">The string to trim.</param>

        return (text || "").replace(rtrim, "");
    };
    jQuery.uaMatch = function (ua) {

        ua = ua.toLowerCase();

        var match = /(webkit)[ \/]([\w.]+)/.exec(ua) ||
			/(opera)(?:.*version)?[ \/]([\w.]+)/.exec(ua) ||
			/(msie) ([\w.]+)/.exec(ua) ||
			!/compatible/.test(ua) && /(mozilla)(?:.*? rv:([\w.]+))?/.exec(ua) ||
		  	[];

        return { browser: match[1] || "", version: match[2] || "0" };
    };
    jQuery.unique = function (results) {
        /// <summary>Sorts an array of DOM elements, in place, with the duplicates removed. Note that this only works on arrays of DOM elements, not strings or numbers.
        /// </summary>
        /// <returns type="Array" />
        /// <param name="results" type="Array">The Array of DOM elements.</param>

        if (sortOrder) {
            hasDuplicate = baseHasDuplicate;
            results.sort(sortOrder);

            if (hasDuplicate) {
                for (var i = 1; i < results.length; i++) {
                    if (results[i] === results[i - 1]) {
                        results.splice(i--, 1);
                    }
                }
            }
        }

        return results;
    };
    jQuery.prototype._toggle = function (fn) {

        // Save reference to arguments for access in closure
        var args = arguments, i = 1;

        // link all the functions, so any of them can unbind this click handler
        while (i < args.length) {
            jQuery.proxy(fn, args[i++]);
        }

        return this.click(jQuery.proxy(fn, function (event) {
            // Figure out which function to execute
            var lastToggle = (jQuery.data(this, "lastToggle" + fn.guid) || 0) % i;
            jQuery.data(this, "lastToggle" + fn.guid, lastToggle + 1);

            // Make sure that clicks stop
            event.preventDefault();

            // and execute the function
            return args[lastToggle].apply(this, arguments) || false;
        }));
    };
    jQuery.prototype.add = function (selector, context) {
        /// <summary>Add elements to the set of matched elements.
        /// 1 - add(selector) 
        /// 2 - add(elements) 
        /// 3 - add(html) 
        /// 4 - add(selector, context)</summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A string containing a selector expression to match additional elements against.</param>
        /// <param name="context" domElement="true">Add some elements rooted against the specified context.</param>

        var set = typeof selector === "string" ?
				jQuery(selector, context || this.context) :
				jQuery.makeArray(selector),
			all = jQuery.merge(this.get(), set);

        return this.pushStack(isDisconnected(set[0]) || isDisconnected(all[0]) ?
			all :
			jQuery.unique(all));
    };
    jQuery.prototype.addClass = function (value) {
        /// <summary>Adds the specified class(es) to each of the set of matched elements.
        /// 1 - addClass(className) 
        /// 2 - addClass(function(index, class))</summary>
        /// <returns type="jQuery" />
        /// <param name="value" type="String">One or more class names to be added to the class attribute of each matched element.</param>

        if (jQuery.isFunction(value)) {
            return this.each(function (i) {
                var self = jQuery(this);
                self.addClass(value.call(this, i, self.attr("class")));
            });
        }

        if (value && typeof value === "string") {
            var classNames = (value || "").split(rspace);

            for (var i = 0, l = this.length; i < l; i++) {
                var elem = this[i];

                if (elem.nodeType === 1) {
                    if (!elem.className) {
                        elem.className = value;

                    } else {
                        var className = " " + elem.className + " ", setClass = elem.className;
                        for (var c = 0, cl = classNames.length; c < cl; c++) {
                            if (className.indexOf(" " + classNames[c] + " ") < 0) {
                                setClass += " " + classNames[c];
                            }
                        }
                        elem.className = jQuery.trim(setClass);
                    }
                }
            }
        }

        return this;
    };
    jQuery.prototype.after = function () {
        /// <summary>Insert content, specified by the parameter, after each element in the set of matched elements.
        /// 1 - after(content) 
        /// 2 - after(function(index))</summary>
        /// <returns type="jQuery" />
        /// <param name="" type="jQuery">An element, HTML string, or jQuery object to insert after each element in the set of matched elements.</param>

        if (this[0] && this[0].parentNode) {
            return this.domManip(arguments, false, function (elem) {
                this.parentNode.insertBefore(elem, this.nextSibling);
            });
        } else if (arguments.length) {
            var set = this.pushStack(this, "after", arguments);
            set.push.apply(set, jQuery(arguments[0]).toArray());
            return set;
        }
    };
    jQuery.prototype.ajaxComplete = function (f) {
        /// <summary>
        ///         Register a handler to be called when Ajax requests complete. This is an Ajax Event.
        ///       
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="f" type="Function">The function to be invoked.</param>

        return this.bind(o, f);
    };
    jQuery.prototype.ajaxError = function (f) {
        /// <summary>
        ///         Register a handler to be called when Ajax requests complete with an error. This is an Ajax Event.
        ///       
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="f" type="Function">The function to be invoked.</param>

        return this.bind(o, f);
    };
    jQuery.prototype.ajaxSend = function (f) {
        /// <summary>
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="f" type="Function">The function to be invoked.</param>

        return this.bind(o, f);
    };
    jQuery.prototype.ajaxStart = function (f) {
        /// <summary>
        ///         Register a handler to be called when the first Ajax request begins. This is an Ajax Event.
        ///       
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="f" type="Function">The function to be invoked.</param>

        return this.bind(o, f);
    };
    jQuery.prototype.ajaxStop = function (f) {
        /// <summary>
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="f" type="Function">The function to be invoked.</param>

        return this.bind(o, f);
    };
    jQuery.prototype.ajaxSuccess = function (f) {
        /// <summary>
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="f" type="Function">The function to be invoked.</param>

        return this.bind(o, f);
    };
    jQuery.prototype.andSelf = function () {
        /// <summary>Add the previous set of elements on the stack to the current set.
        /// </summary>
        /// <returns type="jQuery" />

        return this.add(this.prevObject);
    };
    jQuery.prototype.animate = function (prop, speed, easing, callback) {
        /// <summary>Perform a custom animation of a set of CSS properties.
        /// 1 - animate(properties, duration, easing, callback) 
        /// 2 - animate(properties, options)</summary>
        /// <returns type="jQuery" />
        /// <param name="prop" type="Object">A map of CSS properties that the animation will move toward.</param>
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="easing" type="String">A string indicating which easing function to use for the transition.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        var optall = jQuery.speed(speed, easing, callback);

        if (jQuery.isEmptyObject(prop)) {
            return this.each(optall.complete);
        }

        return this[optall.queue === false ? "each" : "queue"](function () {
            var opt = jQuery.extend({}, optall), p,
				hidden = this.nodeType === 1 && jQuery(this).is(":hidden"),
				self = this;

            for (p in prop) {
                var name = p.replace(rdashAlpha, fcamelCase);

                if (p !== name) {
                    prop[name] = prop[p];
                    delete prop[p];
                    p = name;
                }

                if (prop[p] === "hide" && hidden || prop[p] === "show" && !hidden) {
                    return opt.complete.call(this);
                }

                if ((p === "height" || p === "width") && this.style) {
                    // Store display property
                    opt.display = jQuery.css(this, "display");

                    // Make sure that nothing sneaks out
                    opt.overflow = this.style.overflow;
                }

                if (jQuery.isArray(prop[p])) {
                    // Create (if needed) and add to specialEasing
                    (opt.specialEasing = opt.specialEasing || {})[p] = prop[p][1];
                    prop[p] = prop[p][0];
                }
            }

            if (opt.overflow != null) {
                this.style.overflow = "hidden";
            }

            opt.curAnim = jQuery.extend({}, prop);

            jQuery.each(prop, function (name, val) {
                var e = new jQuery.fx(self, opt, name);

                if (rfxtypes.test(val)) {
                    e[val === "toggle" ? hidden ? "show" : "hide" : val](prop);

                } else {
                    var parts = rfxnum.exec(val),
						start = e.cur(true) || 0;

                    if (parts) {
                        var end = parseFloat(parts[2]),
							unit = parts[3] || "px";

                        // We need to compute starting value
                        if (unit !== "px") {
                            self.style[name] = (end || 1) + unit;
                            start = ((end || 1) / e.cur(true)) * start;
                            self.style[name] = start + unit;
                        }

                        // If a +=/-= token was provided, we're doing a relative animation
                        if (parts[1]) {
                            end = ((parts[1] === "-=" ? -1 : 1) * end) + start;
                        }

                        e.custom(start, end, unit);

                    } else {
                        e.custom(start, val, "");
                    }
                }
            });

            // For JS strict compliance
            return true;
        });
    };
    jQuery.prototype.append = function () {
        /// <summary>Insert content, specified by the parameter, to the end of each element in the set of matched elements.
        /// 1 - append(content) 
        /// 2 - append(function(index, html))</summary>
        /// <returns type="jQuery" />
        /// <param name="" type="jQuery">An element, HTML string, or jQuery object to insert at the end of each element in the set of matched elements.</param>

        return this.domManip(arguments, true, function (elem) {
            if (this.nodeType === 1) {
                this.appendChild(elem);
            }
        });
    };
    jQuery.prototype.appendTo = function (selector) {
        /// <summary>Insert every element in the set of matched elements to the end of the target.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="jQuery">A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted at the end of the element(s) specified by this parameter.</param>

        var ret = [], insert = jQuery(selector),
			parent = this.length === 1 && this[0].parentNode;

        if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
            insert[original](this[0]);
            return this;

        } else {
            for (var i = 0, l = insert.length; i < l; i++) {
                var elems = (i > 0 ? this.clone(true) : this).get();
                jQuery.fn[original].apply(jQuery(insert[i]), elems);
                ret = ret.concat(elems);
            }

            return this.pushStack(ret, name, insert.selector);
        }
    };
    jQuery.prototype.attr = function (name, value) {
        /// <summary>1: Get the value of an attribute for the first element in the set of matched elements.
        /// 1.1 - attr(attributeName)
        /// 2: Set one or more attributes for the set of matched elements.
        /// 2.1 - attr(attributeName, value) 
        /// 2.2 - attr(map) 
        /// 2.3 - attr(attributeName, function(index, attr))</summary>
        /// <returns type="jQuery" />
        /// <param name="name" type="String">The name of the attribute to set.</param>
        /// <param name="value" type="Object">A value to set for the attribute.</param>

        return access(this, name, value, true, jQuery.attr);
    };
    jQuery.prototype.before = function () {
        /// <summary>Insert content, specified by the parameter, before each element in the set of matched elements.
        /// 1 - before(content) 
        /// 2 - before(function)</summary>
        /// <returns type="jQuery" />
        /// <param name="" type="jQuery">An element, HTML string, or jQuery object to insert before each element in the set of matched elements.</param>

        if (this[0] && this[0].parentNode) {
            return this.domManip(arguments, false, function (elem) {
                this.parentNode.insertBefore(elem, this);
            });
        } else if (arguments.length) {
            var set = jQuery(arguments[0]);
            set.push.apply(set, this.toArray());
            return this.pushStack(set, "before", arguments);
        }
    };
    jQuery.prototype.bind = function (type, data, fn) {
        /// <summary>Attach a handler to an event for the elements.
        /// 1 - bind(eventType, eventData, handler(eventObject)) 
        /// 2 - bind(events)</summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">A string containing one or more JavaScript event types, such as "click" or "submit," or custom event names.</param>
        /// <param name="data" type="Object">A map of data that will be passed to the event handler.</param>
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        // Handle object literals
        if (typeof type === "object") {
            for (var key in type) {
                this[name](key, data, type[key], fn);
            }
            return this;
        }

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        var handler = name === "one" ? jQuery.proxy(fn, function (event) {
            jQuery(this).unbind(event, handler);
            return fn.apply(this, arguments);
        }) : fn;

        if (type === "unload" && name !== "one") {
            this.one(type, data, fn);

        } else {
            for (var i = 0, l = this.length; i < l; i++) {
                jQuery.event.add(this[i], type, handler, data);
            }
        }

        return this;
    };
    jQuery.prototype.blur = function (fn) {
        /// <summary>Bind an event handler to the "blur" JavaScript event, or trigger that event on an element.
        /// 1 - blur(handler(eventObject)) 
        /// 2 - blur()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.change = function (fn) {
        /// <summary>Bind an event handler to the "change" JavaScript event, or trigger that event on an element.
        /// 1 - change(handler(eventObject)) 
        /// 2 - change()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.children = function (until, selector) {
        /// <summary>Get the children of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.clearQueue = function (type) {
        /// <summary>Remove from the queue all items that have not yet been run.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">             A string containing the name of the queue. Defaults to fx, the standard effects queue.           </param>

        return this.queue(type || "fx", []);
    };
    jQuery.prototype.click = function (fn) {
        /// <summary>Bind an event handler to the "click" JavaScript event, or trigger that event on an element.
        /// 1 - click(handler(eventObject)) 
        /// 2 - click()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.clone = function (events) {
        /// <summary>Create a copy of the set of matched elements.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="events" type="Boolean">A Boolean indicating whether event handlers should be copied along with the elements. As of jQuery 1.4 element data will be copied as well.</param>

        // Do the clone
        var ret = this.map(function () {
            if (!jQuery.support.noCloneEvent && !jQuery.isXMLDoc(this)) {
                // IE copies events bound via attachEvent when
                // using cloneNode. Calling detachEvent on the
                // clone will also remove the events from the orignal
                // In order to get around this, we use innerHTML.
                // Unfortunately, this means some modifications to
                // attributes in IE that are actually only stored
                // as properties will not be copied (such as the
                // the name attribute on an input).
                var html = this.outerHTML, ownerDocument = this.ownerDocument;
                if (!html) {
                    var div = ownerDocument.createElement("div");
                    div.appendChild(this.cloneNode(true));
                    html = div.innerHTML;
                }

                return jQuery.clean([html.replace(rinlinejQuery, "")
                // Handle the case in IE 8 where action=/test/> self-closes a tag
					.replace(/=([^="'>\s]+\/)>/g, '="$1">')
					.replace(rleadingWhitespace, "")], ownerDocument)[0];
            } else {
                return this.cloneNode(true);
            }
        });

        // Copy the events from the original to the clone
        if (events === true) {
            cloneCopyEvent(this, ret);
            cloneCopyEvent(this.find("*"), ret.find("*"));
        }

        // Return the cloned set
        return ret;
    };
    jQuery.prototype.closest = function (selectors, context) {
        /// <summary>1: Get the first ancestor element that matches the selector, beginning at the current element and progressing up through the DOM tree.
        /// 1.1 - closest(selector) 
        /// 1.2 - closest(selector, context)
        /// 2: Gets an array of all the elements and selectors matched against the current element up through the DOM tree.
        /// 2.1 - closest(selectors, context)</summary>
        /// <returns type="jQuery" />
        /// <param name="selectors" type="String">A string containing a selector expression to match elements against.</param>
        /// <param name="context" domElement="true">A DOM element within which a matching element may be found. If no context is passed in then the context of the jQuery set will be used instead.</param>

        if (jQuery.isArray(selectors)) {
            var ret = [], cur = this[0], match, matches = {}, selector;

            if (cur && selectors.length) {
                for (var i = 0, l = selectors.length; i < l; i++) {
                    selector = selectors[i];

                    if (!matches[selector]) {
                        matches[selector] = jQuery.expr.match.POS.test(selector) ?
							jQuery(selector, context || this.context) :
							selector;
                    }
                }

                while (cur && cur.ownerDocument && cur !== context) {
                    for (selector in matches) {
                        match = matches[selector];

                        if (match.jquery ? match.index(cur) > -1 : jQuery(cur).is(match)) {
                            ret.push({ selector: selector, elem: cur });
                            delete matches[selector];
                        }
                    }
                    cur = cur.parentNode;
                }
            }

            return ret;
        }

        var pos = jQuery.expr.match.POS.test(selectors) ?
			jQuery(selectors, context || this.context) : null;

        return this.map(function (i, cur) {
            while (cur && cur.ownerDocument && cur !== context) {
                if (pos ? pos.index(cur) > -1 : jQuery(cur).is(selectors)) {
                    return cur;
                }
                cur = cur.parentNode;
            }
            return null;
        });
    };
    jQuery.prototype.contents = function (until, selector) {
        /// <summary>Get the children of each element in the set of matched elements, including text nodes.
        /// </summary>
        /// <returns type="jQuery" />

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.css = function (name, value) {
        /// <summary>1: Get the value of a style property for the first element in the set of matched elements.
        /// 1.1 - css(propertyName)
        /// 2: Set one or more CSS properties for the  set of matched elements.
        /// 2.1 - css(propertyName, value) 
        /// 2.2 - css(propertyName, function(index, value)) 
        /// 2.3 - css(map)</summary>
        /// <returns type="jQuery" />
        /// <param name="name" type="String">A CSS property name.</param>
        /// <param name="value" type="Number">A value to set for the property.</param>

        return access(this, name, value, true, function (elem, name, value) {
            if (value === undefined) {
                return jQuery.curCSS(elem, name);
            }

            if (typeof value === "number" && !rexclude.test(name)) {
                value += "px";
            }

            jQuery.style(elem, name, value);
        });
    };
    jQuery.prototype.data = function (key, value) {
        /// <summary>1: Store arbitrary data associated with the matched elements.
        /// 1.1 - data(key, value) 
        /// 1.2 - data(obj)
        /// 2: Returns value at named data store for the element, as set by data(name, value).
        /// 2.1 - data(key) 
        /// 2.2 - data()</summary>
        /// <returns type="jQuery" />
        /// <param name="key" type="String">A string naming the piece of data to set.</param>
        /// <param name="value" type="Object">The new data value; it can be any Javascript type including Array or Object.</param>

        if (typeof key === "undefined" && this.length) {
            return jQuery.data(this[0]);

        } else if (typeof key === "object") {
            return this.each(function () {
                jQuery.data(this, key);
            });
        }

        var parts = key.split(".");
        parts[1] = parts[1] ? "." + parts[1] : "";

        if (value === undefined) {
            var data = this.triggerHandler("getData" + parts[1] + "!", [parts[0]]);

            if (data === undefined && this.length) {
                data = jQuery.data(this[0], key);
            }
            return data === undefined && parts[1] ?
				this.data(parts[0]) :
				data;
        } else {
            return this.trigger("setData" + parts[1] + "!", [parts[0], value]).each(function () {
                jQuery.data(this, key, value);
            });
        }
    };
    jQuery.prototype.dblclick = function (fn) {
        /// <summary>Bind an event handler to the "dblclick" JavaScript event, or trigger that event on an element.
        /// 1 - dblclick(handler(eventObject)) 
        /// 2 - dblclick()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.delay = function (time, type) {
        /// <summary>Set a timer to delay execution of subsequent items in the queue.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="time" type="Number">An integer indicating the number of milliseconds to delay execution of the next item in the queue.</param>
        /// <param name="type" type="String">             A string containing the name of the queue. Defaults to fx, the standard effects queue.           </param>

        time = jQuery.fx ? jQuery.fx.speeds[time] || time : time;
        type = type || "fx";

        return this.queue(type, function () {
            var elem = this;
            setTimeout(function () {
                jQuery.dequeue(elem, type);
            }, time);
        });
    };
    jQuery.prototype.delegate = function (selector, types, data, fn) {
        /// <summary>Attach a handler to one or more events for all elements that match the selector, now or in the future, based on a specific set of root elements.
        /// 1 - delegate(selector, eventType, handler) 
        /// 2 - delegate(selector, eventType, eventData, handler)</summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A selector to filter the elements that trigger the event.</param>
        /// <param name="types" type="String">A string containing one or more space-separated JavaScript event types, such as "click" or "keydown," or custom event names.</param>
        /// <param name="data" type="Object">A map of data that will be passed to the event handler.</param>
        /// <param name="fn" type="Function">A function to execute at the time the event is triggered.</param>

        return this.live(types, data, fn, selector);
    };
    jQuery.prototype.dequeue = function (type) {
        /// <summary>Execute the next function on the queue for the matched elements.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">             A string containing the name of the queue. Defaults to fx, the standard effects queue.           </param>

        return this.each(function () {
            jQuery.dequeue(this, type);
        });
    };
    jQuery.prototype.detach = function (selector) {
        /// <summary>Remove the set of matched elements from the DOM.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A selector expression that filters the set of matched elements to be removed.</param>

        return this.remove(selector, true);
    };
    jQuery.prototype.die = function (types, data, fn, origSelector /* Internal Use Only */) {
        /// <summary>1: 
        ///         Remove all event handlers previously attached using .live() from the elements.
        ///       
        /// 1.1 - die()
        /// 2: 
        ///         Remove an event handler previously attached using .live() from the elements.
        ///       
        /// 2.1 - die(eventType, handler)</summary>
        /// <returns type="jQuery" />
        /// <param name="types" type="String">             A string containing a JavaScript event type, such as click or keydown.           </param>
        /// <param name="data" type="String">The function that is to be no longer executed.</param>

        var type, i = 0, match, namespaces, preType,
			selector = origSelector || this.selector,
			context = origSelector ? this : jQuery(this.context);

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        types = (types || "").split(" ");

        while ((type = types[i++]) != null) {
            match = rnamespaces.exec(type);
            namespaces = "";

            if (match) {
                namespaces = match[0];
                type = type.replace(rnamespaces, "");
            }

            if (type === "hover") {
                types.push("mouseenter" + namespaces, "mouseleave" + namespaces);
                continue;
            }

            preType = type;

            if (type === "focus" || type === "blur") {
                types.push(liveMap[type] + namespaces);
                type = type + namespaces;

            } else {
                type = (liveMap[type] || type) + namespaces;
            }

            if (name === "live") {
                // bind live handler
                context.each(function () {
                    jQuery.event.add(this, liveConvert(type, selector),
						{ data: data, selector: selector, handler: fn, origType: type, origHandler: fn, preType: preType });
                });

            } else {
                // unbind live handler
                context.unbind(liveConvert(type, selector), fn);
            }
        }

        return this;
    };
    jQuery.prototype.domManip = function (args, table, callback) {

        var results, first, value = args[0], scripts = [], fragment, parent;

        // We can't cloneNode fragments that contain checked, in WebKit
        if (!jQuery.support.checkClone && arguments.length === 3 && typeof value === "string" && rchecked.test(value)) {
            return this.each(function () {
                jQuery(this).domManip(args, table, callback, true);
            });
        }

        if (jQuery.isFunction(value)) {
            return this.each(function (i) {
                var self = jQuery(this);
                args[0] = value.call(this, i, table ? self.html() : undefined);
                self.domManip(args, table, callback);
            });
        }

        if (this[0]) {
            parent = value && value.parentNode;

            // If we're in a fragment, just use that instead of building a new one
            if (jQuery.support.parentNode && parent && parent.nodeType === 11 && parent.childNodes.length === this.length) {
                results = { fragment: parent };

            } else {
                results = buildFragment(args, this, scripts);
            }

            fragment = results.fragment;

            if (fragment.childNodes.length === 1) {
                first = fragment = fragment.firstChild;
            } else {
                first = fragment.firstChild;
            }

            if (first) {
                table = table && jQuery.nodeName(first, "tr");

                for (var i = 0, l = this.length; i < l; i++) {
                    callback.call(
						table ?
							root(this[i], first) :
							this[i],
						i > 0 || results.cacheable || this.length > 1 ?
							fragment.cloneNode(true) :
							fragment
					);
                }
            }

            if (scripts.length) {
                jQuery.each(scripts, evalScript);
            }
        }

        return this;

        function root(elem, cur) {
            return jQuery.nodeName(elem, "table") ?
				(elem.getElementsByTagName("tbody")[0] ||
				elem.appendChild(elem.ownerDocument.createElement("tbody"))) :
				elem;
        }
    };
    jQuery.prototype.each = function (callback, args) {
        /// <summary>Iterate over a jQuery object, executing a function for each matched element. 
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="callback" type="Function">A function to execute for each matched element.</param>

        return jQuery.each(this, callback, args);
    };
    jQuery.prototype.empty = function () {
        /// <summary>Remove all child nodes of the set of matched elements from the DOM.
        /// </summary>
        /// <returns type="jQuery" />

        for (var i = 0, elem; (elem = this[i]) != null; i++) {
            // Remove element nodes and prevent memory leaks
            if (elem.nodeType === 1) {
                jQuery.cleanData(elem.getElementsByTagName("*"));
            }

            // Remove any remaining nodes
            while (elem.firstChild) {
                elem.removeChild(elem.firstChild);
            }
        }

        return this;
    };
    jQuery.prototype.end = function () {
        /// <summary>End the most recent filtering operation in the current chain and return the set of matched elements to its previous state.
        /// </summary>
        /// <returns type="jQuery" />

        return this.prevObject || jQuery(null);
    };
    jQuery.prototype.eq = function (i) {
        /// <summary>Reduce the set of matched elements to the one at the specified index.
        /// 1 - eq(index) 
        /// 2 - eq(-index)</summary>
        /// <returns type="jQuery" />
        /// <param name="i" type="Number">An integer indicating the 0-based position of the element. </param>

        return i === -1 ?
			this.slice(i) :
			this.slice(i, +i + 1);
    };
    jQuery.prototype.error = function (fn) {
        /// <summary>Bind an event handler to the "error" JavaScript event.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute when the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.extend = function () {

        // copy reference to target object
        var target = arguments[0] || {}, i = 1, length = arguments.length, deep = false, options, name, src, copy;

        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }

        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !jQuery.isFunction(target)) {
            target = {};
        }

        // extend jQuery itself if only one argument is passed
        if (length === i) {
            target = this;
            --i;
        }

        for (; i < length; i++) {
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null) {
                // Extend the base object
                for (name in options) {
                    src = target[name];
                    copy = options[name];

                    // Prevent never-ending loop
                    if (target === copy) {
                        continue;
                    }

                    // Recurse if we're merging object literal values or arrays
                    if (deep && copy && (jQuery.isPlainObject(copy) || jQuery.isArray(copy))) {
                        var clone = src && (jQuery.isPlainObject(src) || jQuery.isArray(src)) ? src
						: jQuery.isArray(copy) ? [] : {};

                        // Never move original objects, clone them
                        target[name] = jQuery.extend(deep, clone, copy);

                        // Don't bring in undefined values
                    } else if (copy !== undefined) {
                        target[name] = copy;
                    }
                }
            }
        }

        // Return the modified object
        return target;
    };
    jQuery.prototype.fadeIn = function (speed, callback) {
        /// <summary>Display the matched elements by fading them to opaque.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        return this.animate(props, speed, callback);
    };
    jQuery.prototype.fadeOut = function (speed, callback) {
        /// <summary>Hide the matched elements by fading them to transparent.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        return this.animate(props, speed, callback);
    };
    jQuery.prototype.fadeTo = function (speed, to, callback) {
        /// <summary>Adjust the opacity of the matched elements.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="to" type="Number">A number between 0 and 1 denoting the target opacity.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        return this.filter(":hidden").css("opacity", 0).show().end()
					.animate({ opacity: to }, speed, callback);
    };
    jQuery.prototype.filter = function (selector) {
        /// <summary>Reduce the set of matched elements to those that match the selector or pass the function's test. 
        /// 1 - filter(selector) 
        /// 2 - filter(function(index))</summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A string containing a selector expression to match elements against.</param>

        return this.pushStack(winnow(this, selector, true), "filter", selector);
    };
    jQuery.prototype.find = function (selector) {
        /// <summary>Get the descendants of each element in the current set of matched elements, filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A string containing a selector expression to match elements against.</param>

        var ret = this.pushStack("", "find", selector), length = 0;

        for (var i = 0, l = this.length; i < l; i++) {
            length = ret.length;
            jQuery.find(selector, this[i], ret);

            if (i > 0) {
                // Make sure that the results are unique
                for (var n = length; n < ret.length; n++) {
                    for (var r = 0; r < length; r++) {
                        if (ret[r] === ret[n]) {
                            ret.splice(n--, 1);
                            break;
                        }
                    }
                }
            }
        }

        return ret;
    };
    jQuery.prototype.first = function () {
        /// <summary>Reduce the set of matched elements to the first in the set.
        /// </summary>
        /// <returns type="jQuery" />

        return this.eq(0);
    };
    jQuery.prototype.focus = function (fn) {
        /// <summary>Bind an event handler to the "focus" JavaScript event, or trigger that event on an element.
        /// 1 - focus(handler(eventObject)) 
        /// 2 - focus()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.focusin = function (fn) {
        /// <summary>Bind an event handler to the "focusin" JavaScript event.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.focusout = function (fn) {
        /// <summary>Bind an event handler to the "focusout" JavaScript event.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.get = function (num) {
        /// <summary>Retrieve the DOM elements matched by the jQuery object.
        /// </summary>
        /// <returns type="Array" />
        /// <param name="num" type="Number">A zero-based integer indicating which element to retrieve.</param>

        return num == null ?

        // Return a 'clean' array
			this.toArray() :

        // Return just the object
			(num < 0 ? this.slice(num)[0] : this[num]);
    };
    jQuery.prototype.has = function (target) {
        /// <summary>Reduce the set of matched elements to those that have a descendant that matches the selector or DOM element.
        /// 1 - has(selector) 
        /// 2 - has(contained)</summary>
        /// <returns type="jQuery" />
        /// <param name="target" type="String">A string containing a selector expression to match elements against.</param>

        var targets = jQuery(target);
        return this.filter(function () {
            for (var i = 0, l = targets.length; i < l; i++) {
                if (jQuery.contains(this, targets[i])) {
                    return true;
                }
            }
        });
    };
    jQuery.prototype.hasClass = function (selector) {
        /// <summary>Determine whether any of the matched elements are assigned the given class.
        /// </summary>
        /// <returns type="Boolean" />
        /// <param name="selector" type="String">The class name to search for.</param>

        var className = " " + selector + " ";
        for (var i = 0, l = this.length; i < l; i++) {
            if ((" " + this[i].className + " ").replace(rclass, " ").indexOf(className) > -1) {
                return true;
            }
        }

        return false;
    };
    jQuery.prototype.height = function (size) {
        /// <summary>1: Get the current computed height for the first element in the set of matched elements.
        /// 1.1 - height()
        /// 2: Set the CSS height of every matched element.
        /// 2.1 - height(value) 
        /// 2.2 - height(function(index, height))</summary>
        /// <returns type="jQuery" />
        /// <param name="size" type="Number">An integer representing the number of pixels, or an integer with an optional unit of measure appended (as a string).</param>

        // Get window width or height
        var elem = this[0];
        if (!elem) {
            return size == null ? null : this;
        }

        if (jQuery.isFunction(size)) {
            return this.each(function (i) {
                var self = jQuery(this);
                self[type](size.call(this, i, self[type]()));
            });
        }

        return ("scrollTo" in elem && elem.document) ? // does it walk and quack like a window?
        // Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
			elem.document.compatMode === "CSS1Compat" && elem.document.documentElement["client" + name] ||
			elem.document.body["client" + name] :

        // Get document width or height
			(elem.nodeType === 9) ? // is it a document
        // Either scroll[Width/Height] or offset[Width/Height], whichever is greater
				Math.max(
					elem.documentElement["client" + name],
					elem.body["scroll" + name], elem.documentElement["scroll" + name],
					elem.body["offset" + name], elem.documentElement["offset" + name]
				) :

        // Get or set width or height on the element
				size === undefined ?
        // Get width or height on the element
					jQuery.css(elem, type) :

        // Set the width or height on the element (default to pixels if value is unitless)
					this.css(type, typeof size === "string" ? size : size + "px");
    };
    jQuery.prototype.hide = function (speed, callback) {
        /// <summary>Hide the matched elements.
        /// 1 - hide() 
        /// 2 - hide(duration, callback)</summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        if (speed || speed === 0) {
            return this.animate(genFx("hide", 3), speed, callback);

        } else {
            for (var i = 0, l = this.length; i < l; i++) {
                var old = jQuery.data(this[i], "olddisplay");
                if (!old && old !== "none") {
                    jQuery.data(this[i], "olddisplay", jQuery.css(this[i], "display"));
                }
            }

            // Set the display of the elements in a second loop
            // to avoid the constant reflow
            for (var j = 0, k = this.length; j < k; j++) {
                this[j].style.display = "none";
            }

            return this;
        }
    };
    jQuery.prototype.hover = function (fnOver, fnOut) {
        /// <summary>1: Bind two handlers to the matched elements, to be executed when the mouse pointer enters and leaves the elements.
        /// 1.1 - hover(handlerIn(eventObject), handlerOut(eventObject))
        /// 2: Bind a single handler to the matched elements, to be executed when the mouse pointer enters or leaves the elements.
        /// 2.1 - hover(handlerInOut(eventObject))</summary>
        /// <returns type="jQuery" />
        /// <param name="fnOver" type="Function">A function to execute when the mouse pointer enters the element.</param>
        /// <param name="fnOut" type="Function">A function to execute when the mouse pointer leaves the element.</param>

        return this.mouseenter(fnOver).mouseleave(fnOut || fnOver);
    };
    jQuery.prototype.html = function (value) {
        /// <summary>1: Get the HTML contents of the first element in the set of matched elements.
        /// 1.1 - html()
        /// 2: Set the HTML contents of each element in the set of matched elements.
        /// 2.1 - html(htmlString) 
        /// 2.2 - html(function(index, html))</summary>
        /// <returns type="jQuery" />
        /// <param name="value" type="String">A string of HTML to set as the content of each matched element.</param>

        if (value === undefined) {
            return this[0] && this[0].nodeType === 1 ?
				this[0].innerHTML.replace(rinlinejQuery, "") :
				null;

            // See if we can take a shortcut and just use innerHTML
        } else if (typeof value === "string" && !rnocache.test(value) &&
			(jQuery.support.leadingWhitespace || !rleadingWhitespace.test(value)) &&
			!wrapMap[(rtagName.exec(value) || ["", ""])[1].toLowerCase()]) {

            value = value.replace(rxhtmlTag, fcloseTag);

            try {
                for (var i = 0, l = this.length; i < l; i++) {
                    // Remove element nodes and prevent memory leaks
                    if (this[i].nodeType === 1) {
                        jQuery.cleanData(this[i].getElementsByTagName("*"));
                        this[i].innerHTML = value;
                    }
                }

                // If using innerHTML throws an exception, use the fallback method
            } catch (e) {
                this.empty().append(value);
            }

        } else if (jQuery.isFunction(value)) {
            this.each(function (i) {
                var self = jQuery(this), old = self.html();
                self.empty().append(function () {
                    return value.call(this, i, old);
                });
            });

        } else {
            this.empty().append(value);
        }

        return this;
    };
    jQuery.prototype.index = function (elem) {
        /// <summary>Search for a given element from among the matched elements.
        /// 1 - index() 
        /// 2 - index(selector) 
        /// 3 - index(element)</summary>
        /// <returns type="Number" />
        /// <param name="elem" type="String">A selector representing a jQuery collection in which to look for an element.</param>

        if (!elem || typeof elem === "string") {
            return jQuery.inArray(this[0],
            // If it receives a string, the selector is used
            // If it receives nothing, the siblings are used
				elem ? jQuery(elem) : this.parent().children());
        }
        // Locate the position of the desired element
        return jQuery.inArray(
        // If it receives a jQuery object, the first element is used
			elem.jquery ? elem[0] : elem, this);
    };
    jQuery.prototype.init = function (selector, context) {

        var match, elem, ret, doc;

        // Handle $(""), $(null), or $(undefined)
        if (!selector) {
            return this;
        }

        // Handle $(DOMElement)
        if (selector.nodeType) {
            this.context = this[0] = selector;
            this.length = 1;
            return this;
        }

        // The body element only exists once, optimize finding it
        if (selector === "body" && !context) {
            this.context = document;
            this[0] = document.body;
            this.selector = "body";
            this.length = 1;
            return this;
        }

        // Handle HTML strings
        if (typeof selector === "string") {
            // Are we dealing with HTML string or an ID?
            match = quickExpr.exec(selector);

            // Verify a match, and that no context was specified for #id
            if (match && (match[1] || !context)) {

                // HANDLE: $(html) -> $(array)
                if (match[1]) {
                    doc = (context ? context.ownerDocument || context : document);

                    // If a single string is passed in and it's a single tag
                    // just do a createElement and skip the rest
                    ret = rsingleTag.exec(selector);

                    if (ret) {
                        if (jQuery.isPlainObject(context)) {
                            selector = [document.createElement(ret[1])];
                            jQuery.fn.attr.call(selector, context, true);

                        } else {
                            selector = [doc.createElement(ret[1])];
                        }

                    } else {
                        ret = buildFragment([match[1]], [doc]);
                        selector = (ret.cacheable ? ret.fragment.cloneNode(true) : ret.fragment).childNodes;
                    }

                    return jQuery.merge(this, selector);

                    // HANDLE: $("#id")
                } else {
                    elem = document.getElementById(match[2]);

                    if (elem) {
                        // Handle the case where IE and Opera return items
                        // by name instead of ID
                        if (elem.id !== match[2]) {
                            return rootjQuery.find(selector);
                        }

                        // Otherwise, we inject the element directly into the jQuery object
                        this.length = 1;
                        this[0] = elem;
                    }

                    this.context = document;
                    this.selector = selector;
                    return this;
                }

                // HANDLE: $("TAG")
            } else if (!context && /^\w+$/.test(selector)) {
                this.selector = selector;
                this.context = document;
                selector = document.getElementsByTagName(selector);
                return jQuery.merge(this, selector);

                // HANDLE: $(expr, $(...))
            } else if (!context || context.jquery) {
                return (context || rootjQuery).find(selector);

                // HANDLE: $(expr, context)
                // (which is just equivalent to: $(context).find(expr)
            } else {
                return jQuery(context).find(selector);
            }

            // HANDLE: $(function)
            // Shortcut for document ready
        } else if (jQuery.isFunction(selector)) {
            return rootjQuery.ready(selector);
        }

        if (selector.selector !== undefined) {
            this.selector = selector.selector;
            this.context = selector.context;
        }

        return jQuery.makeArray(selector, this);
    };
    jQuery.prototype.innerHeight = function () {
        /// <summary>Get the current computed height for the first element in the set of matched elements, including padding but not border.
        /// </summary>
        /// <returns type="Number" />

        return this[0] ?
			jQuery.css(this[0], type, false, "padding") :
			null;
    };
    jQuery.prototype.innerWidth = function () {
        /// <summary>Get the current computed width for the first element in the set of matched elements, including padding but not border.
        /// </summary>
        /// <returns type="Number" />

        return this[0] ?
			jQuery.css(this[0], type, false, "padding") :
			null;
    };
    jQuery.prototype.insertAfter = function (selector) {
        /// <summary>Insert every element in the set of matched elements after the target.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="jQuery">A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted after the element(s) specified by this parameter.</param>

        var ret = [], insert = jQuery(selector),
			parent = this.length === 1 && this[0].parentNode;

        if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
            insert[original](this[0]);
            return this;

        } else {
            for (var i = 0, l = insert.length; i < l; i++) {
                var elems = (i > 0 ? this.clone(true) : this).get();
                jQuery.fn[original].apply(jQuery(insert[i]), elems);
                ret = ret.concat(elems);
            }

            return this.pushStack(ret, name, insert.selector);
        }
    };
    jQuery.prototype.insertBefore = function (selector) {
        /// <summary>Insert every element in the set of matched elements before the target.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="jQuery">A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted before the element(s) specified by this parameter.</param>

        var ret = [], insert = jQuery(selector),
			parent = this.length === 1 && this[0].parentNode;

        if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
            insert[original](this[0]);
            return this;

        } else {
            for (var i = 0, l = insert.length; i < l; i++) {
                var elems = (i > 0 ? this.clone(true) : this).get();
                jQuery.fn[original].apply(jQuery(insert[i]), elems);
                ret = ret.concat(elems);
            }

            return this.pushStack(ret, name, insert.selector);
        }
    };
    jQuery.prototype.is = function (selector) {
        /// <summary>Check the current matched set of elements against a selector and return true if at least one of these elements matches the selector.
        /// </summary>
        /// <returns type="Boolean" />
        /// <param name="selector" type="String">A string containing a selector expression to match elements against.</param>

        return !!selector && jQuery.filter(selector, this).length > 0;
    };
    jQuery.prototype.keydown = function (fn) {
        /// <summary>Bind an event handler to the "keydown" JavaScript event, or trigger that event on an element.
        /// 1 - keydown(handler(eventObject)) 
        /// 2 - keydown()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.keypress = function (fn) {
        /// <summary>Bind an event handler to the "keypress" JavaScript event, or trigger that event on an element.
        /// 1 - keypress(handler(eventObject)) 
        /// 2 - keypress()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.keyup = function (fn) {
        /// <summary>Bind an event handler to the "keyup" JavaScript event, or trigger that event on an element.
        /// 1 - keyup(handler(eventObject)) 
        /// 2 - keyup()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.last = function () {
        /// <summary>Reduce the set of matched elements to the final one in the set.
        /// </summary>
        /// <returns type="jQuery" />

        return this.eq(-1);
    };
    jQuery.prototype.length =
0;
    jQuery.prototype.live = function (types, data, fn, origSelector /* Internal Use Only */) {
        /// <summary>Attach a handler to the event for all elements which match the current selector, now or in the future.
        /// 1 - live(eventType, handler) 
        /// 2 - live(eventType, eventData, handler)</summary>
        /// <returns type="jQuery" />
        /// <param name="types" type="String">A string containing a JavaScript event type, such as "click" or "keydown." As of jQuery 1.4 the string can contain multiple, space-separated event types or custom event names, as well.</param>
        /// <param name="data" type="Object">A map of data that will be passed to the event handler.</param>
        /// <param name="fn" type="Function">A function to execute at the time the event is triggered.</param>

        var type, i = 0, match, namespaces, preType,
			selector = origSelector || this.selector,
			context = origSelector ? this : jQuery(this.context);

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        types = (types || "").split(" ");

        while ((type = types[i++]) != null) {
            match = rnamespaces.exec(type);
            namespaces = "";

            if (match) {
                namespaces = match[0];
                type = type.replace(rnamespaces, "");
            }

            if (type === "hover") {
                types.push("mouseenter" + namespaces, "mouseleave" + namespaces);
                continue;
            }

            preType = type;

            if (type === "focus" || type === "blur") {
                types.push(liveMap[type] + namespaces);
                type = type + namespaces;

            } else {
                type = (liveMap[type] || type) + namespaces;
            }

            if (name === "live") {
                // bind live handler
                context.each(function () {
                    jQuery.event.add(this, liveConvert(type, selector),
						{ data: data, selector: selector, handler: fn, origType: type, origHandler: fn, preType: preType });
                });

            } else {
                // unbind live handler
                context.unbind(liveConvert(type, selector), fn);
            }
        }

        return this;
    };
    jQuery.prototype.load = function (url, params, callback) {
        /// <summary>1: Bind an event handler to the "load" JavaScript event.
        /// 1.1 - load(handler(eventObject))
        /// 2: Load data from the server and place the returned HTML into the matched element.
        /// 2.1 - load(url, data, complete(responseText, textStatus, XMLHttpRequest))</summary>
        /// <returns type="jQuery" />
        /// <param name="url" type="String">A string containing the URL to which the request is sent.</param>
        /// <param name="params" type="String">A map or string that is sent to the server with the request.</param>
        /// <param name="callback" type="Function">A callback function that is executed when the request completes.</param>

        if (typeof url !== "string") {
            return _load.call(this, url);

            // Don't do a request if no elements are being requested
        } else if (!this.length) {
            return this;
        }

        var off = url.indexOf(" ");
        if (off >= 0) {
            var selector = url.slice(off, url.length);
            url = url.slice(0, off);
        }

        // Default to a GET request
        var type = "GET";

        // If the second parameter was provided
        if (params) {
            // If it's a function
            if (jQuery.isFunction(params)) {
                // We assume that it's the callback
                callback = params;
                params = null;

                // Otherwise, build a param string
            } else if (typeof params === "object") {
                params = jQuery.param(params, jQuery.ajaxSettings.traditional);
                type = "POST";
            }
        }

        var self = this;

        // Request the remote document
        jQuery.ajax({
            url: url,
            type: type,
            dataType: "html",
            data: params,
            complete: function (res, status) {
                // If successful, inject the HTML into all the matched elements
                if (status === "success" || status === "notmodified") {
                    // See if a selector was specified
                    self.html(selector ?
                    // Create a dummy div to hold the results
						jQuery("<div />")
                    // inject the contents of the document in, removing the scripts
                    // to avoid any 'Permission Denied' errors in IE
							.append(res.responseText.replace(rscript, ""))

                    // Locate the specified elements
							.find(selector) :

                    // If not, just inject the full result
						res.responseText);
                }

                if (callback) {
                    self.each(callback, [res.responseText, status, res]);
                }
            }
        });

        return this;
    };
    jQuery.prototype.map = function (callback) {
        /// <summary>Pass each element in the current matched set through a function, producing a new jQuery object containing the return values.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="callback" type="Function">A function object that will be invoked for each element in the current set.</param>

        return this.pushStack(jQuery.map(this, function (elem, i) {
            return callback.call(elem, i, elem);
        }));
    };
    jQuery.prototype.mousedown = function (fn) {
        /// <summary>Bind an event handler to the "mousedown" JavaScript event, or trigger that event on an element.
        /// 1 - mousedown(handler(eventObject)) 
        /// 2 - mousedown()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.mouseenter = function (fn) {
        /// <summary>Bind an event handler to be fired when the mouse enters an element, or trigger that handler on an element.
        /// 1 - mouseenter(handler(eventObject)) 
        /// 2 - mouseenter()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.mouseleave = function (fn) {
        /// <summary>Bind an event handler to be fired when the mouse leaves an element, or trigger that handler on an element.
        /// 1 - mouseleave(handler(eventObject)) 
        /// 2 - mouseleave()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.mousemove = function (fn) {
        /// <summary>Bind an event handler to the "mousemove" JavaScript event, or trigger that event on an element.
        /// 1 - mousemove(handler(eventObject)) 
        /// 2 - mousemove()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.mouseout = function (fn) {
        /// <summary>Bind an event handler to the "mouseout" JavaScript event, or trigger that event on an element.
        /// 1 - mouseout(handler(eventObject)) 
        /// 2 - mouseout()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.mouseover = function (fn) {
        /// <summary>Bind an event handler to the "mouseover" JavaScript event, or trigger that event on an element.
        /// 1 - mouseover(handler(eventObject)) 
        /// 2 - mouseover()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.mouseup = function (fn) {
        /// <summary>Bind an event handler to the "mouseup" JavaScript event, or trigger that event on an element.
        /// 1 - mouseup(handler(eventObject)) 
        /// 2 - mouseup()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.next = function (until, selector) {
        /// <summary>Get the immediately following sibling of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.nextAll = function (until, selector) {
        /// <summary>Get all following siblings of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.nextUntil = function (until, selector) {
        /// <summary>Get all following siblings of each element up to but not including the element matched by the selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to indicate where to stop matching following sibling elements.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.not = function (selector) {
        /// <summary>Remove elements from the set of matched elements.
        /// 1 - not(selector) 
        /// 2 - not(elements) 
        /// 3 - not(function(index))</summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A string containing a selector expression to match elements against.</param>

        return this.pushStack(winnow(this, selector, false), "not", selector);
    };
    jQuery.prototype.offset = function (options) {
        /// <summary>1: Get the current coordinates of the first element in the set of matched elements, relative to the document.
        /// 1.1 - offset()
        /// 2: Set the current coordinates of every element in the set of matched elements, relative to the document.
        /// 2.1 - offset(coordinates) 
        /// 2.2 - offset(function(index, coords))</summary>
        /// <returns type="jQuery" />
        /// <param name="options" type="Object">             An object containing the properties top and left, which are integers indicating the new top and left coordinates for the elements.           </param>

        var elem = this[0];

        if (options) {
            return this.each(function (i) {
                jQuery.offset.setOffset(this, options, i);
            });
        }

        if (!elem || !elem.ownerDocument) {
            return null;
        }

        if (elem === elem.ownerDocument.body) {
            return jQuery.offset.bodyOffset(elem);
        }

        var box = elem.getBoundingClientRect(), doc = elem.ownerDocument, body = doc.body, docElem = doc.documentElement,
			clientTop = docElem.clientTop || body.clientTop || 0, clientLeft = docElem.clientLeft || body.clientLeft || 0,
			top = box.top + (self.pageYOffset || jQuery.support.boxModel && docElem.scrollTop || body.scrollTop) - clientTop,
			left = box.left + (self.pageXOffset || jQuery.support.boxModel && docElem.scrollLeft || body.scrollLeft) - clientLeft;

        return { top: top, left: left };
    };
    jQuery.prototype.offsetParent = function () {
        /// <summary>Get the closest ancestor element that is positioned.
        /// </summary>
        /// <returns type="jQuery" />

        return this.map(function () {
            var offsetParent = this.offsetParent || document.body;
            while (offsetParent && (!/^body|html$/i.test(offsetParent.nodeName) && jQuery.css(offsetParent, "position") === "static")) {
                offsetParent = offsetParent.offsetParent;
            }
            return offsetParent;
        });
    };
    jQuery.prototype.one = function (type, data, fn) {
        /// <summary>Attach a handler to an event for the elements. The handler is executed at most once per element.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">A string containing one or more JavaScript event types, such as "click" or "submit," or custom event names.</param>
        /// <param name="data" type="Object">A map of data that will be passed to the event handler.</param>
        /// <param name="fn" type="Function">A function to execute at the time the event is triggered.</param>

        // Handle object literals
        if (typeof type === "object") {
            for (var key in type) {
                this[name](key, data, type[key], fn);
            }
            return this;
        }

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        var handler = name === "one" ? jQuery.proxy(fn, function (event) {
            jQuery(this).unbind(event, handler);
            return fn.apply(this, arguments);
        }) : fn;

        if (type === "unload" && name !== "one") {
            this.one(type, data, fn);

        } else {
            for (var i = 0, l = this.length; i < l; i++) {
                jQuery.event.add(this[i], type, handler, data);
            }
        }

        return this;
    };
    jQuery.prototype.outerHeight = function (margin) {
        /// <summary>Get the current computed height for the first element in the set of matched elements, including padding and border.
        /// </summary>
        /// <returns type="Number" />
        /// <param name="margin" type="Boolean">A Boolean indicating whether to include the element's margin in the calculation.</param>

        return this[0] ?
			jQuery.css(this[0], type, false, margin ? "margin" : "border") :
			null;
    };
    jQuery.prototype.outerWidth = function (margin) {
        /// <summary>Get the current computed width for the first element in the set of matched elements, including padding and border.
        /// </summary>
        /// <returns type="Number" />
        /// <param name="margin" type="Boolean">A Boolean indicating whether to include the element's margin in the calculation.</param>

        return this[0] ?
			jQuery.css(this[0], type, false, margin ? "margin" : "border") :
			null;
    };
    jQuery.prototype.parent = function (until, selector) {
        /// <summary>Get the parent of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.parents = function (until, selector) {
        /// <summary>Get the ancestors of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.parentsUntil = function (until, selector) {
        /// <summary>Get the ancestors of each element in the current set of matched elements, up to but not including the element matched by the selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to indicate where to stop matching ancestor elements.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.position = function () {
        /// <summary>Get the current coordinates of the first element in the set of matched elements, relative to the offset parent.
        /// </summary>
        /// <returns type="Object" />

        if (!this[0]) {
            return null;
        }

        var elem = this[0],

        // Get *real* offsetParent
		offsetParent = this.offsetParent(),

        // Get correct offsets
		offset = this.offset(),
		parentOffset = /^body|html$/i.test(offsetParent[0].nodeName) ? { top: 0, left: 0} : offsetParent.offset();

        // Subtract element margins
        // note: when an element has margin: auto the offsetLeft and marginLeft
        // are the same in Safari causing offset.left to incorrectly be 0
        offset.top -= parseFloat(jQuery.curCSS(elem, "marginTop", true)) || 0;
        offset.left -= parseFloat(jQuery.curCSS(elem, "marginLeft", true)) || 0;

        // Add offsetParent borders
        parentOffset.top += parseFloat(jQuery.curCSS(offsetParent[0], "borderTopWidth", true)) || 0;
        parentOffset.left += parseFloat(jQuery.curCSS(offsetParent[0], "borderLeftWidth", true)) || 0;

        // Subtract the two offsets
        return {
            top: offset.top - parentOffset.top,
            left: offset.left - parentOffset.left
        };
    };
    jQuery.prototype.prepend = function () {
        /// <summary>Insert content, specified by the parameter, to the beginning of each element in the set of matched elements.
        /// 1 - prepend(content) 
        /// 2 - prepend(function(index, html))</summary>
        /// <returns type="jQuery" />
        /// <param name="" type="jQuery">An element, HTML string, or jQuery object to insert at the beginning of each element in the set of matched elements.</param>

        return this.domManip(arguments, true, function (elem) {
            if (this.nodeType === 1) {
                this.insertBefore(elem, this.firstChild);
            }
        });
    };
    jQuery.prototype.prependTo = function (selector) {
        /// <summary>Insert every element in the set of matched elements to the beginning of the target.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="jQuery">A selector, element, HTML string, or jQuery object; the matched set of elements will be inserted at the beginning of the element(s) specified by this parameter.</param>

        var ret = [], insert = jQuery(selector),
			parent = this.length === 1 && this[0].parentNode;

        if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
            insert[original](this[0]);
            return this;

        } else {
            for (var i = 0, l = insert.length; i < l; i++) {
                var elems = (i > 0 ? this.clone(true) : this).get();
                jQuery.fn[original].apply(jQuery(insert[i]), elems);
                ret = ret.concat(elems);
            }

            return this.pushStack(ret, name, insert.selector);
        }
    };
    jQuery.prototype.prev = function (until, selector) {
        /// <summary>Get the immediately preceding sibling of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.prevAll = function (until, selector) {
        /// <summary>Get all preceding siblings of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.prevUntil = function (until, selector) {
        /// <summary>Get all preceding siblings of each element up to but not including the element matched by the selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to indicate where to stop matching preceding sibling elements.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.pushStack = function (elems, name, selector) {
        /// <summary>Add a collection of DOM elements onto the jQuery stack.
        /// 1 - jQuery.pushStack(elements) 
        /// 2 - jQuery.pushStack(elements, name, arguments)</summary>
        /// <returns type="jQuery" />
        /// <param name="elems" type="Array">An array of elements to push onto the stack and make into a new jQuery object.</param>
        /// <param name="name" type="String">The name of a jQuery method that generated the array of elements.</param>
        /// <param name="selector" type="Array">The arguments that were passed in to the jQuery method (for serialization).</param>

        // Build a new jQuery matched element set
        var ret = jQuery();

        if (jQuery.isArray(elems)) {
            push.apply(ret, elems);

        } else {
            jQuery.merge(ret, elems);
        }

        // Add the old object onto the stack (as a reference)
        ret.prevObject = this;

        ret.context = this.context;

        if (name === "find") {
            ret.selector = this.selector + (this.selector ? " " : "") + selector;
        } else if (name) {
            ret.selector = this.selector + "." + name + "(" + selector + ")";
        }

        // Return the newly-formed element set
        return ret;
    };
    jQuery.prototype.queue = function (type, data) {
        /// <summary>1: Show the queue of functions to be executed on the matched elements.
        /// 1.1 - queue(queueName)
        /// 2: Manipulate the queue of functions to be executed on the matched elements.
        /// 2.1 - queue(queueName, newQueue) 
        /// 2.2 - queue(queueName, callback( next ))</summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">             A string containing the name of the queue. Defaults to fx, the standard effects queue.           </param>
        /// <param name="data" type="Array">An array of functions to replace the current queue contents.</param>

        if (typeof type !== "string") {
            data = type;
            type = "fx";
        }

        if (data === undefined) {
            return jQuery.queue(this[0], type);
        }
        return this.each(function (i, elem) {
            var queue = jQuery.queue(this, type, data);

            if (type === "fx" && queue[0] !== "inprogress") {
                jQuery.dequeue(this, type);
            }
        });
    };
    jQuery.prototype.ready = function (fn) {
        /// <summary>Specify a function to execute when the DOM is fully loaded.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute after the DOM is ready.</param>

        // Attach the listeners
        jQuery.bindReady();

        // If the DOM is already ready
        if (jQuery.isReady) {
            // Execute the function immediately
            fn.call(document, jQuery);

            // Otherwise, remember the function for later
        } else if (readyList) {
            // Add the function to the wait list
            readyList.push(fn);
        }

        return this;
    };
    jQuery.prototype.remove = function (selector, keepData) {
        /// <summary>Remove the set of matched elements from the DOM.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A selector expression that filters the set of matched elements to be removed.</param>

        for (var i = 0, elem; (elem = this[i]) != null; i++) {
            if (!selector || jQuery.filter(selector, [elem]).length) {
                if (!keepData && elem.nodeType === 1) {
                    jQuery.cleanData(elem.getElementsByTagName("*"));
                    jQuery.cleanData([elem]);
                }

                if (elem.parentNode) {
                    elem.parentNode.removeChild(elem);
                }
            }
        }

        return this;
    };
    jQuery.prototype.removeAttr = function (name, fn) {
        /// <summary>Remove an attribute from each element in the set of matched elements.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="name" type="String">An attribute to remove.</param>

        return this.each(function () {
            jQuery.attr(this, name, "");
            if (this.nodeType === 1) {
                this.removeAttribute(name);
            }
        });
    };
    jQuery.prototype.removeClass = function (value) {
        /// <summary>Remove a single class, multiple classes, or all classes from each element in the set of matched elements.
        /// 1 - removeClass(className) 
        /// 2 - removeClass(function(index, class))</summary>
        /// <returns type="jQuery" />
        /// <param name="value" type="String">A class name to be removed from the class attribute of each matched element.</param>

        if (jQuery.isFunction(value)) {
            return this.each(function (i) {
                var self = jQuery(this);
                self.removeClass(value.call(this, i, self.attr("class")));
            });
        }

        if ((value && typeof value === "string") || value === undefined) {
            var classNames = (value || "").split(rspace);

            for (var i = 0, l = this.length; i < l; i++) {
                var elem = this[i];

                if (elem.nodeType === 1 && elem.className) {
                    if (value) {
                        var className = (" " + elem.className + " ").replace(rclass, " ");
                        for (var c = 0, cl = classNames.length; c < cl; c++) {
                            className = className.replace(" " + classNames[c] + " ", " ");
                        }
                        elem.className = jQuery.trim(className);

                    } else {
                        elem.className = "";
                    }
                }
            }
        }

        return this;
    };
    jQuery.prototype.removeData = function (key) {
        /// <summary>Remove a previously-stored piece of data.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="key" type="String">A string naming the piece of data to delete.</param>

        return this.each(function () {
            jQuery.removeData(this, key);
        });
    };
    jQuery.prototype.replaceAll = function (selector) {
        /// <summary>Replace each target element with the set of matched elements.
        /// </summary>
        /// <returns type="jQuery" />

        var ret = [], insert = jQuery(selector),
			parent = this.length === 1 && this[0].parentNode;

        if (parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1) {
            insert[original](this[0]);
            return this;

        } else {
            for (var i = 0, l = insert.length; i < l; i++) {
                var elems = (i > 0 ? this.clone(true) : this).get();
                jQuery.fn[original].apply(jQuery(insert[i]), elems);
                ret = ret.concat(elems);
            }

            return this.pushStack(ret, name, insert.selector);
        }
    };
    jQuery.prototype.replaceWith = function (value) {
        /// <summary>Replace each element in the set of matched elements with the provided new content.
        /// 1 - replaceWith(newContent) 
        /// 2 - replaceWith(function)</summary>
        /// <returns type="jQuery" />
        /// <param name="value" type="jQuery">The content to insert. May be an HTML string, DOM element, or jQuery object.</param>

        if (this[0] && this[0].parentNode) {
            // Make sure that the elements are removed from the DOM before they are inserted
            // this can help fix replacing a parent with child elements
            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this), old = self.html();
                    self.replaceWith(value.call(this, i, old));
                });
            }

            if (typeof value !== "string") {
                value = jQuery(value).detach();
            }

            return this.each(function () {
                var next = this.nextSibling, parent = this.parentNode;

                jQuery(this).remove();

                if (next) {
                    jQuery(next).before(value);
                } else {
                    jQuery(parent).append(value);
                }
            });
        } else {
            return this.pushStack(jQuery(jQuery.isFunction(value) ? value() : value), "replaceWith", value);
        }
    };
    jQuery.prototype.resize = function (fn) {
        /// <summary>Bind an event handler to the "resize" JavaScript event, or trigger that event on an element.
        /// 1 - resize(handler(eventObject)) 
        /// 2 - resize()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.scroll = function (fn) {
        /// <summary>Bind an event handler to the "scroll" JavaScript event, or trigger that event on an element.
        /// 1 - scroll(handler(eventObject)) 
        /// 2 - scroll()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.scrollLeft = function (val) {
        /// <summary>1: Get the current horizontal position of the scroll bar for the first element in the set of matched elements.
        /// 1.1 - scrollLeft()
        /// 2: Set the current horizontal position of the scroll bar for each of the set of matched elements.
        /// 2.1 - scrollLeft(value)</summary>
        /// <returns type="jQuery" />
        /// <param name="val" type="Number">An integer indicating the new position to set the scroll bar to.</param>

        var elem = this[0], win;

        if (!elem) {
            return null;
        }

        if (val !== undefined) {
            // Set the scroll offset
            return this.each(function () {
                win = getWindow(this);

                if (win) {
                    win.scrollTo(
						!i ? val : jQuery(win).scrollLeft(),
						 i ? val : jQuery(win).scrollTop()
					);

                } else {
                    this[method] = val;
                }
            });
        } else {
            win = getWindow(elem);

            // Return the scroll offset
            return win ? ("pageXOffset" in win) ? win[i ? "pageYOffset" : "pageXOffset"] :
				jQuery.support.boxModel && win.document.documentElement[method] ||
					win.document.body[method] :
				elem[method];
        }
    };
    jQuery.prototype.scrollTop = function (val) {
        /// <summary>1: Get the current vertical position of the scroll bar for the first element in the set of matched elements.
        /// 1.1 - scrollTop()
        /// 2: Set the current vertical position of the scroll bar for each of the set of matched elements.
        /// 2.1 - scrollTop(value)</summary>
        /// <returns type="jQuery" />
        /// <param name="val" type="Number">An integer indicating the new position to set the scroll bar to.</param>

        var elem = this[0], win;

        if (!elem) {
            return null;
        }

        if (val !== undefined) {
            // Set the scroll offset
            return this.each(function () {
                win = getWindow(this);

                if (win) {
                    win.scrollTo(
						!i ? val : jQuery(win).scrollLeft(),
						 i ? val : jQuery(win).scrollTop()
					);

                } else {
                    this[method] = val;
                }
            });
        } else {
            win = getWindow(elem);

            // Return the scroll offset
            return win ? ("pageXOffset" in win) ? win[i ? "pageYOffset" : "pageXOffset"] :
				jQuery.support.boxModel && win.document.documentElement[method] ||
					win.document.body[method] :
				elem[method];
        }
    };
    jQuery.prototype.select = function (fn) {
        /// <summary>Bind an event handler to the "select" JavaScript event, or trigger that event on an element.
        /// 1 - select(handler(eventObject)) 
        /// 2 - select()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.serialize = function () {
        /// <summary>Encode a set of form elements as a string for submission.
        /// </summary>
        /// <returns type="String" />

        return jQuery.param(this.serializeArray());
    };
    jQuery.prototype.serializeArray = function () {
        /// <summary>Encode a set of form elements as an array of names and values.
        /// </summary>
        /// <returns type="Array" />

        return this.map(function () {
            return this.elements ? jQuery.makeArray(this.elements) : this;
        })
		.filter(function () {
		    return this.name && !this.disabled &&
				(this.checked || rselectTextarea.test(this.nodeName) ||
					rinput.test(this.type));
		})
		.map(function (i, elem) {
		    var val = jQuery(this).val();

		    return val == null ?
				null :
				jQuery.isArray(val) ?
					jQuery.map(val, function (val, i) {
					    return { name: elem.name, value: val };
					}) :
					{ name: elem.name, value: val };
		}).get();
    };
    jQuery.prototype.show = function (speed, callback) {
        /// <summary>Display the matched elements.
        /// 1 - show() 
        /// 2 - show(duration, callback)</summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        if (speed || speed === 0) {
            return this.animate(genFx("show", 3), speed, callback);

        } else {
            for (var i = 0, l = this.length; i < l; i++) {
                var old = jQuery.data(this[i], "olddisplay");

                this[i].style.display = old || "";

                if (jQuery.css(this[i], "display") === "none") {
                    var nodeName = this[i].nodeName, display;

                    if (elemdisplay[nodeName]) {
                        display = elemdisplay[nodeName];

                    } else {
                        var elem = jQuery("<" + nodeName + " />").appendTo("body");

                        display = elem.css("display");

                        if (display === "none") {
                            display = "block";
                        }

                        elem.remove();

                        elemdisplay[nodeName] = display;
                    }

                    jQuery.data(this[i], "olddisplay", display);
                }
            }

            // Set the display of the elements in a second loop
            // to avoid the constant reflow
            for (var j = 0, k = this.length; j < k; j++) {
                this[j].style.display = jQuery.data(this[j], "olddisplay") || "";
            }

            return this;
        }
    };
    jQuery.prototype.siblings = function (until, selector) {
        /// <summary>Get the siblings of each element in the set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="until" type="String">A string containing a selector expression to match elements against.</param>

        var ret = jQuery.map(this, fn, until);

        if (!runtil.test(name)) {
            selector = until;
        }

        if (selector && typeof selector === "string") {
            ret = jQuery.filter(selector, ret);
        }

        ret = this.length > 1 ? jQuery.unique(ret) : ret;

        if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
            ret = ret.reverse();
        }

        return this.pushStack(ret, name, slice.call(arguments).join(","));
    };
    jQuery.prototype.size = function () {
        /// <summary>Return the number of DOM elements matched by the jQuery object.
        /// </summary>
        /// <returns type="Number" />

        return this.length;
    };
    jQuery.prototype.slice = function () {
        /// <summary>Reduce the set of matched elements to a subset specified by a range of indices.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="" type="Number">An integer indicating the 0-based position after which the elements are selected. If negative, it indicates an offset from the end of the set.</param>
        /// <param name="{name}" type="Number">An integer indicating the 0-based position before which the elements stop being selected. If negative, it indicates an offset from the end of the set. If omitted, the range continues until the end of the set.</param>

        return this.pushStack(slice.apply(this, arguments),
			"slice", slice.call(arguments).join(","));
    };
    jQuery.prototype.slideDown = function (speed, callback) {
        /// <summary>Display the matched elements with a sliding motion.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        return this.animate(props, speed, callback);
    };
    jQuery.prototype.slideToggle = function (speed, callback) {
        /// <summary>Display or hide the matched elements with a sliding motion.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        return this.animate(props, speed, callback);
    };
    jQuery.prototype.slideUp = function (speed, callback) {
        /// <summary>Hide the matched elements with a sliding motion.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="speed" type="Number">A string or number determining how long the animation will run.</param>
        /// <param name="callback" type="Function">A function to call once the animation is complete.</param>

        return this.animate(props, speed, callback);
    };
    jQuery.prototype.stop = function (clearQueue, gotoEnd) {
        /// <summary>Stop the currently-running animation on the matched elements.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="clearQueue" type="Boolean">             A Boolean indicating whether to remove queued animation as well. Defaults to false.           </param>
        /// <param name="gotoEnd" type="Boolean">             A Boolean indicating whether to complete the current animation immediately. Defaults to false.           </param>

        var timers = jQuery.timers;

        if (clearQueue) {
            this.queue([]);
        }

        this.each(function () {
            // go in reverse order so anything added to the queue during the loop is ignored
            for (var i = timers.length - 1; i >= 0; i--) {
                if (timers[i].elem === this) {
                    if (gotoEnd) {
                        // force the next step to be the last
                        timers[i](true);
                    }

                    timers.splice(i, 1);
                }
            }
        });

        // start the next in the queue if the last step wasn't forced
        if (!gotoEnd) {
            this.dequeue();
        }

        return this;
    };
    jQuery.prototype.submit = function (fn) {
        /// <summary>Bind an event handler to the "submit" JavaScript event, or trigger that event on an element.
        /// 1 - submit(handler(eventObject)) 
        /// 2 - submit()</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute each time the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.text = function (text) {
        /// <summary>1: Get the combined text contents of each element in the set of matched elements, including their descendants.
        /// 1.1 - text()
        /// 2: Set the content of each element in the set of matched elements to the specified text.
        /// 2.1 - text(textString) 
        /// 2.2 - text(function(index, text))</summary>
        /// <returns type="jQuery" />
        /// <param name="text" type="String">A string of text to set as the content of each matched element.</param>

        if (jQuery.isFunction(text)) {
            return this.each(function (i) {
                var self = jQuery(this);
                self.text(text.call(this, i, self.text()));
            });
        }

        if (typeof text !== "object" && text !== undefined) {
            return this.empty().append((this[0] && this[0].ownerDocument || document).createTextNode(text));
        }

        return jQuery.text(this);
    };
    jQuery.prototype.toArray = function () {
        /// <summary>Retrieve all the DOM elements contained in the jQuery set, as an array.
        /// </summary>
        /// <returns type="Array" />

        return slice.call(this, 0);
    };
    jQuery.prototype.toggle = function (fn, fn2) {
        /// <summary>1: Bind two or more handlers to the matched elements, to be executed on alternate clicks.
        /// 1.1 - toggle(handler(eventObject), handler(eventObject), handler(eventObject))
        /// 2: Display or hide the matched elements.
        /// 2.1 - toggle(duration, callback) 
        /// 2.2 - toggle(showOrHide)</summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute every even time the element is clicked.</param>
        /// <param name="fn2" type="Function">A function to execute every odd time the element is clicked.</param>
        /// <param name="{name}" type="Function">Additional handlers to cycle through after clicks.</param>

        var bool = typeof fn === "boolean";

        if (jQuery.isFunction(fn) && jQuery.isFunction(fn2)) {
            this._toggle.apply(this, arguments);

        } else if (fn == null || bool) {
            this.each(function () {
                var state = bool ? fn : jQuery(this).is(":hidden");
                jQuery(this)[state ? "show" : "hide"]();
            });

        } else {
            this.animate(genFx("toggle", 3), fn, fn2);
        }

        return this;
    };
    jQuery.prototype.toggleClass = function (value, stateVal) {
        /// <summary>Add or remove one or more classes from each element in the set of matched elements, depending on either the class's presence or the value of the switch argument.
        /// 1 - toggleClass(className) 
        /// 2 - toggleClass(className, switch) 
        /// 3 - toggleClass(function(index, class), switch)</summary>
        /// <returns type="jQuery" />
        /// <param name="value" type="String">One or more class names (separated by spaces) to be toggled for each element in the matched set.</param>
        /// <param name="stateVal" type="Boolean">A boolean value to determine whether the class should be added or removed.</param>

        var type = typeof value, isBool = typeof stateVal === "boolean";

        if (jQuery.isFunction(value)) {
            return this.each(function (i) {
                var self = jQuery(this);
                self.toggleClass(value.call(this, i, self.attr("class"), stateVal), stateVal);
            });
        }

        return this.each(function () {
            if (type === "string") {
                // toggle individual class names
                var className, i = 0, self = jQuery(this),
					state = stateVal,
					classNames = value.split(rspace);

                while ((className = classNames[i++])) {
                    // check each className given, space seperated list
                    state = isBool ? state : !self.hasClass(className);
                    self[state ? "addClass" : "removeClass"](className);
                }

            } else if (type === "undefined" || type === "boolean") {
                if (this.className) {
                    // store className if set
                    jQuery.data(this, "__className__", this.className);
                }

                // toggle whole className
                this.className = this.className || value === false ? "" : jQuery.data(this, "__className__") || "";
            }
        });
    };
    jQuery.prototype.trigger = function (type, data) {
        /// <summary>Execute all handlers and behaviors attached to the matched elements for the given event type.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">             A string containing a JavaScript event type, such as click or submit.           </param>
        /// <param name="data" type="Array">An array of additional parameters to pass along to the event handler.</param>

        return this.each(function () {
            jQuery.event.trigger(type, data, this);
        });
    };
    jQuery.prototype.triggerHandler = function (type, data) {
        /// <summary>Execute all handlers attached to an element for an event.
        /// </summary>
        /// <returns type="Object" />
        /// <param name="type" type="String">             A string containing a JavaScript event type, such as click or submit.           </param>
        /// <param name="data" type="Array">An array of additional parameters to pass along to the event handler.</param>

        if (this[0]) {
            var event = jQuery.Event(type);
            event.preventDefault();
            event.stopPropagation();
            jQuery.event.trigger(event, data, this[0]);
            return event.result;
        }
    };
    jQuery.prototype.unbind = function (type, fn) {
        /// <summary>Remove a previously-attached event handler from the elements.
        /// 1 - unbind(eventType, handler(eventObject)) 
        /// 2 - unbind(event)</summary>
        /// <returns type="jQuery" />
        /// <param name="type" type="String">             A string containing a JavaScript event type, such as click or submit.           </param>
        /// <param name="fn" type="Function">The function that is to be no longer executed.</param>

        // Handle object literals
        if (typeof type === "object" && !type.preventDefault) {
            for (var key in type) {
                this.unbind(key, type[key]);
            }

        } else {
            for (var i = 0, l = this.length; i < l; i++) {
                jQuery.event.remove(this[i], type, fn);
            }
        }

        return this;
    };
    jQuery.prototype.undelegate = function (selector, types, fn) {
        /// <summary>Remove a handler from the event for all elements which match the current selector, now or in the future, based upon a specific set of root elements.
        /// 1 - undelegate() 
        /// 2 - undelegate(selector, eventType) 
        /// 3 - undelegate(selector, eventType, handler)</summary>
        /// <returns type="jQuery" />
        /// <param name="selector" type="String">A selector which will be used to filter the event results.</param>
        /// <param name="types" type="String">A string containing a JavaScript event type, such as "click" or "keydown"</param>
        /// <param name="fn" type="Function">A function to execute at the time the event is triggered.</param>

        if (arguments.length === 0) {
            return this.unbind("live");

        } else {
            return this.die(types, null, fn, selector);
        }
    };
    jQuery.prototype.unload = function (fn) {
        /// <summary>Bind an event handler to the "unload" JavaScript event.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="fn" type="Function">A function to execute when the event is triggered.</param>

        return fn ? this.bind(name, fn) : this.trigger(name);
    };
    jQuery.prototype.unwrap = function () {
        /// <summary>Remove the parents of the set of matched elements from the DOM, leaving the matched elements in their place.
        /// </summary>
        /// <returns type="jQuery" />

        return this.parent().each(function () {
            if (!jQuery.nodeName(this, "body")) {
                jQuery(this).replaceWith(this.childNodes);
            }
        }).end();
    };
    jQuery.prototype.val = function (value) {
        /// <summary>1: Get the current value of the first element in the set of matched elements.
        /// 1.1 - val()
        /// 2: Set the value of each element in the set of matched elements.
        /// 2.1 - val(value) 
        /// 2.2 - val(function(index, value))</summary>
        /// <returns type="jQuery" />
        /// <param name="value" type="String">A string of text or an array of strings to set as the value property of each matched element.</param>

        if (value === undefined) {
            var elem = this[0];

            if (elem) {
                if (jQuery.nodeName(elem, "option")) {
                    return (elem.attributes.value || {}).specified ? elem.value : elem.text;
                }

                // We need to handle select boxes special
                if (jQuery.nodeName(elem, "select")) {
                    var index = elem.selectedIndex,
						values = [],
						options = elem.options,
						one = elem.type === "select-one";

                    // Nothing was selected
                    if (index < 0) {
                        return null;
                    }

                    // Loop through all the selected options
                    for (var i = one ? index : 0, max = one ? index + 1 : options.length; i < max; i++) {
                        var option = options[i];

                        if (option.selected) {
                            // Get the specifc value for the option
                            value = jQuery(option).val();

                            // We don't need an array for one selects
                            if (one) {
                                return value;
                            }

                            // Multi-Selects return an array
                            values.push(value);
                        }
                    }

                    return values;
                }

                // Handle the case where in Webkit "" is returned instead of "on" if a value isn't specified
                if (rradiocheck.test(elem.type) && !jQuery.support.checkOn) {
                    return elem.getAttribute("value") === null ? "on" : elem.value;
                }


                // Everything else, we just grab the value
                return (elem.value || "").replace(rreturn, "");

            }

            return undefined;
        }

        var isFunction = jQuery.isFunction(value);

        return this.each(function (i) {
            var self = jQuery(this), val = value;

            if (this.nodeType !== 1) {
                return;
            }

            if (isFunction) {
                val = value.call(this, i, self.val());
            }

            // Typecast each time if the value is a Function and the appended
            // value is therefore different each time.
            if (typeof val === "number") {
                val += "";
            }

            if (jQuery.isArray(val) && rradiocheck.test(this.type)) {
                this.checked = jQuery.inArray(self.val(), val) >= 0;

            } else if (jQuery.nodeName(this, "select")) {
                var values = jQuery.makeArray(val);

                jQuery("option", this).each(function () {
                    this.selected = jQuery.inArray(jQuery(this).val(), values) >= 0;
                });

                if (!values.length) {
                    this.selectedIndex = -1;
                }

            } else {
                this.value = val;
            }
        });
    };
    jQuery.prototype.width = function (size) {
        /// <summary>1: Get the current computed width for the first element in the set of matched elements.
        /// 1.1 - width()
        /// 2: Set the CSS width of each element in the set of matched elements.
        /// 2.1 - width(value) 
        /// 2.2 - width(function(index, width))</summary>
        /// <returns type="jQuery" />
        /// <param name="size" type="Number">An integer representing the number of pixels, or an integer along with an optional unit of measure appended (as a string).</param>

        // Get window width or height
        var elem = this[0];
        if (!elem) {
            return size == null ? null : this;
        }

        if (jQuery.isFunction(size)) {
            return this.each(function (i) {
                var self = jQuery(this);
                self[type](size.call(this, i, self[type]()));
            });
        }

        return ("scrollTo" in elem && elem.document) ? // does it walk and quack like a window?
        // Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
			elem.document.compatMode === "CSS1Compat" && elem.document.documentElement["client" + name] ||
			elem.document.body["client" + name] :

        // Get document width or height
			(elem.nodeType === 9) ? // is it a document
        // Either scroll[Width/Height] or offset[Width/Height], whichever is greater
				Math.max(
					elem.documentElement["client" + name],
					elem.body["scroll" + name], elem.documentElement["scroll" + name],
					elem.body["offset" + name], elem.documentElement["offset" + name]
				) :

        // Get or set width or height on the element
				size === undefined ?
        // Get width or height on the element
					jQuery.css(elem, type) :

        // Set the width or height on the element (default to pixels if value is unitless)
					this.css(type, typeof size === "string" ? size : size + "px");
    };
    jQuery.prototype.wrap = function (html) {
        /// <summary>Wrap an HTML structure around each element in the set of matched elements.
        /// 1 - wrap(wrappingElement) 
        /// 2 - wrap(wrappingFunction)</summary>
        /// <returns type="jQuery" />
        /// <param name="html" type="jQuery">An HTML snippet, selector expression, jQuery object, or DOM element specifying the structure to wrap around the matched elements.</param>

        return this.each(function () {
            jQuery(this).wrapAll(html);
        });
    };
    jQuery.prototype.wrapAll = function (html) {
        /// <summary>Wrap an HTML structure around all elements in the set of matched elements.
        /// </summary>
        /// <returns type="jQuery" />
        /// <param name="html" type="jQuery">An HTML snippet, selector expression, jQuery object, or DOM element specifying the structure to wrap around the matched elements.</param>

        if (jQuery.isFunction(html)) {
            return this.each(function (i) {
                jQuery(this).wrapAll(html.call(this, i));
            });
        }

        if (this[0]) {
            // The elements to wrap the target around
            var wrap = jQuery(html, this[0].ownerDocument).eq(0).clone(true);

            if (this[0].parentNode) {
                wrap.insertBefore(this[0]);
            }

            wrap.map(function () {
                var elem = this;

                while (elem.firstChild && elem.firstChild.nodeType === 1) {
                    elem = elem.firstChild;
                }

                return elem;
            }).append(this);
        }

        return this;
    };
    jQuery.prototype.wrapInner = function (html) {
        /// <summary>Wrap an HTML structure around the content of each element in the set of matched elements.
        /// 1 - wrapInner(wrappingElement) 
        /// 2 - wrapInner(wrappingFunction)</summary>
        /// <returns type="jQuery" />
        /// <param name="html" type="String">An HTML snippet, selector expression, jQuery object, or DOM element specifying the structure to wrap around the content of the matched elements.</param>

        if (jQuery.isFunction(html)) {
            return this.each(function (i) {
                jQuery(this).wrapInner(html.call(this, i));
            });
        }

        return this.each(function () {
            var self = jQuery(this), contents = self.contents();

            if (contents.length) {
                contents.wrapAll(html);

            } else {
                self.append(html);
            }
        });
    };
    jQuery.fn = jQuery.prototype;
    jQuery.fn.init.prototype = jQuery.fn;
    window.jQuery = window.$ = jQuery;
})(window);