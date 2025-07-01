window.WebVellaBlazorTrace = {
	escapeKeyEventListeners: {},
	escapeKeyEventListenerQueue: [],
	f1KeyEventListener: null,
	inited: false,
	initKeyListeners: function () {
		document.addEventListener("keydown", function (evtobj) {
			if (evtobj.code === "Escape") {
				if (!WebVellaBlazorTrace.escapeKeyEventListenerQueue || WebVellaBlazorTrace.escapeKeyEventListenerQueue.length == 0) return;
				evtobj.preventDefault();
				evtobj.stopPropagation();
				WebVellaBlazorTrace.executeEscapeKeyEventCallbacks(evtobj);
			}
			if (evtobj.code === "F1") {
				if (!WebVellaBlazorTrace.f1KeyEventListener) return;
				evtobj.preventDefault();
				evtobj.stopPropagation();
				WebVellaBlazorTrace.executeF1KeyEventCallbacks(evtobj);
			}
		});
		WebVellaBlazorTrace.inited = true;
	},
	addEscapeKeyEventListener: function (dotNetHelper, listenerId, methodName) {
		if (!WebVellaBlazorTrace.inited) {
			WebVellaBlazorTrace.initKeyListeners();
		}
		WebVellaBlazorTrace.escapeKeyEventListeners[listenerId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		WebVellaBlazorTrace.escapeKeyEventListenerQueue.push(listenerId);
		return true;
	},
	executeEscapeKeyEventCallbacks: function (eventData) {
		if (WebVellaBlazorTrace.escapeKeyEventListenerQueue && WebVellaBlazorTrace.escapeKeyEventListenerQueue.length > 0) {
			var lastRegisteredListenerId = WebVellaBlazorTrace.escapeKeyEventListenerQueue[WebVellaBlazorTrace.escapeKeyEventListenerQueue.length - 1];
			const dotNetHelper = WebVellaBlazorTrace.escapeKeyEventListeners[lastRegisteredListenerId].dotNetHelper;
			const methodName = WebVellaBlazorTrace.escapeKeyEventListeners[lastRegisteredListenerId].methodName;
			if (dotNetHelper && methodName) {
				dotNetHelper.invokeMethodAsync(methodName, eventData.code);
			}
		}
		return true;
	},
	removeEscapeKeyEventListener: function (listenerId) {
		if (WebVellaBlazorTrace.escapeKeyEventListeners && WebVellaBlazorTrace.escapeKeyEventListeners[listenerId]) {
			delete WebVellaBlazorTrace.escapeKeyEventListeners[listenerId];
			WebVellaBlazorTrace.escapeKeyEventListenerQueue = WebVellaBlazorTrace.escapeKeyEventListenerQueue.filter(x => x != listenerId);
		}
		return true;
	},

	addF1KeyEventListener: function (dotNetHelper, methodName) {
		if (!WebVellaBlazorTrace.inited) {
			WebVellaBlazorTrace.initKeyListeners();
		}
		WebVellaBlazorTrace.f1KeyEventListener = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	executeF1KeyEventCallbacks: function (eventData) {
		if (WebVellaBlazorTrace.f1KeyEventListener) {
			const dotNetHelper = WebVellaBlazorTrace.f1KeyEventListener.dotNetHelper;
			const methodName = WebVellaBlazorTrace.f1KeyEventListener.methodName;
			if (dotNetHelper && methodName) {
				dotNetHelper.invokeMethodAsync(methodName, eventData.code);
			}
		}
		return true;
	},
	removeF1KeyEventListener: function () {
		if (WebVellaBlazorTrace.f1KeyEventListener) {
			WebVellaBlazorTrace.f1KeyEventListener = null;
		}
		return true;
	},
	getLocalStorageKeysWithPrefix: function (prefix) {
		const keys = [];

		// Iterate over all items in localStorage
		for (let i = 0; i < localStorage.length; i++) {
			const key = localStorage.key(i);

			// Check if the key starts with the provided prefix
			if (key && key.startsWith(prefix)) {
				keys.push(key);
			}
		}

		return keys;
	},
}