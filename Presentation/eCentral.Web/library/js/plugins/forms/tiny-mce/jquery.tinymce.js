(function (c) { var b, e, a = [], d = window; c.fn.tinymce = function (j) { var p = this, g, k, h, m, i, l = "", n = ""; if (!p.length) { return p } if (!j) { return tinyMCE.get(p[0].id) } p.css("visibility", "hidden"); function o() { var r = [], q = 0; if (f) { f(); f = null } p.each(function (t, u) { var s, w = u.id, v = j.oninit; if (!w) { u.id = w = tinymce.DOM.uniqueId() } s = new tinymce.Editor(w, j); r.push(s); s.onInit.add(function () { var x, y = v; p.css("visibility", ""); if (v) { if (++q == r.length) { if (tinymce.is(y, "string")) { x = (y.indexOf(".") === -1) ? null : tinymce.resolve(y.replace(/\.\w+$/, "")); y = tinymce.resolve(y) } y.apply(x || tinymce, r) } } }) }); c.each(r, function (t, s) { s.render() }) } if (!d.tinymce && !e && (g = j.script_url)) { e = 1; h = g.substring(0, g.lastIndexOf("/")); if (/_(src|dev)\.js/g.test(g)) { n = "_src" } m = g.lastIndexOf("?"); if (m != -1) { l = g.substring(m + 1) } d.tinyMCEPreInit = d.tinyMCEPreInit || { base: h, suffix: n, query: l }; if (g.indexOf("gzip") != -1) { i = j.language || "en"; g = g + (/\?/.test(g) ? "&" : "?") + "js=true&core=true&suffix=" + escape(n) + "&themes=" + escape(j.theme) + "&plugins=" + escape(j.plugins) + "&languages=" + i; if (!d.tinyMCE_GZ) { tinyMCE_GZ = { start: function () { tinymce.suffix = n; function q(r) { tinymce.ScriptLoader.markDone(tinyMCE.baseURI.toAbsolute(r)) } q("langs/" + i + ".js"); q("themes/" + j.theme + "/editor_template" + n + ".js"); q("themes/" + j.theme + "/langs/" + i + ".js"); c.each(j.plugins.split(","), function (s, r) { if (r) { q("plugins/" + r + "/editor_plugin" + n + ".js"); q("plugins/" + r + "/langs/" + i + ".js") } }) }, end: function () { } } } } c.ajax({ type: "GET", url: g, dataType: "script", cache: true, success: function () { tinymce.dom.Event.domLoaded = 1; e = 2; if (j.script_loaded) { j.script_loaded() } o(); c.each(a, function (q, r) { r() }) } }) } else { if (e === 1) { a.push(o) } else { o() } } return p }; c.extend(c.expr[":"], { tinymce: function (g) { return !!(g.id && "tinyMCE" in window && tinyMCE.get(g.id)) } }); function f() { function i(l) { if (l === "remove") { this.each(function (n, o) { var m = h(o); if (m) { m.remove() } }) } this.find("span.mceEditor,div.mceEditor").each(function (n, o) { var m = tinyMCE.get(o.id.replace(/_parent$/, "")); if (m) { m.remove() } }) } function k(n) { var m = this, l; if (n !== b) { i.call(m); m.each(function (p, q) { var o; if (o = tinyMCE.get(q.id)) { o.setContent(n) } }) } else { if (m.length > 0) { if (l = tinyMCE.get(m[0].id)) { return l.getContent() } } } } function h(m) { var l = null; (m) && (m.id) && (d.tinymce) && (l = tinyMCE.get(m.id)); return l } function g(l) { return !!((l) && (l.length) && (d.tinymce) && (l.is(":tinymce"))) } var j = {}; c.each(["text", "html", "val"], function (n, l) { var o = j[l] = c.fn[l], m = (l === "text"); c.fn[l] = function (s) { var p = this; if (!g(p)) { return o.apply(p, arguments) } if (s !== b) { k.call(p.filter(":tinymce"), s); o.apply(p.not(":tinymce"), arguments); return p } else { var r = ""; var q = arguments; (m ? p : p.eq(0)).each(function (u, v) { var t = h(v); r += t ? (m ? t.getContent().replace(/<(?:"[^"]*"|'[^']*'|[^'">])*>/g, "") : t.getContent({ save: true })) : o.apply(c(v), q) }); return r } } }); c.each(["append", "prepend"], function (n, m) { var o = j[m] = c.fn[m], l = (m === "prepend"); c.fn[m] = function (q) { var p = this; if (!g(p)) { return o.apply(p, arguments) } if (q !== b) { p.filter(":tinymce").each(function (s, t) { var r = h(t); r && r.setContent(l ? q + r.getContent() : r.getContent() + q) }); o.apply(p.not(":tinymce"), arguments); return p } } }); c.each(["remove", "replaceWith", "replaceAll", "empty"], function (m, l) { var n = j[l] = c.fn[l]; c.fn[l] = function () { i.call(this, l); return n.apply(this, arguments) } }); j.attr = c.fn.attr; c.fn.attr = function (o, q) { var m = this, n = arguments; if ((!o) || (o !== "value") || (!g(m))) { if (q !== b) { return j.attr.apply(m, n) } else { return j.attr.apply(m, n) } } if (q !== b) { k.call(m.filter(":tinymce"), q); j.attr.apply(m.not(":tinymce"), n); return m } else { var p = m[0], l = h(p); return l ? l.getContent({ save: true }) : j.attr.apply(c(p), n) } } } })(jQuery);
//--------------- Tinymce ------------------//
$('textarea.tinymce').tinymce({
    // Location of TinyMCE script
    script_url: '/library/js/plugins/forms/tiny-mce/tiny_mce.js',

    // General options
    theme: "advanced",
    plugins: "autolink,lists,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,advlist",

    // Theme options
    theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,cut,copy,paste,pastetext,pasteword,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,|,sub,sup,|,charmap,|,cleanup,help,code,fullscreen",
    //theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
    //theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen",
    //theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,pagebreak",
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "bottom",
    theme_advanced_resizing: true,

    // Example content CSS (should be your site CSS)
    content_css: "/library/css/main.css",

    // Drop lists for link/image/media/template dialogs
    template_external_list_url: "lists/template_list.js",
    external_link_list_url: "lists/link_list.js",
    external_image_list_url: "lists/image_list.js",
    media_external_list_url: "lists/media_list.js",

    // Replace values for the template plugin
    template_replace_values: {
        username: "SuprUser",
        staffid: "991234"
    }
});