/**
 * Created by Malal91 and Haziel
 * Select multiple email by jquery.email_multiple
 * **/

(function ($) {

    $.fn.email_multiple = function (options) {

        let defaults = {
            reset: false,
            fill: false,
            data: null
        };

        let settings = $.extend(defaults, options);
        let email = "";
        let totalEmail = 0;
        return this.each(function () {
            $(this).after(
                "<input type=\"email\" name=\"email\" class=\"enter-mail-id form-control mb-2\" placeholder=\"Enter Email and press enter...\" />" +
                "<div class=\"all-mail\"></div>\n"
            );
            let $orig = $(this);
            let $element = $('.enter-mail-id');
            $element.keydown(function (e) {
                $element.css('border', '');
                if (e.keyCode === 13 || e.keyCode === 32) {
                    let getValue = $element.val();
                    if (/^[a-z0-9._-]+@[a-z0-9._-]+\.[a-z]{2,6}$/.test(getValue)) {
                        $('.all-mail').append('<span class="email-ids">' + getValue + '<span class="cancel-email">x</span></span>');
                        $element.val('');

                        email += getValue + ';'
                    } else {
                        $element.css('border', '1px solid red')
                    }
                }

                $orig.val(email.slice(0, -1))
            });

            $(document).on('click', '.cancel-email', function () {
                $(this).parent().remove();
                totalEmail -= 1;
            });

            if (settings.data) {
                $.each(settings.data, function (x, y) {
                    if (totalEmail > 3) {
                        $element.css('border', '1px solid red');
                    }
                    else if (/^[a-z0-9._-]+@[a-z0-9._-]+\.[a-z]{2,6}$/.test(y)) {
                        $('.all-mail').append('<span class="email-ids">' + y + '<span class="cancel-email">x</span></span>');
                        $element.val('');
                        totalEmail += 1;
                        console.log(totalEmail);
                        email += y + ';'
                    } else {
                        $element.css('border', '1px solid red');
                    }
                })
                $orig.val(email.slice(0, -1))
            }

            if (settings.reset) {
                $('.email-ids').remove()
            }

            return $orig.hide()
        });
    };

})(jQuery);
