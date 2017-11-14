//  used to prevent:
//      $(document).ready(onReady); 
//  ...from throwing an exception when unit testing

var $ = function (p){
    return $;
}

$.ready = function (p){
}