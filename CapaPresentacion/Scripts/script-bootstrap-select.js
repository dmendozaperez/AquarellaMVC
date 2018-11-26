$(document).ready(function () 
{
    // Enable Live Search.
    $('#CountryList').attr('data-live-search', true);

    $('.selectCountry').selectpicker(
    {
        width: '100%',
        title: '- [Eliga un items] -',
        style: 'btn-default',
        size: 6
    });
});  

