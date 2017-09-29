var timeout = 1000;
var Xrm = window.parent.Xrm;
var counter = 0;

$(function() {
    switch (Xrm.Page.ui.getFormType()) {
    case 1: //Create
        Xrm.Page.getAttribute("modifiedon").addOnChange(modifiedOnChanged);
        break;
    case 2: //Update
        checkForUpdate();
        break;
    default:
    }
});

function modifiedOnChanged() {
    checkForUpdate();
}

function checkForUpdate() {
    $("#status").empty().append("Loading...");
    var id = Xrm.Page.data.entity.getId().replace("{", "").replace("}", "");
    var odataEndpoint = Xrm.Page.context.getClientUrl() + "/api/data/v8.2";

    o().config({
        endpoint: odataEndpoint
    });

    o("dkdt_asyncrequestresponsesamples")
        .filter("dkdt_asyncrequestresponsesampleid eq " + id)
        .first()
        .select("dkdt_updatefromazurecodecomplete")
        .get(function(entity) {
            var isUpdated = entity.dkdt_updatefromazurecodecomplete;
            if (isUpdated) {
                $("#status").empty().append("Azure code updated entity.");
                counter = 0;
            } else {
                counter++;
                if (counter > 15) {
                    $("#status").empty()
                        .append("Something went wrong on the server.  Please contact your administrator.");
                    counter = 0;
                } else {
                    setTimeout(checkForUpdate, timeout);
                }
            }
        });
}