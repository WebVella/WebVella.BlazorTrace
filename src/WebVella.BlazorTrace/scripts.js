window.WebVellaBlazorTrace = {
	escapeKeyEventListener: null,
	f1KeyEventListener: null,
	inited: false,
	initKeyListeners: function () {
		document.addEventListener("keydown", function (evtobj) {
			if (evtobj.code === "Escape") {
				if(!WebVellaBlazorTrace.escapeKeyEventListener) return;
				evtobj.preventDefault();
				evtobj.stopPropagation();
				WebVellaBlazorTrace.executeEscapeKeyEventCallbacks(evtobj);
			}
			if (evtobj.code === "F1") {
				if(!WebVellaBlazorTrace.f1KeyEventListener) return;
				evtobj.preventDefault();
				evtobj.stopPropagation();
				WebVellaBlazorTrace.executeF1KeyEventCallbacks(evtobj);
			}
		});
		WebVellaBlazorTrace.inited = true;
	},
	addEscapeKeyEventListener: function (dotNetHelper, methodName) {
		if (!WebVellaBlazorTrace.inited) {
			WebVellaBlazorTrace.initKeyListeners();
		}
		WebVellaBlazorTrace.escapeKeyEventListener = { dotNetHelper: dotNetHelper, methodName: methodName };
		return true;
	},
	executeEscapeKeyEventCallbacks: function (eventData) {
		if (WebVellaBlazorTrace.escapeKeyEventListener) {
			const dotNetHelper = WebVellaBlazorTrace.escapeKeyEventListener.dotNetHelper;
			const methodName = WebVellaBlazorTrace.escapeKeyEventListener.methodName;
			if (dotNetHelper && methodName) {
				dotNetHelper.invokeMethodAsync(methodName, eventData.code);
			}
		}
		return true;
	},
	removeEscapeKeyEventListener: function () {
		if (WebVellaBlazorTrace.escapeKeyEventListener) {
			WebVellaBlazorTrace.escapeKeyEventListener = null;
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
	}
}

document.addEventListener("DOMContentLoaded", function () {
	WebVellaBlazorTrace.init();
});