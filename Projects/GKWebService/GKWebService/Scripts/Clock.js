function GetClock() {
    var d = new Date();
    var nhour = d.getHours(), nmin = d.getMinutes(), nsec = d.getSeconds();
    if (nhour <= 9) nhour = "0" + nhour;
    if (nmin <= 9) nmin = "0" + nmin;
    if (nsec <= 9) nsec = "0" + nsec;

    document.getElementById('clockbox').innerHTML = $.datepicker.formatDate("dd.mm.yy", d) + ' ' + nhour + ':' + nmin + ':' + nsec;
}

window.onload = function () {
    GetClock();
    setInterval(GetClock, 1000);
}