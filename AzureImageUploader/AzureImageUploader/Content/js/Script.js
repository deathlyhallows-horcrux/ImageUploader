(function ($) {

    //  Dropzone.autoDiscover = false;
    Dropzone.options.myDropzone = {
        url: "/AzureImageUploader/Home/Index",
        //url: "Home/Index",
        type: "POST",
        success: function (response) {
            if (response != null && response.success) {
                alert(response.responseText);
            }
            var text = JSON.parse(response.xhr.responseText);
            console.log(text.responseText);
            var success = $("<p></p>").text(text.responseText);
            var error = $("<p></p>").text(text.responseText);
            $("#success").append(success);
            if (text.responseText.indexOf("not")>= 0) {
                $("#success").css("color", "red");
            }
            // $(".error").append(error);
            //$("#success").text(text.responseText);
            //$(".error").text(text.errormessage);
            console.log(response.xhr.responseText);
            console.log(success);
        },

    };

    $(document).ready(function () {
        $('#pkimages').prop('selected', true);
       

        var account = getURLParameter('account');
        $('#selectaccount').val(account);
        $(".imageupload").click(function () {
            $(".partial").hide();
            $(".imageupload").attr("class", "btn btn-primary btn-large imageupload active");
            $(".searchimages").attr("class", "btn btn-primary btn-large searchimages");
            $(".uploader").show();

        });
        $(".searchimages").click(function () {
            $(".uploader").hide();
            $("#success").hide();
            $(".imageupload").attr("class", "btn btn-primary btn-large imageupload");
            $(".searchimages").attr("class", "btn btn-primary btn-large searchimages active");
            //  $(".partial").css({ display: "block !important" });
            $(".partial").show();
        });
        $("#blob").click(function () {
            $(".uploader").hide();
            $(".imageupload").attr("class", "btn btn-primary btn-large imageupload");
            $(".searchimages").attr("class", "btn btn-primary btn-large searchimages active");
            //  $(".partial").css({ display: "block !important" });
            $(".partial").show();
        })

        if (window.location.href.indexOf("Azurelistblobs") != -1) {
            $(".searchimages").attr("class", "btn btn-primary btn-large searchimages active");
            $(".uploader").hide();
            $(".imageupload").attr("class", "btn btn-primary btn-large imageupload");
            $(".searchimages").attr("class", "btn btn-primary btn-large searchimages active");
            //  $(".partial").css({ display: "block !important" });
            $(".partial").show();
        }
        else {
            $(".imageupload").attr("class", "btn btn-primary btn-large active imageupload");
        }

        $('.delimage').click(function (e) {
            e.preventDefault();
            //  var url = document.getElementById("imagetodelete").src;
            //  var result = url.replace('https://', '').split('/');
            //var account = document.getElementById("account").data("myAcc");
            var parentrow = $(this).parent().parent();
            var account = $("#account").attr("value");
            var filename = $(this).parent().parent().find(".image").attr("value");
            //var storageaccount = result[1];
            //   var filename = document.getElementById("imagename").data("myValue");
            //  console.log(filename, storageaccount);
            $.ajax({
                type: "POST",
                traditional: true,
                dataType: "json",
                url: "/AzureImageUploader/Home/AzureDelete",
                //url: "Home/Azuredelete",
                data: { image: filename, account: account },
                success: function (response) {
                    console.log(response);
                    if (response != null && response.success) {
                        $(".write").text(response.responseText);
                        parentrow.hide();
                        $("#msgstatus").hide();
                    }
                    else {
                        $(".write").text(response.responseText);
                    }
                }
            });


            //function searchImage() {

            //    var storageaccount = document.getElementsByName("account")[0].value;
            //    var filename = document.getElementsByName("image")[0].value;
            //    console.log(storageaccount, filename);
            //    if (filename != null && filename != "") {
            //        var url = "https://" + storageaccount.toLowerCase() + ".blob.core.windows.net/" + storageaccount.toLowerCase() + "/" + filename;
            //        var request = new XMLHttpRequest();
            //        request.open('GET', url, true);
            //        request.onreadystatechange = function () {
            //            if (request.readyState === 4) {
            //                if (request.status == 200 || request.status == 0) {
            //                    document.getElementById("imagedisplay").src = url;
            //                    document.getElementById("deleteimage").style.display = "block";
            //                    document.getElementsByClassName("write")[0].style.display = "none";
            //                }
            //                else {
            //                    alert("image does not exist");
            //                }
            //            }
            //        };
            //        request.send();


            //        document.getElementById("deleteimage").style.display = "block";
            //        document.getElementsByClassName("write")[0].style.display = "none";
            //    }
            //    else {
            //        document.getElementsByClassName("write")[0].innerHTML = "Enter image name";

            //    }
            //}

        });

        //$('#purgepk').click(function (e) {
        //    e.preventDefault();
        //    $.ajax({
        //        type: "POST",
        //        traditional: true,
        //        dataType: "JSON",
        //        url: "/AzureImageUploader/Home/AzureUpdate",
        //        data: {'purge': true},
        //        //jsonp: 'jsonp',
        //        //jsonpcallback: 'jsoncallback',
        //        success: function (response) {
        //            console.log(response);
        //            if (response != null && response.success) {
        //                $("#success").text(response.responseText);
        //            }
        //            else {
        //                $("#success").text(response.responseText);
        //            }
        //        }
        //    });
        //});

        function getURLParameter(name) {
            return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null
        }
    });

 
    function search() {
        if (document.URL.indexOf("Azurelistblobs")) {
            jQuery(".searchimages").click(function () {
                jQuery(".uploader").hide();
                jQuery(".imageupload").attr("class", "btn btn-primary btn-large imageupload");
                jQuery(".searchimages").attr("class", "btn btn-primary btn-large searchimages active");
                //  $(".partial").css({ display: "block !important" });
                jQuery(".partial").show();

            });

            // alert("search function");

        }
    }
})(jQuery);


