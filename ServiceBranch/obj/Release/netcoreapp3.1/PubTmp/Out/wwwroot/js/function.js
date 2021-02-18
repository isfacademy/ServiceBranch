

// Example starter JavaScript for disabling form submissions if there are invalid fields
(function () {
    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);
})();


//password / confirm password compare
function validatePassword() {
    if (password.value != confirm_password.value) {
        confirm_password.setCustomValidity("Passwords Don't Match");
    } else {
        confirm_password.setCustomValidity('');
    }
}
$(document).ready(function () {

    var password = document.getElementById("password")
        , confirm_password = document.getElementById("confirm_password");

    if (password != null && confirm_password != null) {
        // error onchange why u do this i cut my hand
        password.onchange = validatePassword;
        confirm_password.onkeyup = validatePassword;
    }
})

// this script only to show the file selected in input file
$(document).ready(function () {
    $('.custom-file-input').on("change", function () {
        var valbefor = $(this).val(); // C:\fakepath\IMG_0160.JPG
        // get name of image with extention
        var filename = $(this).val().split("\\").pop(); // IMG_0160.JPG
        // add image name to label
        $(this).parent('.custom-file').find('.custom-file-label').html(filename);
    });
});
