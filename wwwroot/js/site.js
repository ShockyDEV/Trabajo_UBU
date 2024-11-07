// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function showRequestForm() {
    document.getElementById('loginForm').style.display = 'none';
    document.getElementById('requestForm').style.display = 'block';
}

function showLoginForm() {
    document.getElementById('requestForm').style.display = 'none';
    document.getElementById('loginForm').style.display = 'block';
}


function validatePassword() {
    // Obtener los valores de los campos de nueva contraseña y confirmación de contraseña
    const password = document.getElementById('NewPassword').value;
    const confirmPassword = document.getElementById('ConfirmPassword').value;

    // Obtener los elementos para mostrar mensajes de error
    const passwordError = document.getElementById('passwordError');
    const confirmPasswordError = document.getElementById('confirmPasswordError');

    // Criterio de contraseña: al menos 12 caracteres alfanuméricos
    const passwordCriteria = /^[A-Za-z\d]{12,}$/;

    // Validar la contraseña según los criterios establecidos
    if (!passwordCriteria.test(password)) {
        // Mostrar el error si no cumple con los requisitos
        passwordError.style.display = 'block';
        passwordError.textContent = "La contraseña debe tener al menos 12 caracteres y contener solo letras y números.";
    } else {
        // Ocultar el mensaje de error si cumple con los requisitos
        passwordError.style.display = 'none';
    }

    // Verificar que la contraseña y la confirmación coincidan
    if (password !== confirmPassword) {
        // Mostrar el mensaje de error si no coinciden
        confirmPasswordError.style.display = 'block';
        confirmPasswordError.textContent = "Las contraseñas no coinciden.";
    } else {
        // Ocultar el mensaje de error si coinciden
        confirmPasswordError.style.display = 'none';
    }

    // Retornar true solo si se cumplen ambos requisitos para permitir el envío del formulario
    return passwordCriteria.test(password) && (password === confirmPassword);
}
