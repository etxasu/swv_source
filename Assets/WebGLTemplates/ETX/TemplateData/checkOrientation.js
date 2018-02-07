//NB:11/13/2017
//function to check for landscape orientation and block loading gameInstance
//until device is in supported orientation
function checkOrientation() {
	//NB:11/15/2017
      //separated mobile firefox from other mobile browsers, since they're in the
      //process of changing how screen orientation is handled

      //check if device is mobile (desktop browsers don't have window.orientation defined)
      if (typeof window.orientation !== 'undefined') { 
        //check if browser is firefox and handle uniquely
        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1 || navigator.userAgent.toLowerCase().indexOf('fxios') > -1) {
          try {
            if(window.orientation == 0 || window.orientation == 180) {
              if(confirm('Portrait orientation not supported. Please rotate screen to landscape mode and then click "OK".')) {
                location.reload(true);
              }
            }
          } catch(e) {
            console.log(e);
          }
          //otherwise, check if device is not in either accepted landscape mode
        } else if(window.orientation !=-90 && window.orientation != 90) {
          //return confirmation dialogue asking user to rotate device
          if(confirm('Portrait orientation not supported. Please rotate screen to landscape mode and then click "OK".')) {
            //reload until they rotate device
            location.reload(true);
          }
        }
      }
}