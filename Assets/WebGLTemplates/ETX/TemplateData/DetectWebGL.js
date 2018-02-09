
//NB: 11/02/2017
//script that checks for WebGL compatibility. Informs user if their
//device is incompatible and redirects them to the ETX website.
function DetectWebGL(e, t, r) {
    //Create canvas for getting context
    var canvas = document.createElement("canvas");
    //Get webgl Context from canvas
    /* NOTE: removed experimental-webgl context to avoid
    accidental approval of older mobile devices. Remove
    commented out code to reactivate.*/
    var gl = !! canvas.getContext("webgl");// || !! canvas.getContext("experimental-webgl");
    //Get WebGLRenderingContext from window
    var rendcon = !! window.WebGLRenderingContext;
    //if webgl context or rendering context
    if (!rendcon || !gl) {
    	if (confirm("Small Worlds cannot run because" + 
            "WebGL is not supported on your device.")) {
    		window.location = "https://infiniscope.org/";
    	} else {
    		window.location = "https://infiniscope.org/";
    	}
    }
    //calls passed function
    t();
  }
