$(function () {
    $("form[name='recoverForm']").validate({
        rules: {
            CurrentPassword: "required",
            Email: "required"
        },
        messages: {
            CurrentPassword: "Ingrese su contraseña actual",
            Email: "Ingrese su correo electrónico"
        },
        submitHandler: function (form) {
            var email = Email.value;
            var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (!regex.test(email)) {
                toastr.error("Debe ingresar una cuenta de correo válida", "Atención", { timeOut: 5000 });
                Email.focus();
            } else {
                form.sumbit();
            }
        },
        errorClass: "error-class-validation"
    });
});