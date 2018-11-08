$(function () {
    $("form[name='resetForm']").validate({
        rules: {
            CurrentPassword: "required",
            Password: "required",
            PasswordConfirm: "required"
        },
        messages: {
            CurrentPassword: "Ingrese su contraseña actual",
            Password: "Ingrese una nueva contraseña",
            PasswordConfirm: "Confirme su nueva contraseña"
        },
        submitHandler: function (form) {
            var current = CurrentPassword.value;
            var newPassword = Password.value;
            var confirm = PasswordConfirm.value;
            if (current === newPassword) {
                toastr.warning("La nueva contraseña debe ser distinta de la actual", "Atención", { timeOut: 5000 });
                Password.focus();
            } else {
                if (newPassword !== confirm) {
                    toastr.warning("Las contraseñas no coinciden, verifique por favor", "Atención", { timeOut: 5000 });
                    Password.focus();
                    PasswordConfirm.value = "";
                } else {
                    form.sumbit();
                }
            }
        }
    });
});