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
    const password = document.getElementById('NewPassword').value;
    const confirmPassword = document.getElementById('ConfirmPassword').value;

    const passwordError = document.getElementById('passwordError');
    const confirmPasswordError = document.getElementById('confirmPasswordError');

    // Updated password criteria: at least 12 alphanumeric characters
    const passwordCriteria = /^[A-Za-z\d]{12,}$/;

    // Validate password against criteria
    if (!passwordCriteria.test(password)) {
        passwordError.style.display = 'block';
        passwordError.textContent = "Password must be at least 12 characters long and contain only letters and numbers.";
    } else {
        passwordError.style.display = 'none';
    }

    // Validate password match
    if (password !== confirmPassword) {
        confirmPasswordError.style.display = 'block';
    } else {
        confirmPasswordError.style.display = 'none';
    }

    // Return false to prevent form submission if there are errors
    return passwordCriteria.test(password) && (password === confirmPassword);
}

