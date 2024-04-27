async function Swalfire(options) {
    return await Swal.fire({
        heightAuto: false,
        customClass: {
            confirmButton: "px-5 rounded-5 btn btn-success ms-2 shadow-sm",
            cancelButton: "px-3 rounded-5 btn btn-danger ms-2 shadow-sm",
            popup: "rounded-5",
            input: "rounded-5"
        },
        buttonsStyling: false,
        ...options
    });
}

function Swalmixin(options) {
    return Swal.mixin({
        heightAuto: false,
        customClass: {
            confirmButton: "px-5 rounded-5 btn btn-success ms-2 shadow-sm",
            cancelButton: "px-3 rounded-5 btn btn-danger ms-2 shadow-sm",
            popup: "rounded-5"
        },
        buttonsStyling: false,
        ...options
    });
}

$(document).ready(function () {
    adjustMainParentHeight();
    $(window).on('resize', adjustMainParentHeight);

    $(document).on('change', function () {
        adjustMainParentHeight();
    });


});

function adjustMainParentHeight() {
    var headerHeight = $('header').outerHeight();
    var footerHeight = $('footer').outerHeight();
    var mainParent = $('.main-parent');
    var windowHeight = $(window).innerHeight();
    mainParent.css('min-height', (windowHeight - headerHeight - footerHeight) + 'px');

    $('.headerH').css('height', headerHeight + 'px');

    $('.footerH').css('height', footerHeight + 'px');

    mainParent.css('height', $(window).height() - headerHeight - footerHeight);
}