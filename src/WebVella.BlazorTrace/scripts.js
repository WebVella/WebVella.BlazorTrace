window.WebVellaBlazorTrace = {
	escapeKeyEventListeners: {},
	inited: false,
	initKeyListeners: function () {
		document.addEventListener("keydown", function (evtobj) {
			if (evtobj.code === "Escape") {
				WebVellaBlazorTrace.executeEscapeKeyEventCallbacks(evtobj);
			}
		});
		WebVellaBlazorTrace.inited = true;
	},
	addEscapeKeyEventListener: function (dotNetHelper, listenerId, methodName) {
		if(!WebVellaBlazorTrace.inited){
			WebVellaBlazorTrace.initKeyListeners();
		}
		WebVellaBlazorTrace.escapeKeyEventListeners[listenerId] = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	executeEscapeKeyEventCallbacks: function (eventData) {
		if (WebVellaBlazorTrace.escapeKeyEventListeners) {

			for (const prop in WebVellaBlazorTrace.escapeKeyEventListeners) {
				const dotNetHelper = WebVellaBlazorTrace.escapeKeyEventListeners[prop].dotNetHelper;
				const methodName = WebVellaBlazorTrace.escapeKeyEventListeners[prop].methodName;
				if (dotNetHelper && methodName) {
					dotNetHelper.invokeMethodAsync(methodName);
				}
			}
		}
		return true;
	},
	removeEscapeKeyEventListener: function (listenerId) {
		if (WebVellaBlazorTrace.escapeKeyEventListeners) {
			if (WebVellaBlazorTrace.escapeKeyEventListeners[listenerId]) {
				delete WebVellaBlazorTrace.escapeKeyEventListeners[listenerId];
			}
		}
		return true;
	},
}

document.addEventListener("DOMContentLoaded", function () {
	WebVellaBlazorTrace.init();
});