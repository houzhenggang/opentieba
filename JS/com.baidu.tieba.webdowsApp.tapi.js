/*
 *
 *   Lib for Node.js: BaiduTieba (com.baidu.tieba._sApp)
 *   Describe: 用于贴吧的API接口
 *   Date: 2013-12-11
 *
 */
(function () {
    var crypto = require('crypto');
    var http=require('http');
    var sendHttp=function(host,fupa,mot,post,port,hdd){
        http.request({
            host:host,
            port:port,
            method:mot,
            path:fupa
        },function(res){
            res.setEncoding('utf8');
            var chunks="";
            res.on('data', function (chunk) {
                chunks+=chunk;
            });
            res.on('end', function(){
                hdd({ztm:res.statusCode,body:chunks});
            });
        });
    };
    var md5 = function (str) {
        var md5sum = crypto.createHash('md5');
        md5sum.update(str);
        str = md5sum.digest('hex');
        return str;
    };
    function ifnoerror(q) {
        if(q.ztm!=200){
            return false;
        }
        if (q.body.match(/^\{.+\}$/)==null) {
            return false;
        } else {
            return true;
        }
    }

    var _s={};

    _s.tiebaApi = {};

    _s.tiebaApi.getBduss = function () {
        return "";
    };
    _s.tiebaApi.getUid = function () {
        return 0;
    };
    _s.tiebaApi.getUName = function () {
        return "";
    }

    _s.tiebaApi.sendTieba = function (fullpath, post, hdd) {
        sendHttp("c.tieba.baidu.com", fullpath, "POST", "BDUSS=" + encodeURIComponent(_s.tiebaApi.getBduss()) + "&_client_id=a0112ba8-b146-45c5-bb18-5b9fdde4917b&_client_type=4&_client_version=1.3.3&_phone_imei=" +
            "05-00-54-20-06-00-01-00-04-00-9C-35-01-00-26-28-02-00-24-14-09-00-32-53&net_type=3" + (function () {
            if (post != "") {
                return "&";
            } else {
                return "";
            }
        })() + post + "&sign=" + (function () {
            var allpost = String("BDUSS=" + encodeURIComponent(_s.tiebaApi.getBduss()) + "&_client_id=a0112ba8-b146-45c5-bb18-5b9fdde4917b&_client_type=4&_client_version=1.3.3&_phone_imei=" +
                "05-00-54-20-06-00-01-00-04-00-9C-35-01-00-26-28-02-00-24-14-09-00-32-53&net_type=3&" + post);
            var joinednoandargs = decodeURIComponent(allpost).split("&").sort().join("");
            return md5(joinednoandargs + "tiebaclient!!!");
        })(), 80, hdd);
    }
    _s.tiebaApi.getLikeKWs = function (hdd) {
        if (_s.tiebaApi.getBduss() == "") {
            hdd([]);
        }
        _s.tiebaApi.sendTieba("/c/f/forum/favolike", "pn=1", function (q) {
            if (ifnoerror(q)) {
                hdd(JSON.parse(q.body).forum_list);
            } else {
                hdd([]);
            }
        });
    }
    _s.tiebaApi.sign = function (kw, hdd) {
        _s.tiebaApi.userInfo(0, function (q) {
            _s.tiebaApi.sendTieba("/c/c/forum/sign", "kw=" + encodeURIComponent(kw) + "&tbs=" + encodeURIComponent(q.anti.tbs), function (q) {
                if (ifnoerror(q)) {
                    hdd(JSON.parse(q.body));
                } else {
                    hdd(false);
                }
            });
        })
    };
    _s.tiebaApi.like = function (kw, hdd) {
        _s.tiebaApi.userInfo(0, function (q) {
            _s.tiebaApi.sendTieba("/c/c/forum/like", "kw=" + encodeURIComponent(kw) + "&tbs=" + encodeURIComponent(q.anti.tbs), function (q) {
                if (ifnoerror(q)) {
                    hdd(JSON.parse(q.body));
                } else {
                    hdd(false);
                }
            })
        });
    };
    _s.tiebaApi.unlike = function (kw, hdd) {
        _s.tiebaApi.userInfo(0, function (q) {
            _s.tiebaApi.sendTieba("/c/c/forum/unlike", "kw=" + encodeURIComponent(kw) + "&tbs=" + encodeURIComponent(q.anti.tbs), function (q) {
                if (ifnoerror(q)) {
                    hdd(JSON.parse(q.body));
                } else {
                    hdd(false);
                }
            })
        });
    };
    _s.tiebaApi.delThread = function (kw, z, hdd) {
        _s.tiebaApi.userInfo(0, function (u) {
            _s.tiebaApi.sendTieba("/c/c/bawu/delthread", "z=" + Number(z) + "&word=" + encodeURIComponent(kw) + "&tbs=" + u.anti.tbs, function (q) {
                if (ifnoerror(q)) {
                    hdd(JSON.parse(q.body));
                } else {
                    hdd(false);
                }
            });
        });
    };
    _s.tiebaApi.userInfo = function (uid, hdd) {
        _s.tiebaApi.sendTieba("/c/u/user/profile", "uid=" + (function () {
            if (uid == undefined || uid == null || uid == 0 || uid == "" || Number(uid) != uid) {
                return _s.tiebaApi.getUid();
            }
            return uid;
        })(), function (q) {
            if (ifnoerror(q)) {
                var uinfo = JSON.parse(q.body);
                hdd(uinfo);
            }
        });
    };
    _s.tiebaApi.bar = function (barname, isgood, pn, hdd) {
        _s.tiebaApi.sendTieba("/c/f/frs/page", "kw=" + encodeURIComponent(barname) + "&is_good=" + isgood + "&pn=" + pn, function (q) {
            if (ifnoerror(q)) {
                var json = JSON.parse(q.body);
                if (json.error_code != 0) {
                    hdd(false, json.error_code, json.error_msg);
                } else {
                    hdd(json);
                }
            } else {
                hdd(false, false, false, true);
            }
        });
    }
    _s.tiebaApi.messageNum = function (hdd) {
        _s.tiebaApi.sendTieba("/c/s/msg", "", function (q) {
            if (ifnoerror(q)) {
                hdd(JSON.parse(q.body).message);
            } else {
                hdd({error_code: 88550, error_msg: "网络错误", message: {}});
            }
        });
    };
    _s.tiebaApi.fans = function (uid, pn, hdd) {
        _s.tiebaApi.sendTieba("/c/u/fans/page", "pn=" + pn + "&uid=" + ((uid == 0 || uid == undefined || uid < 0) ? _s.tiebaApi.getUid() : uid), function (q) {
            if (ifnoerror(q)) {
                var j = JSON.parse(q.body);
                hdd(j);
            } else {
                hdd(false);
            }
        })
    };
    _s.tiebaApi.gzs = function (uid, pn, hdd) {
        _s.tiebaApi.sendTieba("/c/u/follow/page", "pn=" + pn + "&uid=" + ((uid == 0 || uid == undefined || uid < 0) ? _s.tiebaApi.getUid() : uid), function (q) {
            if (ifnoerror(q)) {
                var j = JSON.parse(q.body);
                hdd(j);
            } else {
                hdd(false);
            }
        })
    };
    _s.tiebaApi.reply = function (uid, pn, hdd) {
        _s.tiebaApi.sendTieba("/c/u/feed/replyme", "pn=" + pn + "&uid=" + ((uid == 0 || uid == undefined || uid < 0) ? _s.tiebaApi.getUid() : uid), function (q) {
            if (ifnoerror(q)) {
                var j = JSON.parse(q.body);
                hdd(j);
            } else {
                hdd(false);
            }
        })
    };
    _s.tiebaApi.atme = function (uid, pn, hdd) {
        _s.tiebaApi.sendTieba("/c/u/feed/atme", "pn=" + pn + "&uid=" + ((uid == 0 || uid == undefined || uid < 0) ? _s.tiebaApi.getUid() : uid), function (q) {
            if (ifnoerror(q)) {
                var j = JSON.parse(q.body);
                hdd(j);
            } else {
                hdd(false);
            }
        })
    };
    _s.tiebaApi.pb = function (kz, pn, hdd) {
        _s.tiebaApi.sendTieba("/c/f/pb/page", "kz=" + encodeURIComponent(kz) + "&pn=" + pn, function (q) {
            if (ifnoerror(q)) {
                hdd(JSON.parse(q.body));
            } else {
                hdd(false);
            }
        })
    }
    _s.tiebaApi.delPost = function (pid, z, ba, hdd) {
        _s.tiebaApi.userInfo(0, function (u) {
            var tbs = u.anti.tbs;
            _s.tiebaApi.sendTieba("/c/c/bawu/delpost", "z=" + encodeURIComponent(z) + "&word=" + encodeURIComponent(ba) + "&pid=" + pid + "&tbs=" + tbs, function (q) {
                if (ifnoerror(q)) {
                    hdd(JSON.parse(q.body));
                } else {
                    hdd(false);
                }
            });
        });
    };
    /*_s.tiebaApi.vcodefunction = function (vcodeimg, codemd5, onok, onconcal) {
        var pint = $("<input style='width: 100%;' type='text'>");
        $(_.CreateMetroDlg(250)).css({padding: "15px"}).append($("<div style='font-size: 22px;'>需要验证码</div>")).append($("<div style='font-size: 18px;'>百度要求您为此次操作提供验证码</div>")).append($("<img src='" +
                vcodeimg + "'>")).append(pint).append($("<button>重试</button>").click(function () {
                var codeid = codemd5;
                var code = pint.val();
                _.CloseMetroDlg();
                onok(codeid, code);
            })).append($("<button>取消</button>").click(function () {
                onconcal();
                _.CloseMetroDlg();
            }));
    }*/
    _s.tiebaApi.addPost = function (con, pid, fn, tid, kw, kwid, hdd) {
        _s.tiebaApi.userInfo(0, function (u) {
            var tbs = u.anti.tbs;

            function yzmun(vcodemd5, code) {
                _s.tiebaApi.sendTieba("/c/c/post/add", "content=" + encodeURIComponent(con) + "&floor_num=" + fn + "&kw=" + encodeURIComponent(kw) + "&tid=" + tid + "&vcode=" + encodeURIComponent(code) +
                    "&vcode_md5=" + vcodemd5 + "&tbs=" + tbs + "&quote_id=" + pid + "&fid=" + kwid, function (q) {
                    if (ifnoerror(q)) {
                        if (JSON.parse(q.body).info.need_vcode && JSON.parse(q.body).info.vcode_pic_url) {
                            _s.tiebaApi.vcodefunction(JSON.parse(q.body).info.vcode_pic_url, JSON.parse(q.body).info.vcode_md5, yzmun, function () {
                                hdd(JSON.parse(q.body));
                            });
                        } else {
                            hdd(JSON.parse(q.body));
                        }
                    } else {
                        hdd(false);
                    }
                });
            }

            yzmun("", "");
        });
    };
    _s.tiebaApi.addThread = function (title, con, kw, kwid, hdd) {
        _s.tiebaApi.userInfo(0, function (u) {
            var tbs = u.anti.tbs;

            function yzmun(vcodemd5, code) {
                _s.tiebaApi.sendTieba("/c/c/thread/add", "content=" + encodeURIComponent(con) + "&kw=" + encodeURIComponent(kw) + "&vcode=" + encodeURIComponent(code) +
                    "&vcode_md5=" + vcodemd5 + "&tbs=" + tbs + "&fid=" + kwid + "&title=" + encodeURIComponent(title), function (q) {
                    if (ifnoerror(q)) {
                        if (JSON.parse(q.body).info && JSON.parse(q.body).info.need_vcode && JSON.parse(q.body).info.vcode_pic_url) {
                            _s.tiebaApi.vcodefunction(JSON.parse(q.body).info.vcode_pic_url, JSON.parse(q.body).info.vcode_md5, yzmun, function () {
                                hdd(JSON.parse(q.body));
                            });
                        } else {
                            hdd(JSON.parse(q.body));
                        }
                    } else {
                        hdd(false);
                    }
                });
            }

            yzmun("", "");
        });
    };
    _s.tiebaApi.postsee = function (pid, pn, kz, hdd) {
        _s.tiebaApi.sendTieba("/c/f/pb/floor", (kz ? ("kz=" + kz + "&") : "") + "pid=" + pid + "&pn=" + pn, function (q) {
            if (ifnoerror(q)) {
                hdd(JSON.parse(q.body));
            } else {
                hdd(false);
            }
        });
    }
    /*_s.tiebaApi.updataImage = function (base64enced, hdd) {
        _.sendThisServerHttp("/sys/php/postImgToaidu.php", "POST", "BDUSS=" + encodeURIComponent(_s.tiebaApi.getBduss()) + "&pic=" + encodeURIComponent(base64enced), function (text) {
            var q = JSON.parse(text);
            if (ifnoerror(q)) {
                hdd(JSON.parse(q.body));
            } else {
                hdd(false);
            }
        })
    }*/
    _s.tiebaApi.web = {};
    _s.tiebaApi.web.cookiebyBDUSS=function(bduss){
        return "BDUSS=" + encodeURIComponent(bduss) + "; BAIDUID=1674952A91B3F40D55193E1D527F9049:FG=1; TIEBAUID=edaa8f29dc77cd0780e78058; TIEBA_USERTYPE=e7d5aa4ea448ed84d7d493f2";
    }
    _s.tiebaApi.web.getmeizhi = function (uid, hdd) {
        sendHttp("tieba.baidu.com", "/encourage/get/meizhi/panel", "POST", "user_id=" + encodeURIComponent(uid) + "&type=1", 80, function (q) {
            if (ifnoerror(q)) {
                var json = JSON.parse(q.body);
                if (json.no == 210007) {
                    hdd(false, "未认证");
                } else if (json.no != 0) {
                    hdd(false, json.no, json.error);
                } else {
                    var dt = json.data;
                    if (dt == undefined) {
                        hdd(false, 0, "data not find");
                    } else {
                        hdd(dt);
                    }
                }
            } else {
                hdd(false, "无法获得妹子信息");
            }
        });
    }
    _s.tiebaApi.web.meizhuvd = function (uid, type, hdd) {
        var bduss= _s.tiebaApi.getBduss();
        _s.tiebaApi.web.getmeizhi(uid, function (q, n, m) {
            if (!q) {
                hdd(false, n, m);
            } else {
                var kw = q.forum_name;
                _s.tiebaApi.bar(kw, 0, 1, function (q) {
                    if (q == false) {
                        hdd(false, 0, "获得妹吧fid错误");
                    } else {
                        var fid = q.forum.id;
                        _s.tiebaApi.dowithbduss(_s.tiebaApi.web.tbs,bduss,0,function (q) {
                            var tbs = q.tbs;
                            _.sendOutserveredHttpWithCookie(_s.tiebaApi.web.cookiebyBDUSS(bduss)
                                , "tieba.baidu.com", "/encourage/post/meizhi/vote"
                                , "POST", "content=&tbs=" + encodeURIComponent(tbs) + "&kw=" + encodeURIComponent(kw) + "&uid=" + encodeURIComponent(uid) + "&scid=" + encodeURIComponent(_s.tiebaApi.getUid())
                                    + "&vtype=" + type + "&ie=utf-8&vcode=&new_vcode=1&tag=11&fid=" + encodeURIComponent(fid), 80, function (q) {
                                    if (ifnoerror(q)) {
                                        hdd(JSON.parse(q.body));
                                    } else {
                                        hdd(false, 0, "网络错误");
                                    }
                                });
                        });
                    }
                });
            }
        });
    };
    _s.tiebaApi.web.tbs = function (hdd) {
        _.sendOutserveredHttpWithCookie(_s.tiebaApi.web.cookiebyBDUSS(_s.tiebaApi.getBduss()), "tieba.baidu.com", "/dc/common/tbs", "GET", "", 80, function (q) {
            hdd(JSON.parse(q.body));
        });
    };
    _s.tiebaApi.uidByName=function(name,hdd){
        sendHttp("tieba.baidu.com","/i/sys/user_json?un="+encodeURIComponent(name)+"&ie=utf-8","GET"
            ,"",80,function(q){
                if(ifnoerror(q)){
                    var json=JSON.parse(q.body);
                    hdd(json.creator.id,json.creator.name);
                }else{
                    hdd(false);
                }
            });
    };
    _s.tiebaApi.uidByBDUSS=function(hdd){
        _s.tiebaApi.bar("机器猫",0,1,function(q){
            if(q && q.user && q.user.id){
                hdd(q.user.id, q.user.name);
            }else{
                hdd(false,false);
            }
        });
    };
    var base64enc=function(a){
        return new Buffer(a).toString('base64');
    }
    var base64dec=function(a){
        return new Buffer(a, 'base64').toString();
    }
    _s.tiebaApi.login = function (un, upass, hdd) {
        function sdvc(codemd5, code) {
            _s.tiebaApi.sendTieba("/c/s/login", "un=" + encodeURIComponent(un) + "&passwd=" + encodeURIComponent(base64enc(upass)) + "&vcode_md5=" + encodeURIComponent(codemd5) + "&vcode=" + encodeURIComponent(code), function (q) {
                if (ifnoerror(q)) {
                    var login = JSON.parse(q.body);
                    if (login.anti && login.anti.need_vcode == 1) {
                        var codemd5 = login.anti.vcode_md5;
                        var picurl = login.anti.vcode_pic_url;
                        _s.tiebaApi.vcodefunction(picurl, codemd5, sdvc, function () {
                            hdd(false, false, login.error_code, login.error_msg);
                        }, function () {
                            hdd(false, false, login.error_code, login.error_msg);
                        });
                        return;
                    }
                    if (login.error_code != 0) {
                        hdd(false, false, login.error_code, login.error_msg);
                        return;
                    }
                    hdd(login.user.BDUSS, login.user.id);
                } else {
                    hdd(false, false, false, false);
                }
            });
        }
        sdvc("", "");
    };
})();
