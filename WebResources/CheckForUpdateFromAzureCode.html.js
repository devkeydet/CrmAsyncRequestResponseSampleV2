var AsyncRequestResponseSample;
(function (AsyncRequestResponseSample) {
    var CheckForUpdateFromAzureCode = (function () {

        function CheckForUpdateFromAzureCode() {
        }

        CheckForUpdateFromAzureCode.init = function () {
            CheckForUpdateFromAzureCode.timeout = 1000;
            CheckForUpdateFromAzureCode.counter = 0;
        };

        CheckForUpdateFromAzureCode.onReady = function () {
            switch (Xrm.Page.ui.getFormType()) {
                case 1://Create
                    Xrm.Page.getAttribute("modifiedon").addOnChange(CheckForUpdateFromAzureCode.checkForUpdate);
                    break;
                case 2://Update
                    CheckForUpdateFromAzureCode.checkForUpdate();
                    break;
                default:
            }
        };

        CheckForUpdateFromAzureCode.updateCounter = function (counter) {
            CheckForUpdateFromAzureCode.counter = counter;
        };

        CheckForUpdateFromAzureCode.checkForUpdate = function () {
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
                .get(function (entity) {
                    var isUpdated = entity.dkdt_updatefromazurecodecomplete;
                    if (isUpdated) {
                        $("#status").empty().append("Azure code updated entity.");
                        CheckForUpdateFromAzureCode.counter = 0;
                    }
                    else {
                        CheckForUpdateFromAzureCode.counter++;
                        if (CheckForUpdateFromAzureCode.counter > 30) {
                            $("#status").empty()
                                .append("Something went wrong on the server.  Please contact your administrator.");
                            CheckForUpdateFromAzureCode.counter = 0;
                        }
                        else {
                            setTimeout(CheckForUpdateFromAzureCode.checkForUpdate, CheckForUpdateFromAzureCode.timeout);
                        }
                    }
                });
        };

        return CheckForUpdateFromAzureCode;
    }());

    AsyncRequestResponseSample.CheckForUpdateFromAzureCode = CheckForUpdateFromAzureCode;
    CheckForUpdateFromAzureCode.init();

})(AsyncRequestResponseSample || (AsyncRequestResponseSample = {}));

var Xrm = window.parent.Xrm;
$(document).ready(AsyncRequestResponseSample.CheckForUpdateFromAzureCode.onReady);