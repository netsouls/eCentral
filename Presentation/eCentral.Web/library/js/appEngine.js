// make console.log safe to use
window.console || (console = { log: function () { } });
if (window.appEngine == undefined) { window.appEngine = {}; }

//------------- Options for Supr - admin tempalte -------------//
var supr_Options = {
    fixedWidth: false, //activate fixed version with true
    rtl: false, //activate rtl version with true
    showSwitcher: false//show switcher with true
}
//------------- Modernizr -------------//
//load some plugins only if is needed
Modernizr.load({
    test: Modernizr.placeholder,
    nope: '/library/js/plugins/forms/placeholder/jquery.placeholder.min.js',
    complete: function () {
        //------------- placeholder fallback  -------------//
        $('input[placeholder], textarea[placeholder]').placeholder();
    }
});
Modernizr.load({
    test: Modernizr.touch,
    yep: ['/plugins/fix/ios-fix/ios-orientationchange-fix.js', '/plugins/fix/touch-punch/jquery.ui.touch-punch.min.js']
});

$.extend(appEngine, {
    global: {
        isDebug: 1,
        emptyGUID: '00000000-0000-0000-0000-000000000000'
    },
    util: {
        consoleMessage: function (message) {
            if (appEngine.global.isDebug == 1) {
                if (!window.console) console = {};
                console.log(message);
            }
        },
        getQueryStringValue: function (name) {
            var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
            return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
        },
        padLeft: function (string, padMask) {
            string = '' + string; // If it ain't a string, make it one (ye olde type coercion!)
            return (padMask.substr(0, (padMask.length - string.length)) + string);
        },
        stringformat: function (source, params) {
            if (arguments.length > 2 && params.constructor != Array) {
                params = $.makeArray(arguments).slice(1);
            }
            if (params.constructor != Array) {
                params = [params];
            }
            $.each(params, function (i, n) {
                source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
            });
            return source;
        },
        regExMatch: function (value, expression) {
            var match = new RegExp(expression).exec(value);
            return (match && (match.index === 0) && (match[0].length === value.length));
        },
        stringToJSON: function (stringData) {
            return jQuery.parseJSON(stringData); // another option is eval("(" + stringData + ")")
        },
        addTrailingSlash: function (strValue) {
            if (strValue.substr(-1) != '/') {
                strValue += '/';
            }

            return strValue;
        },
        removeFromArray: function (source, valuetoRemove, seperator) {
            seperator = seperator || ",";
            var sourceArray = source.split(seperator);
            for (var iKey = 0; iKey < sourceArray.length; iKey++) {
                if (sourceArray[iKey] == valuetoRemove) {
                    sourceArray.splice(iKey, 1);
                    return sourceArray.join(seperator);
                }
            }

            return source;
        },
        truefalseToBoolean: function (value) {
            if (value == "true")
                return true;

            return false;
        },
        ajaxPost: function (serviceUrl, model, onSuccess, onBefore, onComplete) {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: serviceUrl,
                data: JSON.stringify(model),
                beforeSend: function (xmlObject) {
                    if ($.isFunction(onBefore))
                        onBefore();
                },
                complete: function () {
                    if ($.isFunction(onComplete))
                        onComplete();
                },
                success: function (result) {
                    if ($.isFunction(onSuccess))
                        onSuccess(result);
                },
                error: function (xhr, err) {
                    var ajaxErrorMessage = "";
                    if (xhr.readyState != null)
                        ajaxErrorMessage += "readyState: " + xhr.readyState + "\n";
                    if (xhr.status != null)
                        ajaxErrorMessage += "status: " + xhr.status + "\n";
                    if (xhr.responseText != null)
                        ajaxErrorMessage += "responseText: " + xhr.responseText + "\n";
                    alert(ajaxErrorMessage);
                    appEngine.util.consoleMessage(ajaxErrorMessage);
                }
            });
        },
        setqTip: function (elements) {
            $(elements).qtip({
                content: false,
                position: {
                    my: 'bottom center',
                    at: 'top center',
                    viewport: $(window)
                },
                style: {
                    classes: 'qtip-tipsy'
                }
            });
        },
        setDataTable: function (elementId, serviceUrl, pushDataCallback, columnStructure, bLenghtChange, bFilter, callback) {
            $(document).ready(function () {
                if ($('table').hasClass('dynamicTable')) {
                    var $ajaxLoader = $('<img src="' + appEngine.path.root + 'library/images/loaders/horizontal/024.gif" width="80" height="10" alt=""/>');
                    $('.dynamicTable').parent().parent().parent().find('.title h4.clearfix .processing').html($ajaxLoader);
                    var dataTable = $('#' + elementId).dataTable({
                        'sServerMethod': 'POST',
                        'sAjaxSource': serviceUrl,
                        'sDom': "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
                        'sPaginationType': 'bootstrap',
                        'bJQueryUI': false,
                        'bAutoWidth': false,
                        'bServerSide': true,
                        'bFilter': (typeof bFilter === "undefined") ? true : bFilter,
                        'bSort': false,
                        'bLengthChange': (typeof bLenghtChange === "undefined") ? true : bLenghtChange,
                        'bProcessing': false,
                        'oLanguage': {
                            'sSearch': '<span>Filter:</span> _INPUT_',
                            'sLengthMenu': '<span>_MENU_ entries</span>',
                            'oPaginate': { 'sFirst': 'First', 'sLast': 'Last' }
                        },
                        'aoColumns': columnStructure,
                        'fnServerData': function (sSource, aoData, fnCallback, oSettings) {
                            if ($.isFunction(pushDataCallback))
                                pushDataCallback(aoData);
                            oSettings.jqXHR = $.ajax({
                                'dataType': 'json',
                                'beforeSend': function () {
                                    $ajaxLoader.show();
                                },
                                'complete': function () {
                                    $ajaxLoader.hide();
                                },
                                'type': 'POST',
                                'url': sSource,
                                'data': aoData,
                                'success': function (data) {
                                    fnCallback(data);
                                }
                            });
                        },
                        'fnDrawCallback': function (oSettings) {
                            if (oSettings._iRecordsTotal <= oSettings._iDisplayLength) { // hide pagination if records are for one page only
                                $('.dataTables_paginate, .dataTables_info').hide();
                            }
                            else {
                                $('.dataTables_paginate, .dataTables_info').show();
                            }

                            if (oSettings._iRecordsTotal == 0) {
                                $('.edit-action, .change-status').hide();
                            }
                            else
                                $('.edit-action, .change-status').show();
                            $('td.chChildren input:checkbox').uniform();
                            appEngine.util.setqTip('td .tip');
                        },
                        'fnInitComplete': function (oSettings, json) {
                            $('#' + elementId + ' tr').live('click', function () {
                                $(this).toggleClass('row_selected')
                            });
                            if ($.isFunction(callback))
                                callback(dataTable);
                        }
                    });
                    $('.dataTables_length select').uniform(); //uniform style
                    dataTable.fnFilterOnReturn();

                    /* all checkboxes */
                    $("#masterCh").click(function () {
                        var checkedStatus = $(this).find('span').hasClass('checked');
                        $("td.chChildren input:checkbox").each(function () {
                            this.checked = checkedStatus;
                            if (checkedStatus == this.checked) {
                                $(this).closest('.checker > span').removeClass('checked');
                            }
                            if (this.checked) {
                                $(this).closest('.checker > span').addClass('checked');
                            }
                            $(this).closest('tr').removeClass('row_selected');
                            if (checkedStatus)
                                $(this).closest('tr').addClass('row_selected');
                        });
                    });

                    /* edit action */
                    $('.edit-action').click(function () {
                        if (dataTable.$('tr.row_selected').length != 1)
                            appEngine.util.errorNotification('Please select one record to edit!');
                        else
                            window.location.href = appEngine.util.addTrailingSlash($(this).attr('data-href')) + $('tr.row_selected input:checkbox').val();
                    });
                    /* change status*/
                    $('.change-status-action').click(function () {
                        if (dataTable.$('tr.row_selected').length == 0)
                            appEngine.util.errorNotification('Please select the record(s) that you want to change the status!');
                        else {
                            var selectedRows = new Array();
                            $('tr.row_selected input:checked').each(function () {
                                if (this.value != 'all')
                                    selectedRows.push(this.value);
                            });

                            appEngine.util.ajaxPost($(this).attr('data-href'),
                            {
                                'StatusId': $(this).attr('data-value'),
                                'RowIds': selectedRows
                            }, function (result) {
                                $("#masterCh span.checked").removeClass('checked');
                                appEngine.util.successNotification(result);
                                dataTable.fnDraw();
                            });
                        }
                    });
                }
            });
        },
        setModalAction: function (callback) {
            $(document).ready(function () {
                if ($('#modal-form-container').length < 1) {
                    var $modalContainer = $('<div id="modal-form-container" class="modal hide fade" style="display: none;"><div id="modal-form"></div></div>');
                    $('body').append($modalContainer);
                }
                $('.modal-action').live('click', function (e) {
                    e.preventDefault();
                    $.get($(this).attr('data-url'), function (data) {
                        $('#modal-form').html(data);
                        $('#modal-form-container').modal('show');
                        jQuery.validator.unobtrusive.parse($('#modal-form'));
                        appEngine.util.iToggleButton('#modal-form ');
                    });
                });

                if ($.isFunction(callback))
                    callback();
            });
        },
        setModalActionCallback: function (dataResult, onSuccess, onFailure) {
            var jsonData = appEngine.util.stringToJSON(dataResult.responseText);
            if (jsonData.IsValid) {
                $('#modal-form-container').modal('hide');
                appEngine.util.successNotification('Changes saved.');
                if ($.isFunction(onSuccess))
                    onSuccess();
            }
            else {
                // show error notification
                if (jsonData.errorMessage) {
                    $('#modal-form-container').modal('hide');
                    appEngine.util.errorNotification(jsonData.errorMessage);
                }
                else {
                    $('#modal-form').html(jsonData.htmlData);
                }
                if ($.isFunction(onFailure))
                    onFailure();
            }
        },
        statesByCountry: function ($country, $states) {
            /* Initialize */
            $country.html('<option></option>' + $country.html());
            $country.select2({ placeholder: appEngine.i18n.Address_SelectCountry, allowClear: true });
            if ($country.val().length < 1)
                $states.html('<option></option>').select2({ placeholder: appEngine.i18n.Address_SelectState });
            else {
                $states.html('<option></option>' + $states.html());
                $states.select2({ placeholder: appEngine.i18n.Address_SelectSate, allowClear: true });
            }

            /* get records */
            $country.on('change', function (e) {
                $states.select2("destroy");
                $.ajax({
                    cache: false,
                    type: "GET",
                    url: "/api/statesbycountry",
                    data: { "countryId": e.val, "addEmptyStateIfRequired": "true" },
                    success: function (data) {
                        $states.html('<option></option>');
                        if (data) {
                            $.each(data, function (id, option) {
                                $states.append($('<option></option>').val(option.id).html(option.name));
                            });
                        }
                        $states.select2({ placeholder: appEngine.i18n.Address_SelectState, allowClear: true });
                    }
                });
            });
        },
        iToggleButton: function (parentElement) {
            $(parentElement + '.iToggle-button').toggleButtons({
                onChange: function (el, status, e) {
                    if (parentElement == '')
                        $('#' + $(el).parent().parent().find('input:checkbox').attr('data-element')).val(status);
                    else
                        $(parentElement + '#' + $(parentElement).find(el).find('input').attr('data-element')).val(status);
                },
                width: 70,
                label: {
                    enabled: "<span class='icon16 icomoon-icon-checkmark white'></span>",
                    disabled: "<span class='icon16 icomoon-icon-close white marginL10'></span>"
                }
            });
        },
        setNotification: function (message, icons, notificationType, title) {
            $.pnotify({
                type: notificationType,
                title: (typeof title === "undefined") ? '' : title,
                text: message,
                icon: 'picon icon16 ' + icons,
                opacity: 0.95,
                sticker: false,
                history: false
            });
        },
        noticeNotification: function (message, title) {
            appEngine.util.setNotification(message, 'entypo-icon-warning', '', title);
        },
        infoNotification: function (message, title) {
            appEngine.util.setNotification(message, 'brocco-icon-info', 'info', title);
        },
        successNotification: function (message, title) {
            appEngine.util.setNotification(message, 'iconic-icon-check-alt', 'success', title);
        },
        errorNotification: function (message, title) {
            appEngine.util.setNotification(message, 'typ-icon-cancel', 'error', title);
        }
    }
});
var userAgent = navigator.userAgent.toLowerCase();
$.browser.chrome = /chrome/.test(navigator.userAgent.toLowerCase());

$(document).ready(function () {

    //------------- Switcher code ( Remove it in production site ) -------------//
    (function () {
        supr_switcher = {
            create: function () {
                //create switcher and inject into html
                $('body').append('<a href="#" id="switchBtn"><span class="icon24 icomoon-icon-cogs"></span></a>');
                $('body').append('<div id="switcher"><h4>Header patterns</h4><div class="header-patterns"><ul><li><a href="#" class="hpat1"><img src="' + appEngine.path.root + 'library/images/patterns/header/1.png"></a></li><li><a href="#" class="hpat2"><img src="' + appEngine.path.root + 'library/images/patterns/header/2.png"></a></li><li><a href="#" class="hpat3"><img src="' + appEngine.path.root + 'library/images/patterns/header/3.png"></a></li><li><a href="#" class="hpat4"><img src="' + appEngine.path.root + 'library/images/patterns/header/4.png"></a></li></ul></div><h4>Sidebar patterns</h4><div class="sidebar-patterns"><ul><li><a href="#" class="spat1"><img src="' + appEngine.path.root + 'library/images/patterns/sidebar/1.png"></a></li><li><a href="#" class="spat2"><img src="' + appEngine.path.root + 'library/images/patterns/sidebar/2.png"></a></li><li><a href="#" class="spat3"><img src="' + appEngine.path.root + 'library/images/patterns/sidebar/3.png"></a></li><li><a href="#" class="spat4"><img src="' + appEngine.path.root + 'library/images/patterns/sidebar/4.png"></a></li></ul></div><h4>Body patterns</h4><div class="body-patterns"><ul><li><a href="#" class="bpat1"><img src="' + appEngine.path.root + 'library/images/patterns/body/1.png"></a></li><li><a href="#" class="bpat2"><img src="' + appEngine.path.root + 'library/images/patterns/body/2.png"></a></li><li><a href="#" class="bpat3"><img src="' + appEngine.path.root + 'library/images/patterns/body/3.png"></a></li><li><a href="#" class="bpat4"><img src="' + appEngine.path.root + 'library/images/patterns/body/4.png"></a></li></ul></div></div>');
            },
            init: function () {
                supr_switcher.create();
                $('#switcher a').click(function () {
                    if ($(this).hasClass('hpat1')) { $('#header').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/header/bedge_grunge.png)'); }
                    if ($(this).hasClass('hpat2')) { $('#header').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/header/grid.png)'); }
                    if ($(this).hasClass('hpat3')) { $('#header').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/header/nasty_fabric.png)'); }
                    if ($(this).hasClass('hpat4')) { $('#header').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/header/natural_paper.png)'); }
                    if ($(this).hasClass('spat1')) { $('#sidebarbg').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/sidebar/az_subtle.png)'); }
                    if ($(this).hasClass('spat2')) { $('#sidebarbg').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/sidebar/billie_holiday.png)'); }
                    if ($(this).hasClass('spat3')) { $('#sidebarbg').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/sidebar/grey.png)'); }
                    if ($(this).hasClass('spat4')) { $('#sidebarbg').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/sidebar/noise_lines.png)'); }
                    if ($(this).hasClass('bpat1')) { $('#content').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/body/cream_dust.png)'); }
                    if ($(this).hasClass('bpat2')) { $('#content').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/body/dust.png)'); }
                    if ($(this).hasClass('bpat3')) { $('#content').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/body/grey.png)'); }
                    if ($(this).hasClass('bpat4')) { $('#content').css('background', 'url(' + appEngine.path.root + 'library/images/patterns/body/subtle_dots.png)'); }
                });

                $('#switchBtn').click(function () {
                    if ($(this).hasClass('toggle')) {
                        //hide switcher
                        $(this).removeClass('toggle').css('right', '-1px');
                        $('#switcher').css('display', 'none');

                    } else {
                        //expand switcher
                        $(this).animate({
                            right: '135'
                        }, 200, function () {
                            // Animation complete.
                            $('#switcher').css('display', 'block');
                            $(this).addClass('toggle');
                        });
                    }
                });
            }
        }
    })();

    //show switcher
    if (supr_Options.showSwitcher) {
        supr_switcher.init();
    }
    //make template fixed width
    if (supr_Options.fixedWidth) {
        $('body').addClass('fixedWidth');
        $('#header .container-fluid').addClass('container').removeClass('container-fluid');
        $('#wrapper').addClass('container');
    }

    $('.search-btn').addClass('nostyle'); //tell uniform to not style this element

    //Disable certain links
    $('a[href^=#]').click(function (e) {
        e.preventDefault()
    })

    //------------- Navigation -------------//
    mainNav = $('.mainnav>ul>li');
    mainNav.find('ul').siblings().addClass('hasUl').append('<span class="hasDrop icon16 icomoon-icon-arrow-down-2"></span>');
    mainNavLink = mainNav.find('a').not('.sub a');
    mainNavLinkAll = mainNav.find('a');
    mainNavSubLink = mainNav.find('.sub a').not('.sub li .sub a');
    mainNavCurrent = mainNav.find('a.current');

    /*Auto current system in main navigation */
    var domain = document.domain;
    var absoluteUrl = 0; //put value of 1 if use absolute path links. example http://www.host.com/dashboard instead of /dashboard

    function setCurrentClass(mainNavLinkAll, url) {
        url = url.toLowerCase();
        mainNavLinkAll.each(function (index) {
            //convert href to array and get last element
            var href = $(this).attr('href').toLowerCase();
            if (href == url || url.indexOf(href) > 0) {
                //set new current class
                $(this).addClass('current');

                ulElem = $(this).closest('ul');
                if (ulElem.hasClass('sub')) {
                    //its a part of sub menu need to expand this menu
                    aElem = ulElem.prev('a.hasUl').addClass('drop');
                    ulElem.addClass('expand');
                }
            }
        });
    }

    if (domain === '') {
        //domain not found
        var pageUrl = window.location.pathname.split('/');
        var winLoc = pageUrl.pop(); // get last item
        setCurrentClass(mainNavLinkAll, winLoc);

    } else {
        if (absoluteUrl === 0) {
            //absolute url is disabled
            var afterDomain = window.location.pathname;
            //afterDomain = afterDomain.replace('/', '');
            setCurrentClass(mainNavLinkAll, afterDomain);
        } else {
            //absolute url is enabled
            var newDomain = 'http://' + domain + window.location.pathname;
            setCurrentClass(mainNavLinkAll, newDomain);
        }
    }

    //hover magic add blue color to icons when hover - remove or change the class if not you like.
    mainNavLinkAll.hover(
	  function () {
	      $(this).find('span.icon16').addClass('blue');
	  },
	  function () {
	      $(this).find('span.icon16').removeClass('blue');
	  }
	);

    //click magic
    mainNavLink.click(function (event) {
        $this = $(this);
        if ($this.hasClass('hasUl')) {
            event.preventDefault();
            if ($this.hasClass('drop')) {
                $(this).siblings('ul.sub').slideUp(250).siblings().toggleClass('drop');
            } else {
                $(this).siblings('ul.sub').slideDown(250).siblings().toggleClass('drop');
            }
        }
    });
    mainNavSubLink.click(function (event) {
        $this = $(this);
        if ($this.hasClass('hasUl')) {
            event.preventDefault();
            if ($this.hasClass('drop')) {
                $(this).siblings('ul.sub').slideUp(250).siblings().toggleClass('drop');
            } else {
                $(this).siblings('ul.sub').slideDown(250).siblings().toggleClass('drop');
            }
        }
    });

    //responsive buttons
    $('.resBtn>a').click(function (event) {
        $this = $(this);
        if ($this.hasClass('drop')) {
            $this.removeClass('drop');
        } else {
            $this.addClass('drop');
        }
        if ($('#sidebar').length) {
            $('#sidebar').toggleClass('offCanvas');
            $('#sidebarbg').toggleClass('offCanvas');
            if ($('#sidebar-right').length) {
                $('#sidebar-right').toggleClass('offCanvas');
            }
        }
        if ($('#sidebar-right').length) {
            $('#sidebar-right').toggleClass('offCanvas');
            $('#sidebarbg-right').toggleClass('offCanvas');
        }
        $('#content').toggleClass('offCanvas');
        if ($('#content-one').length) {
            $('#content-one').toggleClass('offCanvas');
        }
    });

    $('.resBtnSearch>a').click(function (event) {
        $this = $(this);
        if ($this.hasClass('drop')) {
            $('.search').slideUp(250);
        } else {
            $('.search').slideDown(250);
        }
        $this.toggleClass('drop');
    });

    //Hide and show sidebar btn
    $(function () {
        //var pages = ['grid.html','charts.html'];
        var pages = [];
        for (var i = 0, j = pages.length; i < j; i++) {
            if ($.cookie("currentPage") == pages[i]) {
                var cBtn = $('.collapseBtn.leftbar');
                cBtn.children('a').attr('title', 'Show Left Sidebar');
                cBtn.addClass('shadow hide');
                cBtn.css({ 'top': '20px', 'left': '200px' });
                $('#sidebarbg').css('margin-left', '-299' + 'px');
                $('#sidebar').css('margin-left', '-299' + 'px');
                if ($('#content').length) {
                    $('#content').css('margin-left', '0');
                }
                if ($('#content-two').length) {
                    $('#content-two').css('margin-left', '0');
                }
            }
        }

    });

    $('.collapseBtn').bind('click', function () {
        $this = $(this);

        //left sidbar clicked
        if ($this.hasClass('leftbar')) {

            if ($(this).hasClass('hide')) {
                //show sidebar
                $this.removeClass('hide');
                $this.children('a').attr('title', 'Hide Left Sidebar');

            } else {
                //hide sidebar
                $this.addClass('hide');
                $this.children('a').attr('title', 'Show Left Sidebar');
            }
            $('#sidebarbg').toggleClass('hided');
            $('#sidebar').toggleClass('hided')
            $('.collapseBtn.leftbar').toggleClass('top shadow');
            //expand content

            if ($('#content').length) {
                $('#content').toggleClass('hided');
            }
            if ($('#content-two').length) {
                $('#content-two').toggleClass('hided');
            }

        }

        //right sidebar clicked
        if ($this.hasClass('rightbar')) {

            if ($(this).hasClass('hide')) {
                //show sidebar
                $this.removeClass('hide');
                $this.children('a').attr('title', 'Hide Right Sidebar');

            } else {
                //hide sidebar
                $this.addClass('hide');
                $this.children('a').attr('title', 'Show Right Sidebar')
            }
            $('#sidebarbg-right').toggleClass('hided');
            $('#sidebar-right').toggleClass('hided');
            if ($('#content').length) {
                $('#content').toggleClass('hided-right');
            }
            if ($('#content-one').length) {
                $('#content-one').toggleClass('hided');
            }
            if ($('#content-two').length) {
                $('#content-two').toggleClass('hided-right');
            }
            $('.collapseBtn.rightbar').toggleClass('top shadow')
        }
    });

    //------------- widget box magic -------------//
    var widget = $('div.box');
    var widgetOpen = $('div.box').not('div.box.closed');
    var widgetClose = $('div.box.closed');
    //close all widgets with class "closed"
    widgetClose.find('div.content').hide();
    widgetClose.find('.title>.minimize').removeClass('minimize').addClass('maximize');

    widget.find('.title>a').click(function (event) {
        event.preventDefault();
        var $this = $(this);
        if ($this.hasClass('minimize')) {
            //minimize content
            $this.removeClass('minimize').addClass('maximize');
            $this.parent('div').addClass('min');
            cont = $this.parent('div').next('div.content')
            cont.slideUp(500, 'easeOutExpo'); //change effect if you want :)

        } else
            if ($this.hasClass('maximize')) {
                //minimize content
                $this.removeClass('maximize').addClass('minimize');
                $this.parent('div').removeClass('min');
                cont = $this.parent('div').next('div.content');
                cont.slideDown(500, 'easeInExpo'); //change effect if you want :)
            }
    })

    //show minimize and maximize icons
    widget.hover(function () {
        $(this).find('.title>a').show(50);
    }, function () {
        $(this).find('.title>a').hide();
    });

    //add shadow if hover box
    widget.not('.drag').hover(function () {
        $(this).addClass('hover');
    }, function () {
        $(this).removeClass('hover');
    });

    //------------- Masked input fields -------------//
    $(".mask-phone").mask("(999) 999-9999", { completed: function () { alert("Callback action after complete"); } });
    $(".mask-mobile").mask("(+99) 99999-99999");
    $(".mask-phoneExt").mask("(999) 999-9999? x99999");
    $(".mask-phoneInt").mask("(+99) ?999 99999999");
    $(".mask-date").mask("99/99/9999");
    $(".mask-ssn").mask("999-99-9999");
    $(".mask-productKey").mask("a*-999-a999", { placeholder: "*" });
    $(".mask-eyeScript").mask("~9.99 ~9.99 999");
    $(".mask-percent").mask("99%");

    //------------- Toggle button  -------------//
    $('.normal-toggle-button').toggleButtons({
        onChange: function (el, status, e) {
            $('#' + el.find('input:checkbox').attr('data-element')).val(status);
        },
        label: {
            enabled: "Yes", disabled: "No"
        }
    });
    appEngine.util.iToggleButton('');
    //------------- To top plugin  -------------//
    $().UItoTop({ easingType: 'easeOutQuart' });

    //------------- Search forms  submit handler  -------------//
    if ($('#search_input').length) {
        //make custom redirect for search form in .heading
        $('#search-form').submit(function () {
            var sText = $('.top-search').val();
            var sAction = $(this).attr('action');
            var sUrl = sAction + '?q=' + sText;
            $(location).attr('href', sUrl);
            return false;
        });
    }

    //------------- Tooltips -------------//
    //top tooltip
    appEngine.util.setqTip('.tip');

    //tooltip in right
    $('.tipR').qtip({
        content: false,
        position: {
            my: 'left center',
            at: 'right center',
            viewport: $(window)
        },
        style: {
            classes: 'qtip-tipsy'
        }
    });

    //tooltip in bottom
    $('.tipB').qtip({
        content: false,
        position: {
            my: 'top center',
            at: 'bottom center',
            viewport: $(window)
        },
        style: {
            classes: 'qtip-tipsy'
        }
    });

    //tooltip in left
    $('.tipL').qtip({
        content: false,
        position: {
            my: 'right center',
            at: 'left center',
            viewport: $(window)
        },
        style: {
            classes: 'qtip-tipsy'
        }
    });

    //------------- Jrespond -------------//
    var jRes = jRespond([
        {
            label: 'small',
            enter: 0,
            exit: 1000
        }, {
            label: 'desktop',
            enter: 1001,
            exit: 10000
        }
    ]);

    jRes.addFunc({
        breakpoint: 'small',
        enter: function () {
            $('#sidebarbg,#sidebar,#content').removeClass('hided');
        },
        exit: function () {
            $('.collapseBtn.top.hide').removeClass('top hide');
        }
    });

    //------------- Uniform  -------------//
    //add class .nostyle if not want uniform to style field
    $("input, textarea, select").not('.nostyle').uniform();

    // -- spinners --//
    $('.spinner').spinner();

    //remove overlay and show page
    $("#qLoverlay").fadeOut(250);
    $("#qLbar").fadeOut(250);

    //------------- Application specific actions -------------//
    $("form input").attr("autocomplete", "off");
    $('.multi-select').select2(
    {
        placeholder: $(this).attr('placeholder')
    }).on('change', function (e) {
        $('#' + $(this).attr('dataField')).val(e.val);
    });

    $('.href-action').click(function (e) {
        e.preventDefault();
        window.location.href = $(this).attr('data-href');
    });

    if ($('form #IsEdit').length) {
        if ($('form #IsEdit').val().toLowerCase() == 'true') {
            $('form .readonly').attr('readonly', 'readonly');
            if ($('.multi-select-hidden').length > 0) {
                $.each($('.multi-select-hidden'), function (index, element) {
                    $('#' + element.id + '_List').select2('val', $(element).val().split(','));
                });
            }
        }
    }
    //------------- Datepicker -------------//
    if ($('.datepicker').length) {
        $(".datepicker").datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            dateFormat: 'd MM, y'
        });
    }

    //* modal window confirmation*/
    $('a[data-confirm]').click(function (ev) {
        if ($('#modal-confirmation-container').length < 1) {
            $('body').append('<div id="modal-confirmation-container" class="modal modal-confirmation hide fade" style="display: none;"><div class="modal-header"><button type="button" class="close" data-dismiss="modal"><span class="icon12 minia-icon-close"></span></button><h3>Please confirm</h3></div><form class="modal-form" method="post"><div class="modal-body"></div><div class="modal-footer"><button class="btn btn-mini" data-dismiss="modal">Cancel</button><button type="submit" class="btn btn-mini btn-danger" value="delete">OK</button></div></form></div>');
        }
        $('#modal-confirmation-container').find('.modal-body').text($(this).attr('data-confirm'))
            .parent('form').attr('action', $(this).attr('href'));
        $('#modal-confirmation-container').modal('show');
        return false;
    });
});

//window resize events
$(window).resize(function () {
    //get the window size
    var wsize = $(window).width();
    if (wsize > 980) {
        $('.shortcuts.hided').removeClass('hided').attr("style", "");
        $('.sidenav.hided').removeClass('hided').attr("style", "");
    }

    var size = "Window size is:" + $(window).width();
});
$(window).load(function () {
    var wheight = $(window).height();
    $('#sidebar.scrolled').css('height', wheight - 63 + 'px');
});

/**
Custom obstructive validation method
**/
$.validator.setDefaults({ ignore: '' }); // ignored for validating hidden field also
(function ($) {
    $.validator.unobtrusive.adapters.add('equalischecked', ['other'], function (options) {
        options.rules['equalischecked'] = options.params;
        if (options.message != null)
            options.messages['equalischecked'] = options.message;

    });

    $.validator.addMethod('equalischecked', function (value, element, params) {
        if ($(element).is(':checkbox')) {
            return $(element).is(':checked');
        }
        return false;
    });

    $.validator.unobtrusive.adapters.addSingleVal("nomatchregex", "nomatchpattern");

    $.validator.addMethod("nomatchregex", function (value, element, params) {
        var match;
        if (this.optional(element)) {
            return true;
        }
        match = new RegExp(params).exec(value);
        return (!match);
    });
})(jQuery);