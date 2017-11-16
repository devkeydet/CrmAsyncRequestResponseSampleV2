/// <reference path="../testhelpers/sinon.js"/>
/// <reference path="../testhelpers/fakedocready.js"/>
/// <reference path="../testhelpers/Xrm.Page.js"/>
/// <reference path="../scripts/o.js"/>
/// <reference path="../CheckForUpdateFromAzureCode.html.js"/>

// TODO:    Make CheckForUpdateFromAzureCode.html.js more test friendly
// TODO:    Rationalize when to use sinon stubs vs jasmine spies.  
//          Right now, using stubs mostly for the withArgs() capability, which sinon doesn't appear to have:
//          https://github.com/jasmine/jasmine/issues/94

// NOTE:    The easies way to debug/run these tests in Visual Studio is to use: http://mmanela.github.io/chutzpah/

describe("CheckForUpdateFromAzureCode", function() {
    var _fakeResult;
    var _fakeAttribute;
    var _fireOnChange;
    var _formTypeStub;

    beforeEach(function () {
        // Fake for jquery        
        // NOTE: simple faking approach.  For more elaborate html scenarios, consider using https://github.com/velesin/jasmine-jquery fixtures
        _fakeResult = function() { };
        _fakeResult.empty = function() { return _fakeResult; }
        _fakeResult.append = function(val) { }       
        spyOn(_fakeResult, 'append');
        $ = sinon.stub();
        $.withArgs("#status").returns(_fakeResult);

        // Fake Xrm
        // NOTE: Simple faking approach.  Consider using https://github.com/camelCaseDave/xrm-mock-generator
        _fakeAttribute = {
            addOnChange: function(func) { 
                _fireOnChange = func;
            }
        }
        spyOn(_fakeAttribute, 'addOnChange').and.callThrough();

        var attrStub = sinon.stub(Xrm.Page, "getAttribute");
        attrStub.withArgs("modifiedon").returns(_fakeAttribute);
        sinon.stub(Xrm.Page.data.entity, "getId").returns("{some-id}");
        sinon.stub(Xrm.Page.context, "getClientUrl").returns("https://some.crm.dynamics.com");
        _formTypeStub = sinon.stub(Xrm.Page.ui, "getFormType")

        // Fake XMLHttpRequest
        this.xhr = sinon.useFakeXMLHttpRequest();
        var requests = this.requests = [];

        this.xhr.onCreate = function (xhr) {
            requests.push(xhr);
        };
    });

    afterEach(function () {
        Xrm.Page.data.entity.getId.restore();
        Xrm.Page.getAttribute.restore();
        Xrm.Page.context.getClientUrl.restore();
        Xrm.Page.ui.getFormType.restore();
        this.xhr.restore();
    });

    function expectResponse(requests, val){
        expect(requests.length).toBe(1);
        requests[0].respond(200, {"Content-Type": "application/json"},
            '{"dkdt_updatefromazurecodecomplete":' + val + '}'); 
    }

    it("Create_Form_Save_Success", function() {
        // Arrange (majority of arrange happens in beforeEach)     
        _formTypeStub.returns(1);

        //Act
        AsyncRequestResponseSample.CheckForUpdateFromAzureCode.onReady();
        _fireOnChange();
        
        //Assert
        expectResponse(this.requests, true);     
        expect(_fakeResult.append).toHaveBeenCalledWith("Azure code updated entity.");
        expect(_fakeAttribute.addOnChange).toHaveBeenCalled();
    });

     it("Create_Form_Save_CallSetTimeout", function() {
        // Arrange (majority of arrange happens in beforeEach)              
        _formTypeStub.returns(1);
         AsyncRequestResponseSample.CheckForUpdateFromAzureCode.updateCounter(0);

        //Act
        AsyncRequestResponseSample.CheckForUpdateFromAzureCode.onReady();
        _fireOnChange();
        
        //Assert
        expectResponse(this.requests, false);       
        expect(_fakeResult.append).toHaveBeenCalledWith("Loading...");
    });

    it("Create_Form_Save_Timeout", function() {
        // Arrange (majority of arrange happens in beforeEach)              
        _formTypeStub.returns(1);
        AsyncRequestResponseSample.CheckForUpdateFromAzureCode.updateCounter(16);

        //Act
        AsyncRequestResponseSample.CheckForUpdateFromAzureCode.onReady();
        _fireOnChange();
        
        //Assert
        expectResponse(this.requests, false);       
        expect(_fakeResult.append).toHaveBeenCalledWith("Something went wrong on the server.  Please contact your administrator.");
        expect(_fakeAttribute.addOnChange).toHaveBeenCalled();        
    });

    it("Update_Form_Load", function() {
        // Arrange (majority of arrange happens in beforeEach)       
        _formTypeStub.returns(2);

        //Act
        AsyncRequestResponseSample.CheckForUpdateFromAzureCode.onReady();

        //Assert
        expectResponse(this.requests, true);
        expect(_fakeResult.append).toHaveBeenCalledWith("Loading...");
    });
});