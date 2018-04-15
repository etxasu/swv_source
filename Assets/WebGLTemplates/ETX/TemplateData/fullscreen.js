//NB: 03/28/2018

//toggles between fullscreen and standard view
function toggleFull() {
	var frame = document.getElementById("gameContainer");
	if (!document.isFullScreen && !document.fullscreenElement && !document.webkitFullscreenElement && !document.mozFullScreenElement && !document.msFullscreenElement) {
		console.log ("Attempting to enter fullscreen");
		if(frame.requestFullScreen) {
			console.log("standardFull");
			frame.requestFullScreen();
			//addExit();
		} else if (frame.mozRequestFullscreen) {
			console.log("mozFull");
			frame.mozRequestFullscreen();
			//addExit();
		} else if (frame.webkitRequestFullscreen) {
			console.log("webkitFull");
			frame.webkitRequestFullscreen();
			//addExit();
		} else if (frame.msRequestFullscreen) {
			console.log("msFull");
			frame.msRequestFullscreen();
			//addExit();
		} 
	} else {
		console.log("Attempting to exit fullscreen");
		if(document.exitFullScreen) {
			console.log("standardExit");
			document.exitFullScreen();
		} else if (document.mozCancelFullscreen) {
			console.log("moxExit");
			document.mozCancelFullscreen();
		} else if (document.webkitExitFullscreen) {
			console.log("webkitExit");
			document.webkitExitFullscreen();
		} else if (document.msExitFullscreen) {
			console.log("msExit");
			document.msExitFullscreen();
		}
	document.getElementById("exitFullButton").style.visibility = 'hidden';
	document.getElementById("fullButton").style.visibility = 'initial';
	}
}

//adds exit button in gameContainer div, so it'll be present in fullscreen frame
function addExit() {
	if(!document.getElementById("exitFullButton")) {
		var button = document.createElement("BUTTON");   
		button.id = "exitFullButton"     
		button.class = "exitFullButton";                            
		document.getElementById("gameContainer").appendChild(button); 
		button = document.getElementById("exitFullButton");
		button.className = "exitFullButton";
		button.addEventListener("click", toggleFull);
	} else{
		document.getElementById("exitFullButton").style.visibility = 'initial';
	}
	document.getElementById("fullButton").style.visibility = 'hidden';
}

//removes fullscreen button if 
function checkSupport(){
	if(!detectFullFunc()) {
		console.log("Fullscreen not supported, removing button.");
		document.getElementById("fullButton").remove();
	}

}

//detects if any of the fullscreen functions are supported on the browser.
function detectFullFunc() {
	var frame = document.getElementById("gameContainer");
	if(frame.requestFullScreen || frame.mozRequestFullscreen || frame.webkitRequestFullscreen || frame.msRequestFullscreen){
		return true;
	}
	return false;
}