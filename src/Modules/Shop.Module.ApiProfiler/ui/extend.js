﻿this.listInitStat = function (t) {
    function u(n) {
        var e = function (n, t) {
            return n.ClientTimings.Timings.filter(function (n) {
                return n.Name === t
            })[0] || {Name: t, Duration: "", Start: ""}
        };
        r.ajax({
            url: f.path + "results-stat-list" + f.queryString,
            data: {"last-id": n},
            dataType: "json",
            type: "GET",
            success: function (f) {
                var s = "", h, o;
                f.forEach(function (n) {
                    s += '\n<tr>\n  <td>' + n.Name + "<\/td>\n  <td>" + n.TotalCount + '<\/td>\n  ' + " <td>" + n.MaxDurationMilliseconds + "<\/td>" + " <td>" + n.MinDurationMilliseconds + "<\/td>" + " <td>" + n.AvgDurationMilliseconds + "<\/td>" + "\n<\/tr>"
                });
                r("table tbody").append(s);
                h = n;
                o = f;
            }
        })
    }

    var i = n, r = i.jq, f = n.options = t || {};
    u()
};